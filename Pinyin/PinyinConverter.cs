using System.Reflection;
using System.Text;
using System.Text.Json;

namespace Pinyin;

/// <summary>
/// 提供汉字转拼音的功能
/// </summary>
public static class PinyinConverter
{
    private static readonly Dictionary<string, string[]> _pinyinDict;
    private static readonly Dictionary<string, string> _wordPinyinDict;
    private static readonly Dictionary<string, Dictionary<string, string>> _multipleDict;

    // 声调标记
    private static readonly Dictionary<char, int> _toneMarks = new Dictionary<char, int>
    {
        {'ā', 1}, {'á', 2}, {'ǎ', 3}, {'à', 4},
        {'ē', 1}, {'é', 2}, {'ě', 3}, {'è', 4},
        {'ī', 1}, {'í', 2}, {'ǐ', 3}, {'ì', 4},
        {'ō', 1}, {'ó', 2}, {'ǒ', 3}, {'ò', 4},
        {'ū', 1}, {'ú', 2}, {'ǔ', 3}, {'ù', 4},
        {'ǖ', 1}, {'ǘ', 2}, {'ǚ', 3}, {'ǜ', 4},
        {'ü', 0}
    };

    // 韵母
    private static readonly Dictionary<char, char[]> _vowels = new Dictionary<char, char[]>
    {
        {'a', new[] {'ā', 'á', 'ǎ', 'à', 'a'}},
        {'e', new[] {'ē', 'é', 'ě', 'è', 'e'}},
        {'i', new[] {'ī', 'í', 'ǐ', 'ì', 'i'}},
        {'o', new[] {'ō', 'ó', 'ǒ', 'ò', 'o'}},
        {'u', new[] {'ū', 'ú', 'ǔ', 'ù', 'u'}},
        {'v', new[] {'ǖ', 'ǘ', 'ǚ', 'ǜ', 'ü'}}
    };

    // 初始汉字范围
    private const int CHINESE_CHAR_START = 0x4e00;
    private const int CHINESE_CHAR_END = 0x9fff;

    static PinyinConverter()
    {
        // 加载拼音字典 - 从空格分隔的字符串转换为数组
        var rawPinyinDict = LoadResource<Dictionary<string, string>>("pinyin-dict.json");
        _pinyinDict = rawPinyinDict.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries)
        );

        // 加载词语拼音字典
        _wordPinyinDict = LoadResource<Dictionary<string, string>>("word-pinyin-dict.json");

        // 加载多音字词典
        _multipleDict = LoadResource<Dictionary<string, Dictionary<string, string>>>("multiple-dict.json");
    }

    /// <summary>
    /// 将汉字转换为拼音
    /// </summary>
    /// <param name="text">要转换的汉字文本</param>
    /// <param name="options">转换选项</param>
    /// <returns>拼音结果</returns>
    public static string GetPinyin(string text, PinyinOptions options = null)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        options = options ?? new PinyinOptions();
        var result = new StringBuilder();

        // 分词处理
        var segments = SegmentText(text);

        foreach (var segment in segments)
        {
            // 检查是否是词组
            if (segment.Length > 1 && _wordPinyinDict.TryGetValue(segment, out var wordPinyin))
            {
                // 处理词组拼音
                var formattedPinyin = FormatPinyinWord(wordPinyin, options);
                result.Append(formattedPinyin);
            }
            else
            {
                // 处理单字拼音
                for (int i = 0; i < segment.Length; i++)
                {
                    // 获取上下文
                    string prevChar = i > 0 ? segment[i - 1].ToString() : null;
                    string nextChar = i < segment.Length - 1 ? segment[i + 1].ToString() : null;

                    var charPinyin = GetCharPinyin(segment[i], options, prevChar, nextChar);
                    result.Append(charPinyin);

                    // 添加分隔符，但不在最后一个字符后添加
                    if (!string.IsNullOrEmpty(options.Separator) &&
                        i < segment.Length - 1 &&
                        IsChineseChar(segment[i]) &&
                        IsChineseChar(segment[i + 1]))
                    {
                        result.Append(options.Separator);
                    }
                }
            }
        }

        return result.ToString();
    }

    /// <summary>
    /// 获取单个汉字的拼音
    /// </summary>
    /// <param name="c">汉字</param>
    /// <param name="options">选项</param>
    /// <param name="prev">前一个字符</param>
    /// <param name="next">后一个字符</param>
    /// <returns>拼音</returns>
    public static string GetCharPinyin(char c, PinyinOptions options = null, string prev = null, string next = null)
    {
        options = options ?? new PinyinOptions();

        // 如果不是汉字，直接返回
        if (!IsChineseChar(c))
            return c.ToString();

        string[] pinyins;
        string charStr = c.ToString();

        // 处理多音字
        if (options.Heteronym && TryGetMultiplePinyin(charStr, prev, next, out pinyins))
        {
            // 使用多音字逻辑
        }
        else if (_pinyinDict.TryGetValue(charStr, out pinyins))
        {
            // 直接从字典获取
        }
        else
        {
            // 找不到拼音，返回原字符
            return c.ToString();
        }

        // 默认取第一个读音
        string pinyin = pinyins.FirstOrDefault() ?? c.ToString();

        return FormatPinyin(pinyin, options);
    }

    /// <summary>
    /// 获取拼音首字母
    /// </summary>
    /// <param name="text">汉字文本</param>
    /// <returns>首字母串</returns>
    public static string GetFirstLetter(string text)
    {
        var options = new PinyinOptions { Format = PinyinFormat.FirstLetter };
        return GetPinyin(text, options);
    }

    /// <summary>
    /// 判断是否为汉字
    /// </summary>
    public static bool IsChineseChar(char c)
    {
        return c >= CHINESE_CHAR_START && c <= CHINESE_CHAR_END;
    }

    /// <summary>
    /// 格式化拼音
    /// </summary>
    private static string FormatPinyin(string pinyin, PinyinOptions options)
    {
        var result = options.Format switch
        {
            PinyinFormat.WithTone => pinyin,
            PinyinFormat.WithoutTone => RemoveTone(pinyin),
            PinyinFormat.WithToneNumber => ConvertToToneNumber(pinyin),
            PinyinFormat.FirstLetter => RemoveTone(pinyin).FirstOrDefault().ToString(),
            _ => RemoveTone(pinyin)
        };
        if (result != null && options.Case == PinyinCase.Upper)
            result = result.ToUpper();
        return result;
    }

    /// <summary>
    /// 格式化词组拼音
    /// </summary>
    private static string FormatPinyinWord(string pinyin, PinyinOptions options)
    {
        // 处理词组拼音格式
        var parts = pinyin.Split(' ');
        var result = new StringBuilder();

        for (int i = 0; i < parts.Length; i++)
        {
            result.Append(FormatPinyin(parts[i], options));

            if (i < parts.Length - 1 && !string.IsNullOrEmpty(options.Separator))
            {
                result.Append(options.Separator);
            }
        }

        return result.ToString();
    }

    /// <summary>
    /// 移除拼音声调
    /// </summary>
    private static string RemoveTone(string pinyin)
    {
        var result = new StringBuilder();

        foreach (char c in pinyin)
        {
            // 检查是否是带声调的字符
            bool found = false;
            foreach (var vowel in _vowels)
            {
                if (vowel.Value.Contains(c))
                {
                    result.Append(vowel.Key);
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                result.Append(c);
            }
        }

        return result.ToString();
    }

    /// <summary>
    /// 将带声调拼音转换为数字声调格式
    /// </summary>
    private static string ConvertToToneNumber(string pinyin)
    {
        foreach (char c in pinyin)
        {
            if (_toneMarks.TryGetValue(c, out int tone) && tone > 0)
            {
                return RemoveTone(pinyin) + tone;
            }
        }

        return RemoveTone(pinyin) + "5"; // 轻声
    }

    /// <summary>
    /// 尝试获取多音字拼音
    /// </summary>
    private static bool TryGetMultiplePinyin(string c, string prev, string next, out string[] pinyins)
    {
        pinyins = null;

        // 尝试根据上下文确定多音字的读音
        if (_multipleDict.TryGetValue(c, out var contextDict))
        {
            string context = (prev ?? "") + (next ?? "");

            foreach (var entry in contextDict)
            {
                if (context.Contains(entry.Key))
                {
                    // 将找到的拼音字符串转换为单元素数组
                    pinyins = new[] { entry.Value };
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// 分词处理文本
    /// </summary>
    private static IEnumerable<string> SegmentText(string text)
    {
        // 简单实现：按照汉字/非汉字分组
        int start = 0;
        bool isChineseMode = IsChineseChar(text[0]);

        for (int i = 1; i < text.Length; i++)
        {
            bool currentIsChinese = IsChineseChar(text[i]);

            if (currentIsChinese != isChineseMode)
            {
                yield return text.Substring(start, i - start);
                start = i;
                isChineseMode = currentIsChinese;
            }
        }

        // 返回最后一部分
        yield return text.Substring(start);
    }

    /// <summary>
    /// 从嵌入资源加载数据
    /// </summary>
    private static T LoadResource<T>(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var fullResourceName = $"Pinyin.Resources.{resourceName}";

        using Stream stream = assembly.GetManifestResourceStream(fullResourceName) ?? throw new Exception($"找不到资源: {fullResourceName}");
        using StreamReader reader = new(stream, Encoding.UTF8);
        string json = reader.ReadToEnd();
        return JsonSerializer.Deserialize<T>(json);
    }
}

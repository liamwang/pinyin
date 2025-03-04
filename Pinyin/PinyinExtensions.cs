namespace Pinyin;

/// <summary>
/// 拼音扩展方法
/// </summary>
public static class PinyinExtensions
{
    /// <summary>
    /// 将字符串转换为拼音
    /// </summary>
    public static string ToPinyin(this string text, PinyinOptions options = null)
    {
        return PinyinConverter.GetPinyin(text, options);
    }
    
    /// <summary>
    /// 将字符串转换为拼音首字母
    /// </summary>
    public static string ToFirstLetters(this string text)
    {
        return PinyinConverter.GetFirstLetter(text);
    }
    
    /// <summary>
    /// 判断字符是否为汉字
    /// </summary>
    public static bool IsChineseChar(this char c)
    {
        return PinyinConverter.IsChineseChar(c);
    }
}
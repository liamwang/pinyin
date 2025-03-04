using Pinyin;

Console.OutputEncoding = System.Text.Encoding.UTF8;

// 显示标题
Console.WriteLine("拼音转换器示例");
Console.WriteLine("-----------------------\n");

// 示例中文文本
string chineseText = "你好世界";
Console.WriteLine($"原始文本: {chineseText}");

// 示例1：默认样式（带声调标记）
string withToneMark = PinyinConverter.GetPinyin(chineseText);
Console.WriteLine($"\n1. 带声调标记: {withToneMark}");

// 示例2：带数字声调
string withToneNumber = PinyinConverter.GetPinyin(chineseText, new PinyinOptions { Format = PinyinFormat.WithToneNumber });
Console.WriteLine($"2. 带数字声调: {withToneNumber}");

// 示例3：不带声调
string withoutTone = PinyinConverter.GetPinyin(chineseText, new PinyinOptions { Format = PinyinFormat.WithoutTone });
Console.WriteLine($"3. 不带声调: {withoutTone}");

// 示例4：仅首字母
string firstLetter = PinyinConverter.GetFirstLetter(chineseText);
Console.WriteLine($"4. 仅首字母: {firstLetter}");

// 示例5：大写输出
var upperCase = PinyinConverter.GetPinyin(chineseText, new PinyinOptions { Format = PinyinFormat.WithoutTone, Case = PinyinCase.Upper });
Console.WriteLine($"5. 大写输出: {upperCase}");

// 示例6：自定义分隔符
var customSeparator = PinyinConverter.GetPinyin(chineseText, new PinyinOptions { Separator = "-" });
Console.WriteLine($"6. 自定义分隔符: {customSeparator}");

// 示例7：多音字处理
Console.WriteLine($"7. 多音字处理: ");
Console.WriteLine($"   (1) 行为: {PinyinConverter.GetPinyin("行为")}");
Console.WriteLine($"       因为: {PinyinConverter.GetPinyin("因为")}");
Console.WriteLine($"   (2) 我长大了: {PinyinConverter.GetPinyin("我长大了")}");
Console.WriteLine($"       头发很长: {PinyinConverter.GetPinyin("头发很长")}");

// 示例5：自定义文本输入
Console.WriteLine("-----------------------");
Console.WriteLine("请输入要转换的中文文本（按回车键退出）：");

while (true)
{
    Console.Write("> ");
    string? input = Console.ReadLine();

    if (string.IsNullOrEmpty(input))
        break;

    string result = PinyinConverter.GetPinyin(input);
    Console.WriteLine($"结果: {result}");
}

Console.WriteLine("\n感谢使用拼音转换器！");

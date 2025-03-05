# Pinyin

这是一个将中文字符转换为拼音的库。

## 功能特点

- 支持多种拼音输出格式（带声调、数字声调、无声调）
- 智能处理多音字
- 支持自定义分隔符
- 支持大小写转换
- 支持首字母提取

## 快速开始

1. 通过 NuGet 安装包（TODO）
   该类库还处于开发阶段，尚未发布到 NuGet。你可以 Clone 下来，将 `Pinyin` 文件夹直接复制到你的项目中使用。

2. 在您的项目中引用：
   ```csharp
   using Pinyin;
   ```

3. 开始使用：
   ```csharp
   string pinyin = PinyinConverter.GetPinyin("你好");
   ```

## 使用方法

```csharp
using Pinyin;

// 设置输出编码以正确显示中文字符
Console.OutputEncoding = System.Text.Encoding.UTF8;

// 示例中文文本
string chineseText = "你好世界";

// 默认转换（带声调标记）
string withToneMark = PinyinConverter.GetPinyin(chineseText);
// 输出: nǐ hǎo shì jiè

// 带数字声调
string withToneNumber = PinyinConverter.GetPinyin(chineseText, 
    new PinyinOptions { Format = PinyinFormat.WithToneNumber });
// 输出: ni3 hao3 shi4 jie4

// 不带声调
string withoutTone = PinyinConverter.GetPinyin(chineseText, 
    new PinyinOptions { Format = PinyinFormat.WithoutTone });
// 输出: ni hao shi jie

// 仅首字母
string firstLetter = PinyinConverter.GetFirstLetter(chineseText);
// 输出: n h s j

// 大写输出
var upperCase = PinyinConverter.GetPinyin(chineseText, 
    new PinyinOptions { Format = PinyinFormat.WithoutTone, Case = PinyinCase.Upper });
// 输出: NI HAO SHI JIE

// 自定义分隔符
var customSeparator = PinyinConverter.GetPinyin(chineseText, 
    new PinyinOptions { Separator = "-" });
// 输出: nǐ-hǎo-shì-jiè
```

更多请查看 `PinyinExample` 项目中的使用示例。

### 多音字处理

本库能够根据上下文智能处理具有多种发音的汉字，已适配常用词组，但上下文智能处理功能尚不完善。

```csharp
// 多音字示例
PinyinConverter.GetPinyin("行为"); // xíng wéi
PinyinConverter.GetPinyin("因为"); // yīn wèi

PinyinConverter.GetPinyin("我长大了"); // wǒ zhǎng dà le 
PinyinConverter.GetPinyin("头发很长"); // tóu fà hěn cháng
```

### 配置选项

`PinyinOptions` 类提供了多种配置选项：

```csharp
var options = new PinyinOptions 
{
    Format = PinyinFormat.WithToneNumber, // WithToneNumber, WithToneMark 或 WithoutTone
    Case = PinyinCase.Lower,              // Lower 或 Upper
    Separator = " ",                      // 用于分隔音节的字符
};

string result = PinyinConverter.GetPinyin("中文文本", options);
```

## JSON 数据格式

本库使用多个 JSON 文件进行汉字到拼音的映射：

- `pinyin-dict.json`：映射单个汉字到其拼音读音
- `word-pinyin-dict.json`：映射中文词语到其拼音读音
- `multiple-dict.json`：处理具有多种读音的汉字在不同上下文中的发音（待完善）

## 贡献

欢迎提交问题报告和拉取请求来帮助改进这个库。

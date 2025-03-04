namespace Pinyin;

/// <summary>
/// 拼音转换选项
/// </summary>
public class PinyinOptions
{
    /// <summary>
    /// 拼音格式
    /// </summary>
    public PinyinFormat Format { get; set; } = PinyinFormat.WithTone;
    
    /// <summary>
    /// 多音字处理
    /// </summary>
    public bool Heteronym { get; set; } = true;
    
    /// <summary>
    /// 拼音分隔符
    /// </summary>
    public string Separator { get; set; } = " ";
    
    /// <summary>
    /// 拼音大小写
    /// </summary>
    public PinyinCase Case { get; set; } = PinyinCase.Lower;
}

/// <summary>
/// 拼音格式
/// </summary>
public enum PinyinFormat
{
    /// <summary>
    /// 带声调标记，如：zhōng guó
    /// </summary>
    WithTone,
    
    /// <summary>
    /// 不带声调，如：zhong guo
    /// </summary>
    WithoutTone,
    
    /// <summary>
    /// 数字声调，如：zhong1 guo2
    /// </summary>
    WithToneNumber,
    
    /// <summary>
    /// 首字母，如：z g
    /// </summary>
    FirstLetter
}

/// <summary>
/// 拼音大小写
/// </summary>
public enum PinyinCase
{
    /// <summary>
    /// 小写，如：zhong guo
    /// </summary>
    Lower,
    
    /// <summary>
    /// 大写，如：ZHONG GUO
    /// </summary>
    Upper
}
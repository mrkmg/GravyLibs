namespace Gravy.MetaString;

public class MetaStringGeneratorAttribute : Attribute
{
    public string ContainedType { get; }
    public bool Empty { get; set; } = true;
    public bool Equality { get; set; } = true;
    public bool Operators { get; set; } = true;
    public bool StringMethods { get; set; } = true;
    
    public MetaStringGeneratorAttribute(string containedType)
    {
        ContainedType = containedType;
    }
}
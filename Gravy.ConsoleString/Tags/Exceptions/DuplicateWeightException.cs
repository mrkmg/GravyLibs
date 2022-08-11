namespace Gravy.ConsoleString.Tags;

public class DuplicateWeightException : TagsCompilerException
{
    public FontWeight Value { get; set; }
    
    public DuplicateWeightException(FontWeight value, int line, int column) : base(line, column)
    {
        Value = value;
    }

    public override string Message => $"Duplicate Weight '{Value}' at {Line}:{Column}";
}
namespace Gravy.ConsoleString.Tags;

public class DuplicateStyleException : TagsCompilerException
{
    public FontStyle Value { get; set; }
    
    public DuplicateStyleException(FontStyle value, int line, int column) : base(line, column)
    {
        Value = value;
    }

    public override string Message => $"Duplicate Style '{Value}' at {Line}:{Column}";
}
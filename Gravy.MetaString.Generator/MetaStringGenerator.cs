using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Gravy.MetaString.Generator;

[Generator]
public class MetaStringGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxReceiver is not SyntaxReceiver { ClassData: { } classData })
            return;

        string Convert(string inner)
            => $"(new {classData.ClassName}(({inner}).MetaData))";
        
        var sb = new StringBuilder();

        foreach(var line in classData.Usings) sb.AppendLine(line);
        if (!classData.Usings.Contains("using Gravy.MetaString;"))
            sb.AppendLine("using Gravy.MetaString;");
        sb.AppendLine("using System.Runtime.CompilerServices;");
        
        sb.Append($@"
namespace {classData.Namespace};
#nullable enable
public partial class {classData.ClassName} : {classData.BaseClassName}
{{");
        
        if (classData.StringMethods)
            sb.Append($@"
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static {classData.ClassName}[] MetaStringGenerator_ArrayConvert({classData.BaseClassName}[] array)
    {{
        var result = new {classData.ClassName}[array.Length];
        for (int i = 0; i < array.Length; i++)
            result[i] = {Convert("array[i]")};
        return result;
    }}

    public override {classData.ClassName} this[Range range] => {Convert("base[range]")};
    public override {classData.ClassName} PadLeft(int length) => {Convert("base.PadLeft(length)")};
    public override {classData.ClassName} PadLeft(char ch, int length) => {Convert("base.PadLeft(ch, length)")};
    public override {classData.ClassName} PadRight(int length) => {Convert("base.PadRight(length)")};
    public override {classData.ClassName} PadRight(char ch, int length) => {Convert("base.PadRight(ch, length)")};
    public override {classData.ClassName} Trim() => {Convert("base.Trim()")};
    public override {classData.ClassName} Trim(char ch) => {Convert("base.Trim(ch)")};
    public override {classData.ClassName} Trim(params char[] ch) => {Convert("base.Trim(ch)")};
    public override {classData.ClassName} Substring(int start, int length = 0) => {Convert("base.Substring(start, length)")};
    public override {classData.ClassName}[] Split(char separator) => MetaStringGenerator_ArrayConvert(base.Split(separator));
    public override {classData.ClassName}[] Split(char separator, StringSplitOptions options) => MetaStringGenerator_ArrayConvert(base.Split(separator, options));
    public override {classData.ClassName}[] Split(char separator, int count, StringSplitOptions options = StringSplitOptions.None) => MetaStringGenerator_ArrayConvert(base.Split(separator, count, options));
    public override {classData.ClassName}[] Split(char[] separators) => MetaStringGenerator_ArrayConvert(base.Split(separators));
    public override {classData.ClassName}[] Split(char[] separator, StringSplitOptions options) => MetaStringGenerator_ArrayConvert(base.Split(separator, options));
    public override {classData.ClassName}[] Split(char[] separator, int count, StringSplitOptions options = StringSplitOptions.None) => MetaStringGenerator_ArrayConvert(base.Split(separator, count, options));
    public override {classData.ClassName}[] Split(string separator) => MetaStringGenerator_ArrayConvert(base.Split(separator));
    public override {classData.ClassName}[] Split(string separator, StringSplitOptions options) => MetaStringGenerator_ArrayConvert(base.Split(separator, options));
    public override {classData.ClassName}[] Split(string separator, int count, StringSplitOptions options = StringSplitOptions.None) => MetaStringGenerator_ArrayConvert(base.Split(separator, count, options));
    public override {classData.ClassName}[] Split(string[] separator) => MetaStringGenerator_ArrayConvert(base.Split(separator));
    public override {classData.ClassName}[] Split(string[] separator, StringSplitOptions options) => MetaStringGenerator_ArrayConvert(base.Split(separator, options));
    public override {classData.ClassName}[] Split(string[] separator, int count, StringSplitOptions options = StringSplitOptions.None) => MetaStringGenerator_ArrayConvert(base.Split(separator, count, options));");

        if (classData.Equality)
            sb.Append($@"
    private bool Equals({classData.ClassName} other) => MetaEntries.SequenceEqual(other.MetaEntries); 
    public override bool Equals(object? obj) => obj is {classData.ClassName} other && Equals(other);
    public override int GetHashCode() => MetaEntries.GetHashCode();
    public static bool operator ==({classData.ClassName} first, {classData.ClassName} second) => first.Equals(second);
    public static bool operator !=({classData.ClassName} first, {classData.ClassName} second) => !first.Equals(second);");

        if (classData.Operators)
            sb.Append($@"
    public static {classData.ClassName} operator +({classData.ClassName} first, {classData.ClassName} second) => {Convert($"(({classData.BaseClassName})first) + (({classData.BaseClassName})second)")};
    public static {classData.ClassName} operator +({classData.ClassName} first, string second) => {Convert($"(({classData.BaseClassName})first) + second")};
    public static {classData.ClassName} operator +(string first, {classData.ClassName} second) => {Convert($"first + (({classData.BaseClassName})second)")};
    public static {classData.ClassName} operator +({classData.ClassName} first, object second) => {Convert($"(({classData.BaseClassName})first) + second")};
    public static {classData.ClassName} operator +(object first, {classData.ClassName} second) => {Convert($"first + (({classData.BaseClassName})second)")};");

        if (classData.Empty)
            sb.Append($@"
    public new static {classData.ClassName} Empty {{ get; }} = new(Array.Empty<MetaEntry<{classData.MetaType}>>());");

        sb.Append(@"
}");

        if (classData.DotMeta)
            sb.Append($@"
public static class MetaString_{classData.ClassName}_Generated {{
    public static {classData.ClassName} Meta(this string str) => {Convert($"str.Meta<{classData.MetaType}>()")};
    public static {classData.ClassName} Meta(this string str, {classData.MetaType} meta) => {Convert($"str.Meta<{classData.MetaType}>(meta)")};
}}");
        
        var sourceText = SourceText.From(sb.ToString(), Encoding.UTF8);
        context.AddSource($"{classData.ClassName}.g.cs", sourceText);
    }

    private class SyntaxReceiver : ISyntaxReceiver
    {
        public ClassData? ClassData { get; set; }

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is not ClassDeclarationSyntax cds) return;
            if (cds.Modifiers.All(st => st.RawKind != (int)SyntaxKind.PartialKeyword)) return;
            // WaitForDebug();
            if (!cds.AttributeLists.SelectMany(x => x.Attributes).Select(a => a.Name.ToString()).Contains("MetaStringGenerator")) return;
            ClassData = new(cds);
        }
    }

#if DEBUG
    // ReSharper disable once UnusedMember.Local
    private static void WaitForDebug()
    {
        Console.Error.WriteLine($"PID: {Process.GetCurrentProcess().Id}");
        while (!Debugger.IsAttached)
        {
            Thread.Sleep(100);
        }
    }
#endif
    
    private class ClassData
    {
        public readonly string ClassName;
        public readonly string BaseClassName;
        public readonly string Namespace;
        public readonly string[] Usings;
        public readonly string MetaType;
        public readonly bool Empty = true;
        public readonly bool Equality = true;
        public readonly bool Operators = true;
        public readonly bool StringMethods = true;
        public readonly bool DotMeta = true;

        public ClassData(ClassDeclarationSyntax classToAugment)
        {
            ClassName = classToAugment.Identifier.ToString();

            if (TryGetParentSyntax<NamespaceDeclarationSyntax>(classToAugment, out var nsTag))
                Namespace = nsTag!.Name.ToString();
            else if (TryGetParentSyntax<FileScopedNamespaceDeclarationSyntax>(classToAugment, out var fsTag))
                Namespace = fsTag!.Name.ToString();
            else 
                Namespace = string.Empty;
            
            Usings = classToAugment.SyntaxTree.GetCompilationUnitRoot().Usings.Select(x => x.ToString()).ToArray();

            var attributes = classToAugment.AttributeLists
                .SelectMany(x => x.Attributes)
                .First(x => x.Name.ToString().Contains("MetaStringGenerator"))
                // Must have at lease one argument (the base class)
                .ArgumentList!.Arguments;


            MetaType = GetNameOfValue(attributes[0]);
            // Can use shortened name as "using Gravy.MetaString;" is always added
            BaseClassName = $"MetaString<{MetaType}>";
            
            foreach (var attribute in attributes.Skip(1))
            {
                switch (attribute.NameEquals?.Name.Identifier.ValueText)
                {
                    case "Empty":
                        Empty = GetBoolValue(attribute);
                        break;
                    case "Equality":
                        Equality = GetBoolValue(attribute);
                        break;
                    case "Operators":
                        Operators = GetBoolValue(attribute);
                        break;
                    case "StringMethods":
                        StringMethods = GetBoolValue(attribute);
                        break;
                    case "DotMeta":
                        DotMeta = GetBoolValue(attribute);
                        break;
                }
            }
        }
    }
    
    
    private static bool GetBoolValue(AttributeArgumentSyntax attribute)
        => attribute.Expression.NormalizeWhitespace().ToString().Trim() == "true";

    private static string GetNameOfValue(AttributeArgumentSyntax attribute)
    {
        var value = attribute.Expression.NormalizeWhitespace().ToString().Trim();
        return value.StartsWith("nameof(") ? value.Substring(7, value.Length - 8) : value;
    }

    private static bool TryGetParentSyntax<T>(SyntaxNode? syntaxNode, out T? result) 
        where T : SyntaxNode
    {
        // set defaults
        result = null;

        if (syntaxNode == null)
        {
            return false;
        }

        try
        {
            syntaxNode = syntaxNode.Parent;

            if (syntaxNode == null)
            {
                return false;
            }

            if (syntaxNode.GetType() != typeof(T))
                return TryGetParentSyntax(syntaxNode, out result);
            
            result = syntaxNode as T;
            return true;
        }
        catch
        {
            return false;
        }
    }
}
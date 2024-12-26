# Gravy.ConsoleString

Gravy.ConsoleString is a C# library for formatting and manipulating console output with rich text features. It provides utilities to set colors, text styles, and other formatting options for console strings.

## Features

- **Color Formatting**: Set foreground and background colors using ANSI colors.
- **Text Styles**: Apply styles like bold, italic, underline, blink, inverse, and strikethrough.
- **Immutable Formatting**: Create and manipulate immutable console format objects.
- **Tag Parsing**: Parse and apply formatting tags within strings.

## Installation

To install Gravy.ConsoleString, add the following package to your project:

```sh
dotnet add package Gravy.ConsoleString
```

## Usage

### Basic Example

```csharp
var format = new ConsoleFormat(AnsiColor.Red, AnsiColor.Black, FontWeight.Bold, FontStyle.Underline);
Console.WriteLine("This is a formatted string".ApplyFormat(format));
```

### Creating Console Formats

You can create `ConsoleFormat` objects with various formatting options.

```csharp
var defaultFormat = ConsoleFormat.Default;
var redText = new ConsoleFormat(AnsiColor.Red);
var boldText = new ConsoleFormat(FontWeight.Bold);
var italicText = new ConsoleFormat(FontStyle.Italic);
```

### Applying Formatting

You can apply formatting to strings using the `ApplyFormat` extension method.

```csharp
var format = new ConsoleFormat(AnsiColor.Green, AnsiColor.Black, FontWeight.Bold);
var formattedString = "Green bold text".ApplyFormat(format);
Console.WriteLine(formattedString);
```

### Combining Formats

You can combine multiple formatting options using the `With` methods.

```csharp
var format = ConsoleFormat.Default
    .WithForeground(AnsiColor.Blue)
    .WithBackground(AnsiColor.White)
    .WithBold()
    .WithUnderline();
Console.WriteLine("Blue bold underlined text on white background".ApplyFormat(format));
```

### Using ConsoleStrings

You can use a ConsoleString just like a normal `string` object.

```csharp
var firstString = new ConsoleString("First String");
var secondString = new ConsoleString("Second String");
var combinedString = firstString.WithForeground(AnsiColo.Blue) + secondString.WithForeground(AnsiColor.Red);
var substring = combinedString.Substring(10, 20);
Console.WriteLine(substring);
```

### Tag Parsing

Sure, here are more examples of using tags in `Gravy.ConsoleString`:

#### Applying Foreground and Background Colors
```csharp
var taggedString = "[F!Red]Red text[/F] and [G!Blue]blue background[/G]".ParseTags();
Console.WriteLine(taggedString.ToAnsiString());
```

#### Applying Text Styles
```csharp
var taggedString = "[B]Bold text[/B], [I]italic text[/I], and [U]underlined text[/U]".ParseTags();
Console.WriteLine(taggedString.ToAnsiString());
```

#### Combining Multiple Tags
```csharp
var taggedString = "[F!Green][B]Bold green text[/B][/F] and [G!Yellow][I]italic text with yellow background[/I][/G]".ParseTags();
Console.WriteLine(taggedString.ToAnsiString());
```

#### Using RGB Colors
```csharp
var taggedString = "[F#FF5733]Text with RGB foreground color[/F] and [G#33FF57]RGB background color[/G]".ParseTags();
Console.WriteLine(taggedString.ToAnsiString());
```

#### Handling Nested Tags
```csharp
var taggedString = "[B]Bold text with [I]italic inside[/I][/B] and [U]underlined text[/U]".ParseTags();
Console.WriteLine(taggedString.ToAnsiString());
```

These examples demonstrate how to use various tags to format console output using the `Gravy.ConsoleString` library.

## Learn More

See the tests for all possible usages of the library.

## License

This package is licensed under the MIT License. See the `LICENSE` file for more details.
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

ConsoleStrings can also be created using marked up strings with tags. These tags
are similar to HTML tags and can be used to apply formatting to text. Each 
tag has a start and end, and can adjust a variety of formatting options.

#### Possible Tags

- `[F{COLOR}][/F]`: Set the foreground to `{COLOR}`. *See how to choose a 
  color below.*
- `[G{COLOR}][/G]`: Set the background to `{COLOR}`. *See how to choose a 
  color below.*
- `[B][/B]`: Set the text to bold.
- `[I][/I]`: Set the text to italic.
- `[U][/U]`: Set the text to underline.
- `[K][/K]`: Set the text to blink.
- `[V][/V]`: Set the text to inverse.
- `[T][/T]`: Set the text to strikethrough.
- `[L][/L]`: Set the text to light.
- `[//]`: Reset all formatting. *Not available in strict mode.*

You can escape a tag by using a backslash before the opening bracket. e.g. 
`\[This is not a tag]`.

##### Colors

Colors can be selected via a variety of methods. The following are all valid:

- **!** followed by a dotnet `Color` name: `!Red`, `!Blue`, `!Green`, etc. 
  Any color in the `System.Drawing.Color` enumeration can be used.
- **#** followed by a hex color code: `#FF5733`, `#33FF57`, etc.
- **@** followed by an ANSI 16 theme color name: `@Black`, `@White`, `@Cyan`, 
  etc. The full list of ANSI colors can be found [here](https://en.wikipedia.org/wiki/ANSI_escape_code#Colors).
- **$** followed by an ANSI 256 theme color name: `$Blue1`, `$DarkSeaGreen2`,
  etc. The full list of ANSI 256 colors can be found at [/Gravy.
  Ansi/Ansi256Color.cs](../Gravy.Ansi/Ansi256Color.cs).

Examples of each of these are shown below.

- `[F!Red]Some Text[/F]`: Set the foreground to a `System.Drawing.Color`.
- `[G#FF5733]Some Text[/G]`: Set the background to a hex color.
- `[F@BrightYellow]Some Text[/F]`: Set the foreground to an ANSI 16 color.
- `[G$DarkSeaGreen2]Some Text[/G]`: Set the background to an ANSI 256 color.


#### Strict Mode

By default, the parser will operate in a relaxed mode. You can enable string 
mode by setting the `strict` parameter to `true`. This will cause the parser 
to report errors in your markup. The possible errors are:

- **DuplicateColorException**: Thrown when the same foreground or background 
  color is set twice on some text. e.g. `[F!Red]Some text [F!Red]and more[/F]
  [/F]`.
- **DuplicateWeightException**: Thrown when the same weight is set twice on 
  some text. e.g. `[B]Some text [B]and more[/B][/B]`.
- **DuplicateStyleException**: Thrown when the same style is set twice on some 
  text. e.g. `[I]Some text [I]and more[/I][/I]`.
- **UnmatchedStartTokenException**: Thrown when a start token is found without a 
  corresponding stop token. e.g. `[B]Some text[U]and more[/B]`.
- **UnmatchedStopTokenException**: Thrown when a stop token is found without a 
  corresponding start token. e.g. `[B]Some text[/B][/U]`.
- **ResetAllInStrictModeException**: Thrown when a reset all token is found in 
  strict mode. e.g. `[R]Some text[/R]`.

#### Other Errors

Some other errors that can be thrown are:

- **UnknownThemeColorException**: Thrown when a theme color is not found. e.g. 
  `[F@BrightAqua]Some text[/F]` because `BrightAqua` is not a valid ANSI 16 
  color.
- **InvalidHexColorException**: Thrown when a hex color is not valid. e.g. 
  `[G#FF573]Some text[/G]` because `#FF573` is not a valid hex color.
- **UnknownNamedColorException**: Thrown when a named color is not found. e.g. 
  `[F!BrightYellow]Some text[/F]` because `BrightYellow` is not a valid 
  `System.Drawing.Color` name.
- **UnknownTagException**: Thrown when a tag is not recognized. e.g. 
  `[X]Some text[/X]` because `X` is not a valid tag.
- **MissingCloseBracketException**: Thrown when a tag is not closed. e.g. 
  `[BSome text[/B]` because the closing bracket is missing.
- **UnexpectedEndOfStringException**: Thrown when the end of the string is 
  reached before all tags are closed. e.g. `[B]Some text[/B`.

#### Examples

```csharp

#### Applying Foreground and Background Colors
```csharp
var taggedString = "[F!Red]Red text[/F] and [G!Blue]blue background[/G]".ParseTags();
Console.WriteLine(taggedString.ToAnsiString());
```

##### Applying Text Styles
```csharp
var taggedString = "[B]Bold text[/B], [I]italic text[/I], and [U]underlined text[/U]".ParseTags();
Console.WriteLine(taggedString.ToAnsiString());
```

##### Combining Multiple Tags
```csharp
var taggedString = "[F!Green][B]Bold green text[/B][/F] and [G!Yellow][I]italic text with yellow background[/I][/G]".ParseTags();
Console.WriteLine(taggedString.ToAnsiString());
```

##### Using RGB Colors
```csharp
var taggedString = "[F#FF5733]Text with RGB foreground color[/F] and [G#33FF57]RGB background color[/G]".ParseTags();
Console.WriteLine(taggedString.ToAnsiString());
```

##### Handling Nested Tags
```csharp
var taggedString = "[B]Bold text with [I]italic inside[/I][/B] and [U]underlined text[/U]".ParseTags();
Console.WriteLine(taggedString.ToAnsiString());
```

## Learn More

See the tests for all possible usages of the library.

## License

This package is licensed under the MIT License. See the `LICENSE` file for more details.
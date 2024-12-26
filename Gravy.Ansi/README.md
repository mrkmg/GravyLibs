# Gravy.Ansi

Gravy.Ansi is a C# library for working with ANSI escape codes in a terminal. It provides a set of utilities to manipulate terminal output, including setting colors, cursor movements, and other terminal control sequences.

## Features

- **ANSI 16 Colors**: Support for standard 16 ANSI colors.
- **ANSI 256 Colors**: Support for extended 256 ANSI colors.
- **RGB Colors**: Support for true color (24-bit RGB).
- **Cursor Control**: Move the cursor, save and restore cursor position, and more.
- **Screen Control**: Clear the screen, clear lines, and resize the terminal window.
- **Text Attributes**: Set text attributes like bold, underline, and blink.

## Installation

To install Gravy.Ansi, add the following package to your project:

```sh
dotnet add package Gravy.Ansi
```

## Usage

### Basic Example

```csharp
using Gravy.Ansi;
using System;

class Program
{
    static void Main()
    {
        Console.Out.SetForegroundColor(Ansi16Color.Red);
        Console.Out.WriteLine("This text is red!");
        Console.Out.ResetState();
    }
}
```

### Setting Colors

You can set the foreground and background colors using ANSI 16 colors, ANSI 256 colors, or RGB colors.

```csharp
using Gravy.Ansi;
using System.Drawing;

class Program
{
    static void Main()
    {
        // Set foreground to bright blue using ANSI 16 color
        Console.Out.SetForegroundColor(Ansi16Color.BrightBlue);
        Console.Out.WriteLine("Bright blue text");

        // Set background to dark green using ANSI 256 color
        Console.Out.SetBackgroundColor(Ansi256Color.DarkGreen);
        Console.Out.WriteLine("Text with dark green background");

        // Set foreground to a specific RGB color
        Console.Out.SetForegroundColor(Color.FromArgb(255, 165, 0)); // Orange
        Console.Out.WriteLine("Orange text");

        // Reset to default colors
        Console.Out.ResetState();
    }
}
```

### Cursor Control

You can move the cursor, save and restore its position, and more.

```csharp
using Gravy.Ansi;

class Program
{
    static void Main()
    {
        // Move cursor to specific position
        Console.Out.SetCursorPosition(10, 5);
        Console.Out.WriteLine("Text at position (10, 5)");

        // Save cursor position
        Console.Out.SaveState();

        // Move cursor down and write text
        Console.Out.Down(2);
        Console.Out.WriteLine("Text two lines down");

        // Restore cursor position
        Console.Out.RestoreState();
        Console.Out.WriteLine("Text at saved position");
    }
}
```

### Screen Control

You can clear the screen, clear lines, and resize the terminal window.

```csharp
using Gravy.Ansi;

class Program
{
    static void Main()
    {
        // Clear the entire screen
        Console.Out.EraseScreen();

        // Clear from cursor to end of screen
        Console.Out.EraseScreenFromCursor();

        // Clear the current line
        Console.Out.EraseLine();

        // Resize the terminal window
        Console.Out.ResizeWindow(30, 100);
    }
}
```

### Note for Windows Users

To enable ANSI escape codes on Windows, you may need to enable virtual terminal processing. This can be done using the WindowsConsole.TryEnableVirtualTerminalProcessing method provided by the library.

```csharp
using Gravy.Ansi;

class Program
{
    static void Main()
    {
        if (WindowsConsole.TryEnableVirtualTerminalProcessing())
        {
            Console.WriteLine("Virtual terminal processing enabled.");
        }
        else
        {
            Console.WriteLine("Failed to enable virtual terminal processing.");
        }
    }
}
```

## License

This package is licensed under the MIT License. See the `LICENSE` file for more details.
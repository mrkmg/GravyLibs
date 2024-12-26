# MetaString

MetaString is a C# library designed to serve as a foundation for other libraries that need to store and manipulate meta-data on strings. It provides a robust and flexible way to handle strings with associated meta-data, enabling advanced string processing and manipulation.

## Features

- **Meta-Data Storage**: Store custom meta-data alongside string content.
- **String Manipulation**: Perform various string operations while preserving meta-data.
- **Splitting**: Split strings based on characters or substrings, with options to trim entries and remove empty entries.
- **Trimming**: Trim strings while maintaining meta-data integrity.
- **Range Operations**: Extract sub-ranges of strings with meta-data.
- **Equality and Comparison**: Compare strings with meta-data for equality.

## Installation

To install MetaString, add the following package to your project:

```sh
dotnet add package Gravy.MetaString
```

## Usage

### Example of Usage

Below is an example of how to use MetaString to int metadata on a string, 
then access the metadata and the string content.

```csharp
using Gravy.MetaString;

var metaString1 = new MetaString<int>("Hello", 123);
var metaString2 = new MetaString<int>("World", 456);
var combinedMetaString = metaString1 + ", " + metaString2 + "!";

// Get the whole string
Console.WriteLine(metaString.Text); // Output: Hello, World!

// Get the MetaData

// First MetaData Entry
Console.WriteLine(metaString.MetaEntries[0].Text); // Output: Hello
Console.WriteLine(metaString.MetaEntries[0].Data); // Output: 123
Console.WriteLine(metaString.MetaEntries[0].Offset); // Output: 0

// Second MetaData Entry
Console.WriteLine(metaString.MetaEntries[1].Text); // Output: World
Console.WriteLine(metaString.MetaEntries[1].Data); // Output: 456
Console.WriteLine(metaString.MetaEntries[1].Offset); // Output: 7
```

MetaStrings can be used like normal strings.

```csharp

var metaString = new MetaString<int>("Hello", 123);
var newMetaString = metaString + ", World!";
Console.WriteLine(newMetaString); // Output: Hello, World!

var subString = newMetaString.Substring(7, 5);
Console.WriteLine(subString); // Output: World

// Most string operations are supported.
```

## License

This package is licensed under the MIT License. See the `LICENSE` file for more details.
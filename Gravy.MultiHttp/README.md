# Gravy.MultiHttp

Gravy.MultiHttp is a C# library designed for efficient and reliable multi-threaded HTTP downloading. It provides a robust framework for downloading large files by splitting them into chunks and downloading them concurrently.

## Features

- **Multi-threaded Downloads**: Split files into chunks and download them concurrently.
- **Resumable Downloads**: Resume downloads from where they left off in case of interruptions.
- **Progress Tracking**: Track the progress of each chunk and the overall download.
- **Error Handling**: Handle errors gracefully and retry failed chunks.
- **Customizable**: Define custom download strategies and chunk sizes.

## Installation

To install Gravy.MultiHttp, add the following package to your project:

```sh
dotnet add package Gravy.MultiHttp
```

## Usage

### Basic Example

This example demonstrates how to create a simple download using `DownloadBuilder`.

```csharp
using Gravy.MultiHttp;
using Gravy.MultiHttp.Interfaces;

var downloader = DownloadBuilder.Create()
    .AddDownload("http://example.com/file.zip", "/some/path/file.zip")
    .Build();

await downloader.StartAsync();
```

### Customizing Chunk Size, Concurrency, and File Writing

This example shows how to customize the chunk size, the maximum number of 
concurrent downloads, and the file writing strategy.

```csharp
using Gravy.MultiHttp;
using Gravy.MultiHttp.Interfaces;

var downloader = DownloadBuilder.Create()
    .AddDownload("http://example.com/file.zip", overwrite: true)
    .WithMaxChunkSize(1024 * 1024) // Set chunk size to 1 MB
    .WithMaxConcurrency(8) // Set maximum concurrency to 8
    .WithFileDestinationType(FileWriterType.InMemory) // Store download in memory before writing to disk
    .Build();

await downloader.StartAsync();
```

### Adding Event Handlers

This example demonstrates how to add event handlers for tracking progress and handling errors.

```csharp
using Gravy.MultiHttp;
using Gravy.MultiHttp.Interfaces;

var downloader = DownloadBuilder.Create()
    .AddDownload("http://example.com/file.zip", overwrite: true)
    .OnStarted(() => Console.WriteLine("Download started"))
    .OnProgress(progress => Console.WriteLine($"Downloaded {progress.BytesDownloaded} of {progress.TotalBytes} bytes"))
    .OnEnded(success => Console.WriteLine(success ? "Download completed" : "Download failed"))
    .OnError(ex => Console.WriteLine($"Error: {ex.Message}"))
    .Build();

await downloader.StartAsync();
```

### Downloading Multiple Files

This example shows how to add multiple downloads to the `DownloadBuilder`.

```csharp
using Gravy.MultiHttp;
using Gravy.MultiHttp.Interfaces;

var downloader = DownloadBuilder.Create()
    .AddDownload("http://example.com/file1.zip", overwrite: true)
    .AddDownload("http://example.com/file2.zip", overwrite: true)
    .Build();

await downloader.StartAsync();
```

### Using a Custom HttpClient Configuration

This example demonstrates how to configure the `HttpClient` used by the downloader.

```csharp
using Gravy.MultiHttp;
using Gravy.MultiHttp.Interfaces;

var downloader = DownloadBuilder.Create()
    .AddDownload("http://example.com/file.zip", overwrite: true)
    .ConfigureHttpClient(client => client.Timeout = TimeSpan.FromMinutes(10))
    .Build();

await downloader.StartAsync();
```

### Setting a Destination Path

This example shows how to set a destination path for the downloads.

```csharp
using Gravy.MultiHttp;
using Gravy.MultiHttp.Interfaces;

var downloader = DownloadBuilder.Create("C:\\Downloads")
    .AddDownload("http://example.com/file.zip", overwrite: true)
    .Build();

await downloader.StartAsync();
```

### Handling File-Specific Events

This example demonstrates how to handle events specific to individual files.

```csharp
using Gravy.MultiHttp;
using Gravy.MultiHttp.Interfaces;

var downloader = DownloadBuilder.Create()
    .AddDownload("http://example.com/file.zip", overwrite: true)
    .OnFileStarted(file => Console.WriteLine($"File download started: {file.FileName}"))
    .OnFileProgress(progress => Console.WriteLine($"File progress: {progress.BytesDownloaded} of {progress.TotalBytes} bytes"))
    .OnFileEnded(file => Console.WriteLine($"File download completed: {file.FileName}"))
    .OnFileError(fe => Console.WriteLine($"Error downloading file {fe.File.FileName}: {fe.Exception.Message}"))
    .Build();

await downloader.StartAsync();
```

## License

This package is licensed under the MIT License. See the `LICENSE` file for more details.
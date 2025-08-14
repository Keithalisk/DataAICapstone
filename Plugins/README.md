# Semantic Kernel Plugins

This directory contains Semantic Kernel plugins that provide external tools and functions for your AI agents.

## Available Plugins

### MathPlugin
Provides mathematical calculation functions:
- `Add` - Add two numbers
- `Subtract` - Subtract numbers  
- `Multiply` - Multiply numbers
- `Divide` - Divide numbers
- `SquareRoot` - Calculate square root
- `Power` - Calculate power/exponent
- `Absolute` - Get absolute value

### TextPlugin
Provides text processing functions:
- `ToUpperCase` - Convert to uppercase
- `ToLowerCase` - Convert to lowercase
- `ToTitleCase` - Convert to title case
- `CountCharacters` - Count characters
- `CountWords` - Count words
- `ReverseText` - Reverse text
- `ReplaceText` - Replace text
- `ExtractSubstring` - Extract substring
- `SplitText` - Split text by delimiter
- `JoinText` - Join text with delimiter
- `TrimText` - Trim whitespace
- `ContainsText` - Check if text contains substring

### TimePlugin
Provides date and time functions:
- `GetCurrentDateTime` - Get current date/time
- `GetCurrentDate` - Get current date
- `GetCurrentTime` - Get current time
- `GetCurrentUtcDateTime` - Get UTC date/time
- `AddDaysToDate` - Add days to a date
- `GetDaysBetweenDates` - Calculate days between dates
- `GetDayOfWeek` - Get day of week
- `FormatDateTime` - Format date/time

### FilePlugin
Provides safe file system operations:
- `ReadFileAsync` - Read file contents
- `WriteFileAsync` - Write to file
- `AppendToFileAsync` - Append to file
- `ListFiles` - List files in directory
- `FileExists` - Check if file exists
- `DeleteFile` - Delete a file
- `GetFileInfo` - Get file information

## How to Use

These plugins are automatically registered with the Semantic Kernel in `Program.cs`. Your agents can use these functions by calling them through the kernel.

## Adding New Plugins

To add a new plugin:

1. Create a new class in this directory
2. Add methods decorated with `[KernelFunction]` attribute
3. Use `[Description]` attributes to describe functions and parameters
4. Register the plugin in `Program.cs` using `kernelBuilder.Plugins.AddFromType<YourPlugin>()`

Example:
```csharp
public class MyPlugin
{
    [KernelFunction]
    [Description("Does something useful")]
    public string DoSomething([Description("Input parameter")] string input)
    {
        return $"Processed: {input}";
    }
}
```

## Security Notes

- FilePlugin operations are restricted to a safe working directory
- All file paths are validated to prevent directory traversal attacks
- Consider adding appropriate validation and error handling for your custom plugins

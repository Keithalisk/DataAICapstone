using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace DataAICapstone.Plugins
{
    /// <summary>
    /// File operations plugin for Semantic Kernel that provides safe file system access
    /// </summary>
    public class FilePlugin
    {
        private readonly string _workingDirectory;

        public FilePlugin(IConfiguration configuration)
        {
            _workingDirectory = configuration.GetValue<string>("FileOperations:WorkingDirectory") 
                ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "AIAssistant");
            
            // Ensure working directory exists
            Directory.CreateDirectory(_workingDirectory);
        }

        [KernelFunction]
        [Description("Read the contents of a text file")]
        public async Task<string> ReadFileAsync([Description("The name of the file to read")] string fileName)
        {
            var filePath = GetSafeFilePath(fileName);
            if (filePath == null)
                throw new ArgumentException("Invalid file name");

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File '{fileName}' not found");

            return await File.ReadAllTextAsync(filePath);
        }

        [KernelFunction]
        [Description("Write content to a text file")]
        public async Task WriteFileAsync(
            [Description("The name of the file to write")] string fileName,
            [Description("The content to write to the file")] string content)
        {
            var filePath = GetSafeFilePath(fileName);
            if (filePath == null)
                throw new ArgumentException("Invalid file name");

            await File.WriteAllTextAsync(filePath, content);
        }

        [KernelFunction]
        [Description("Append content to a text file")]
        public async Task AppendToFileAsync(
            [Description("The name of the file to append to")] string fileName,
            [Description("The content to append to the file")] string content)
        {
            var filePath = GetSafeFilePath(fileName);
            if (filePath == null)
                throw new ArgumentException("Invalid file name");

            await File.AppendAllTextAsync(filePath, content);
        }

        [KernelFunction]
        [Description("List all files in the working directory")]
        public string[] ListFiles([Description("File pattern to match (e.g., *.txt)")] string pattern = "*")
        {
            try
            {
                return Directory.GetFiles(_workingDirectory, pattern)
                    .Select(Path.GetFileName)
                    .Where(name => !string.IsNullOrEmpty(name))
                    .ToArray()!;
            }
            catch
            {
                return Array.Empty<string>();
            }
        }

        [KernelFunction]
        [Description("Check if a file exists")]
        public bool FileExists([Description("The name of the file to check")] string fileName)
        {
            var filePath = GetSafeFilePath(fileName);
            return filePath != null && File.Exists(filePath);
        }

        [KernelFunction]
        [Description("Delete a file")]
        public bool DeleteFile([Description("The name of the file to delete")] string fileName)
        {
            var filePath = GetSafeFilePath(fileName);
            if (filePath == null || !File.Exists(filePath))
                return false;

            try
            {
                File.Delete(filePath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        [KernelFunction]
        [Description("Get file information including size and last modified date")]
        public string GetFileInfo([Description("The name of the file")] string fileName)
        {
            var filePath = GetSafeFilePath(fileName);
            if (filePath == null || !File.Exists(filePath))
                return "File not found";

            try
            {
                var fileInfo = new FileInfo(filePath);
                return $"File: {fileName}\nSize: {fileInfo.Length} bytes\nLast Modified: {fileInfo.LastWriteTime:yyyy-MM-dd HH:mm:ss}";
            }
            catch (Exception ex)
            {
                return $"Error getting file info: {ex.Message}";
            }
        }

        private string? GetSafeFilePath(string fileName)
        {
            // Prevent directory traversal attacks
            if (string.IsNullOrWhiteSpace(fileName) || 
                fileName.Contains("..") || 
                Path.IsPathRooted(fileName))
            {
                return null;
            }

            try
            {
                return Path.Combine(_workingDirectory, fileName);
            }
            catch
            {
                return null;
            }
        }
    }
}

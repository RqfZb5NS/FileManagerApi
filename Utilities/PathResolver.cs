using System.IO;

namespace FileManagerApi.Utilities;

public class PathResolver
{
    private readonly string _rootPath;

    public PathResolver(string rootPath)
    {
        _rootPath = Path.GetFullPath(rootPath);
        Directory.CreateDirectory(_rootPath);
    }

    public string ResolvePath(string relativePath)
    {
        var fullPath = Path.GetFullPath(Path.Combine(_rootPath, relativePath));
        
        if (!fullPath.StartsWith(_rootPath))
            throw new InvalidOperationException("Invalid path");
        
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        return fullPath;
    }
}
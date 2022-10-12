namespace DirectoryScanner.Core.Model.Impl;

public class FileNode : IFileSystemComponent
{
    public string FullPath { get; }
    
    public long Size { get; set; }
    public string Name { get; set; }
    public string RelativeSize { get; set;  }

    public FileNode(string fullPath, string name, long size)
    {
        FullPath = fullPath;
        Name = name;
        Size = size;
    }

    public void SpecifySize()
    {
        Console.WriteLine($"File: {FullPath} {Size}");
    }

    public void InitRelativeSize(long parentSize)
    {
        var relativeSize = parentSize != 0 ? Size / (decimal) parentSize * 100 : 0;
        RelativeSize = $"{relativeSize:0.00}%";
    }
}
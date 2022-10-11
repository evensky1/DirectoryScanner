namespace DirectoryScanner.Core.Model.Impl;

public class FileNode : IFileSystemComponent
{
    public string FullPath { get; }
    
    public long Size { get; set; }
    public string Name { get; set; }
    public decimal RelativeSize { get; set;  }

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

    public void InitRelativeSize(IFileSystemComponent parentComponent)
    {
        RelativeSize = Size / parentComponent.Size * 100;
    }
}
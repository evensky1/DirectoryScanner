using DirectoryScanner.Core.Model;

namespace DirectoryScanner.Core.Model.Impl;

public class DirectoryNode : IFileSystemComponent
{
    public List<IFileSystemComponent> ChildComponents { get; }
    
    public string FullPath { get; }
    public long Size { get; set; }
    public string Name { get; set; }
    public decimal RelativeSize { get; }

    public DirectoryNode(string fullPath, string name)
    {
        ChildComponents = new List<IFileSystemComponent>();
        Name = name;
        FullPath = fullPath;
    }

    public void Add(IFileSystemComponent c)
    {
        ChildComponents.Add(c);
    }

    public void IncSize(long val)
    {
        Size += val;
    }
    
    public void ExecuteInitialization()
    {
        Console.WriteLine($"Dir: {FullPath} {Size}");
        ChildComponents.ForEach(c => c.ExecuteInitialization());
    }
}
namespace DirectoryScanner.Core.Model;

public interface IFileSystemComponent
{
    public long Size { get; }
    public string RelativeSize { get; }
    public string Name { get; }
    void SpecifySize();
    void InitRelativeSize(long parentSize);
}
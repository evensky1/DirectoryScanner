namespace DirectoryScanner.Core.Model;

public interface IFileSystemComponent
{
    public long Size { get; }
    public string RelativeSize { get; set; }
    void SpecifySize();
    void InitRelativeSize(long parentSize);
}
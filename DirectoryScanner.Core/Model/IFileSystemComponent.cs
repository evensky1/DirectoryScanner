namespace DirectoryScanner.Core.Model;

public interface IFileSystemComponent
{
    public long Size { get; set; }
    public decimal RelativeSize { get; }
    void SpecifySize();

    void InitRelativeSize(IFileSystemComponent parentComponent);
}
namespace DirectoryScanner.Core.Model;

public interface IFileSystemComponent
{
    public long Size { get; set; }
    public decimal RelativeSize { get; }
    void ExecuteInitialization();
}
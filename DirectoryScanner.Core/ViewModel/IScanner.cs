using DirectoryScanner.Core.Model;

namespace DirectoryScanner.Core.ViewModel;

public interface IScanner
{
    IFileSystemComponent StartScan();
}
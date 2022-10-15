using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using DirectoryScanner.Core.Model;
using DirectoryScanner.Core.ViewModel.Command;
using DirectoryScanner.Core.ViewModel.ScannerImpl;

namespace DirectoryScanner.Core.ViewModel;

public class DirectoryScannerVM : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private ObservableCollection<IFileSystemComponent> _root = new();
    public ObservableCollection<IFileSystemComponent> Root
    {
        get { return _root; }
        set
        {
            _root = value;
            OnPropertyChanged("Root");
        }
    }
    private CancellationTokenSource _ctSource;
    
    private CommonCommand _executeScan;
    public CommonCommand ExecuteScan
    {
        get { return _executeScan ?? new CommonCommand(obj => RunScanner()); }
    }

    private CommonCommand _cancelOperation;
    public CommonCommand CancelOperation
    {
        get { return _cancelOperation ?? new CommonCommand(obj => _ctSource.Cancel()); }
    }
    
    private void RunScanner()
    {
        var fbd = new FolderBrowserForWPF.Dialog();
        if (fbd.ShowDialog().GetValueOrDefault())
        {
            _ctSource = new CancellationTokenSource();
            var scanner = new Scanner(fbd.FileName, 12, _ctSource.Token);
            Root.Clear();
            Root.Add(scanner.StartScan());
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
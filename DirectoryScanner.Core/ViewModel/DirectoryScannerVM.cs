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
    
    private RelayCommand _showDialog;
    public RelayCommand ShowDialog
    {
        get { return _showDialog ?? new RelayCommand(obj => RunScanner()); }
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

    // protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    // {
    //     if (EqualityComparer<T>.Default.Equals(field, value)) return false;
    //     field = value;
    //     OnPropertyChanged(propertyName);
    //     return true;
    // }
}
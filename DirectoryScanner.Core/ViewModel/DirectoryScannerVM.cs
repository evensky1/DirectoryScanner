﻿using System.Collections.ObjectModel;
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
    
    private RelayCommand _executeScan;
    public RelayCommand ExecuteScan
    {
        get { return _executeScan ?? new RelayCommand(obj => RunScanner()); }
    }

    private RelayCommand _cancelOperation;
    public RelayCommand CancelOperation
    {
        get { return _cancelOperation ?? new RelayCommand(obj => _ctSource.Cancel()); }
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
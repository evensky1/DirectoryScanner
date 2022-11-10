using System.Collections.Concurrent;
using System.Diagnostics;
using System.Security;
using DirectoryScanner.Core.Model;
using DirectoryScanner.Core.Model.Impl;

namespace DirectoryScanner.Core.ViewModel.ScannerImpl;

public class Scanner : IScanner
{
    private readonly Semaphore _semaphore;
    private readonly ConcurrentQueue<Task> _tasks;
    private readonly CancellationToken _cancellationToken;
    private readonly DirectoryNode _initDir;

    public Scanner(string initDirPath, int maxThreadCount, CancellationToken cancellationToken)
    {
        _semaphore = new Semaphore(maxThreadCount, maxThreadCount);
        _tasks = new ConcurrentQueue<Task>();
        _initDir = new DirectoryNode(initDirPath, Path.GetFileName(initDirPath));
        _cancellationToken = cancellationToken;
    }

    public IFileSystemComponent StartScan()
    {
        try
        {
            _tasks.Enqueue(Task.Run(() => ScanDir(_initDir), _cancellationToken));
        
            while (_tasks.TryDequeue(out var currentTask) && !_cancellationToken.IsCancellationRequested)
            {
                currentTask.Wait(_cancellationToken);
            }
        }
        catch (Exception e)
        {
            _tasks.Clear();
        }
        
        _initDir.SpecifySize();
        _initDir.InitRelativeSize(_initDir.Size);
        return _initDir;
    }

    private void ScanDir(DirectoryNode currentDirNode)
    {
        _semaphore.WaitOne();

        try
        {
            var currentDirInfo = new DirectoryInfo(currentDirNode.FullPath);
            
            foreach (var dirInfo in currentDirInfo.EnumerateDirectories())
            {
                if (_cancellationToken.IsCancellationRequested) return;

                var childDirNode = new DirectoryNode(dirInfo.FullName, dirInfo.Name);

                _tasks.Enqueue(Task.Run(() => ScanDir(childDirNode), _cancellationToken));

                currentDirNode.Add(childDirNode);
            }

            foreach (var fileInfo in currentDirInfo.EnumerateFiles())
            {
                if (_cancellationToken.IsCancellationRequested) return;

                var fileNode = new FileNode(fileInfo.FullName, fileInfo.Name, fileInfo.Length);

                currentDirNode.Add(fileNode);

                currentDirNode.IncSize(fileNode.Size);
            }
        }
        catch (Exception e)
        {
            // ignored
        }

        _semaphore.Release();
    }
}
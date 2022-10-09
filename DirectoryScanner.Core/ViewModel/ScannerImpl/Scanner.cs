using System.Collections.Concurrent;
using DirectoryScanner.Core.Model;
using DirectoryScanner.Core.Model.Impl;

namespace DirectoryScanner.Core.ViewModel.ScannerImpl;

public class Scanner : IScanner
{
    private readonly Semaphore _semaphore;
    private readonly ConcurrentQueue<DirectoryNode> _dirsQueue;
    private readonly List<Task> _tasks;
    private readonly CancellationToken _cancellationToken;
    private readonly DirectoryNode _initDir;

    public Scanner(string initDirPath, int maxThreadCount, CancellationToken cancellationToken)
    {
        _semaphore = new Semaphore(maxThreadCount, maxThreadCount);
        _dirsQueue = new ConcurrentQueue<DirectoryNode>();
        _tasks = new List<Task>();
        _initDir = new DirectoryNode(initDirPath, Path.GetFileName(initDirPath));
        _dirsQueue.Enqueue(_initDir);
        _cancellationToken = cancellationToken;
    }

    public IFileSystemComponent StartScan()
    {
        while (_dirsQueue.Any() || _tasks.Any())
        {
            if (_cancellationToken.IsCancellationRequested) break;

            if (_dirsQueue.TryDequeue(out var currentDirNode))
                _tasks.Add(Task.Run(() => ScanDir(currentDirNode), _cancellationToken));
            
            _tasks.RemoveAll(t => t.IsCompleted);
        }

        return _initDir;
    }

    private void ScanDir(DirectoryNode currentDirNode)
    {
        _semaphore.WaitOne();

        var currentDirInfo = new DirectoryInfo(currentDirNode.FullPath);
        //mb i should implement additional list with dirs in current task
        //so it would not complete untill all sub dirs gonna be completed
        //or push it into monitor or smth like that
        foreach (var dirInfo in currentDirInfo.EnumerateDirectories())
        {
            if (_cancellationToken.IsCancellationRequested) return;

            var childDirNode = new DirectoryNode(dirInfo.FullName, dirInfo.Name);

            _dirsQueue.Enqueue(childDirNode);

            currentDirNode.Add(childDirNode);
        }

        foreach (var fileInfo in currentDirInfo.EnumerateFiles())
        {
            if (_cancellationToken.IsCancellationRequested) return;

            var fileNode = new FileNode(fileInfo.FullName, fileInfo.Name, fileInfo.Length);

            currentDirNode.Add(fileNode);
            
            currentDirNode.IncSize(fileNode.Size);
        }
        
        _semaphore.Release();
    }
}
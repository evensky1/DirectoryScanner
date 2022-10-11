﻿using DirectoryScanner.Core.Model;

namespace DirectoryScanner.Core.Model.Impl;

public class DirectoryNode : IFileSystemComponent
{
    public List<IFileSystemComponent> ChildComponents { get; }
    public string FullPath { get; }
    public long Size { get; set; }
    public string Name { get; set; }
    public decimal RelativeSize { get; set; }
    private bool _isRecounted;

    public DirectoryNode(string fullPath, string name)
    {
        ChildComponents = new List<IFileSystemComponent>();
        Name = name;
        FullPath = fullPath;
        _isRecounted = false;
    }

    public void Add(IFileSystemComponent c)
    {
        ChildComponents.Add(c);
    }

    public void IncSize(long val)
    {
        Size += val;
    }

    public void SpecifySize()
    {
        if (_isRecounted) return;
        var dirs =
            ChildComponents.FindAll(c => c.GetType() == typeof(DirectoryNode));
        dirs.ForEach(c => c.SpecifySize());
        dirs.ForEach(c => IncSize(c.Size));
        _isRecounted = true;
    }

    public void InitRelativeSize(IFileSystemComponent parentComponent)
    {
        ChildComponents.ForEach(c => InitRelativeSize(this));
        RelativeSize = Size / parentComponent.Size * 100;
    }
}
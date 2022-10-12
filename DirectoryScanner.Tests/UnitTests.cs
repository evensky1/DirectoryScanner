using DirectoryScanner.Core.Model.Impl;
using DirectoryScanner.Core.ViewModel;
using DirectoryScanner.Core.ViewModel.ScannerImpl;

namespace DirectoryScanner.Tests;

public class Tests
{
    private const string RootDirPath = "C:\\Users\\fromt\\RiderProjects\\DirectoryScanner\\DirectoryScanner.Tests\\RootTestDir";

    [SetUp]
    public void Setup()
    {
        
    }

    [Test]
    public void First_Layer_Child_Count_Default_Scenario()
    {
        var scanner = new Scanner(RootDirPath, 10, new CancellationToken());
        var result = (DirectoryNode) scanner.StartScan();
        Assert.That(result.ChildComponents, Has.Count.EqualTo(9));
    }
    
    [Test]
    public void First_Layer_Child_Count_Single_Thread()
    {
        var scanner = new Scanner(RootDirPath, 1, new CancellationToken());
        var result = (DirectoryNode) scanner.StartScan();
        Assert.That(result.ChildComponents, Has.Count.EqualTo(9));
    }
    
    [Test]
    public void Root_Size_Default_Scenario()
    {
        var scanner = new Scanner(RootDirPath, 10, new CancellationToken());
        var result = scanner.StartScan();
        Assert.That(result.Size, Is.EqualTo(2290));
    }

    [Test]
    public void Relative_Size_Default_Scenario()
    {
        var scanner = new Scanner(RootDirPath, 10, new CancellationToken());
        var result = (DirectoryNode) scanner.StartScan();
        var testFile = result.ChildComponents.Find(c => c.Name.Equals("TestFile1.txt")); 
        Assert.That(testFile?.RelativeSize, Is.EqualTo("10,09%"));
    }
    
    [Test]
    public void Cancellation_Token_Was_Activated()
    {
        var cancellationSource = new CancellationTokenSource();
        var scanner = new Scanner(RootDirPath, 10, cancellationSource.Token);
        
        var task = Task<DirectoryNode>.Factory.StartNew(() => (DirectoryNode)scanner.StartScan());
        for (var i = 0; i < 17500; i++)
        {
            Console.WriteLine("My cool thread sleep");
        }
        cancellationSource.Cancel();
        var result = task.Result;
        Assert.That(result.ChildComponents, Has.Count.Not.EqualTo(9));
        Assert.That(result.ChildComponents, Has.Count.GreaterThan(0));
    }
}
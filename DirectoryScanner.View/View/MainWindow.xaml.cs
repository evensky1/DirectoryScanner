using System.Windows;
using DirectoryScanner.Core.ViewModel;

namespace DirectoryScanner.View
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new DirectoryScannerVM();
        }
    }
}
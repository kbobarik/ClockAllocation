using Avalonia.Controls;
using clock_allocation.ViewModels;

namespace clock_allocation.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }
}
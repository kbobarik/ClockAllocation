using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using clock_allocation.Models;
using clock_allocation.ViewModels;

namespace clock_allocation.Views;

public partial class CertainCurriculumView : UserControl
{
    public CertainCurriculumView(Curriculum Curriculum)
    {
        InitializeComponent();
        DataContext = new CertainCurriculumViewModel(Curriculum);
    }
}
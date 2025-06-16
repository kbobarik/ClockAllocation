using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using clock_allocation.Models;
using clock_allocation.ViewModels;

namespace clock_allocation.Views;

public partial class UpdateTeacherLoadingView : UserControl
{
    public UpdateTeacherLoadingView(List<TeachingLoad> teachers)
    {
        InitializeComponent();
        DataContext = new UpdateTeachingLoadViewModel(teachers);
    }
}
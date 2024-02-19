using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace MusicCreator.UI;

public partial class MusicExplorer : UserControl
{
    private Action showDashboardAction;
    public MusicExplorer(Action showDashboard)
    {
        InitializeComponent();
        showDashboardAction = showDashboard;
    }
    
    private void ShowDashboard(object sender, RoutedEventArgs args)
    {
        showDashboardAction();
    }
}
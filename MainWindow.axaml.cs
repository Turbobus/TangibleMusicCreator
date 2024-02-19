using System;
using Avalonia.Controls;
using MusicCreator.UI;


namespace MusicCreator;

public partial class MainWindow : Window
{
    private Dashboard dashboard;
    private MusicExplorer musicExplorer;
    public MainWindow()
    {
        InitializeComponent();
        dashboard = new Dashboard(ShowMusicExplorer);
        musicExplorer = new MusicExplorer(ShowDashboard);
        Content = dashboard;
    }

    private void ShowMusicExplorer()
    {
        Content = musicExplorer;
    }

    private void ShowDashboard()
    {
        Content = dashboard;
    }
}
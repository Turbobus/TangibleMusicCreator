using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace MusicCreator.UI;

public partial class MusicExplorer : UserControl
{
    private Action showDashboardAction;
    private string selectedSong;
    public MusicExplorer(Action showDashboard)
    {
        InitializeComponent();
        showDashboardAction = showDashboard;
        DataContext = new MusicExplorerViewModel();
    }
    
    private void ShowDashboard(object sender, RoutedEventArgs args)
    {
        showDashboardAction();
    }

    private void PlaySound(object sender, RoutedEventArgs args)
    {
        _stopSound();
        var tag = ((Button)sender).Tag?.ToString() ?? "";
        PlaySound(tag);
    }
    
    private void KillSound(object sender, RoutedEventArgs args)
    {
       _stopSound();
    }

    private void SelectSound(object sender, RoutedEventArgs args)
    {
        var tag = ((Button)sender).Tag?.ToString() ?? "";
        selectedSong = tag;
        SoundSelection.Text = tag;
    }

    private void UploadSound(object sender, RoutedEventArgs args)
    {
        Console.WriteLine("Upload Sound");   
    }
    
    private void ScanTag(object sender, RoutedEventArgs args)
    {
        Console.WriteLine("Scan Tag");
    }
    
    private void PlaySound(string fileName)
    {
        var workingDirectory = Environment.CurrentDirectory;
        var basePath = Directory.GetParent(workingDirectory)?.Parent?.Parent?.FullName ?? "";
        var scriptPath = basePath + "/Scripts/playSound.sh";
        var musicPath = basePath + $"/Sounds/{fileName}";
        var startInfo = new ProcessStartInfo() { FileName = scriptPath, Arguments = musicPath };
        var proc = new Process() { StartInfo = startInfo };
        proc.Start();
    }
    
    private void _stopSound()
    {
        var startInfo = new ProcessStartInfo()
        {
            FileName = "/bin/bash",
            Arguments = $"-c \"ps -A | grep \"afplay\" | awk '{{print $1}}' | xargs kill -9\"",
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        var proc = new Process() { StartInfo = startInfo };
        proc.Start();
        proc.WaitForExit();
    }
}
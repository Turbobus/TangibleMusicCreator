using System;
using System.Diagnostics;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MusicCreator.ReaderCode;

namespace MusicCreator.UI;

public partial class Dashboard : UserControl
{
    private SimpleSerialRead _simpleSerialRead = new SimpleSerialRead();
    private TextWriter originalConsoleOut;
    private readonly Action showMusicExplorerAction;
    public Dashboard(Action showMusicExplorer)
    {
        InitializeComponent();
        //PlaySound();
        //Sound.SetupSound();

        showMusicExplorerAction = showMusicExplorer;
        // Redirect the Console.Out to a custom TextWriter
        originalConsoleOut = Console.Out;
        Console.SetOut(new MultiTextWriter(originalConsoleOut, new MyTextBoxWriter(outputTextBox)));
        try
        {
            _simpleSerialRead.StartSerialPortProgram("COM 4");
        }
        catch (Exception e)
        {
            Console.WriteLine("-ERROR- " + e.Message);
        }
    }

    public void PortButtonClicked(object source, RoutedEventArgs args)
    {
        outputTextBox.Text = "";
        _simpleSerialRead.StartSerialPortProgram(PortName.Text ?? "COM 4");
    }
    
    private void StartScan(object sender, RoutedEventArgs args)
    {
        _simpleSerialRead.StartScan();
    }

    private void PlaySound()
    {
        var workingDirectory = Environment.CurrentDirectory;
        var basePath = Directory.GetParent(workingDirectory)?.Parent?.Parent?.FullName ?? "";
        var scriptPath = basePath + "/Scripts/playSound.sh";
        var musicPath = basePath + "/Sounds/1.mp3";
        var startInfo = new ProcessStartInfo() { FileName = scriptPath, Arguments = musicPath };
        var proc = new Process() { StartInfo = startInfo };
        proc.Start();
    }

    private void ShowSoundLibrary(object sender, RoutedEventArgs args)
    {
        showMusicExplorerAction();
    }

    private void PlaySampleSound(object sender, RoutedEventArgs args)
    {
        
    }
}
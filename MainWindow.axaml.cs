using System;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MusicCreator.ReaderCode;
using MusicCreator.UI;

namespace MusicCreator;

public partial class MainWindow : Window
{
    private SimpleSerialRead _simpleSerialRead = new SimpleSerialRead();
    private ReadSerialMusic readSerialMusic = new ReadSerialMusic();
    private TextWriter originalConsoleOut;

    public MainWindow()
    {
        InitializeComponent();
        
        Sound.SetupSound();

        // Redirect the Console.Out to a custom TextWriter
        originalConsoleOut = Console.Out;
        Console.SetOut(new MultiTextWriter(originalConsoleOut, new MyTextBoxWriter(outputTextBox)));
        try
        {
            //_simpleSerialRead.StartSerialPortProgram("COM 3");
            
            readSerialMusic.StartSerialMusicProgram("COM 3");
        }
        catch (Exception e)
        {
            Console.WriteLine("-ERROR- " + e.Message);
        }
    }

    public void PortButtonClicked(object source, RoutedEventArgs args)
    {
        outputTextBox.Text = "";
        //_simpleSerialRead.StartSerialPortProgram(PortName.Text ?? "COM 3");
        readSerialMusic.StartSerialMusicProgram(PortName.Text ?? "COM 3");
    }
    
    private void StartScan(object sender, RoutedEventArgs args)
    {
        //_simpleSerialRead.StartScan();
        readSerialMusic.DecodeCommand("start");
    }
    
    private void EndScan(object sender, RoutedEventArgs args)
    {
        //_simpleSerialRead.StartScan();
        readSerialMusic.DecodeCommand("end");
    }

    private void PlaySampleSound(object sender, RoutedEventArgs args)
    {
        readSerialMusic.DecodeCommand("play");
    }
}
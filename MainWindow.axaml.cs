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
            _simpleSerialRead.StartSerialPortProgram("COM 3");
        }
        catch (Exception e)
        {
            Console.WriteLine("-ERROR- " + e.Message);
        }
    }

    public void PortButtonClicked(object source, RoutedEventArgs args)
    {
        outputTextBox.Text = "";
        _simpleSerialRead.StartSerialPortProgram(PortName.Text ?? "COM 3");
    }
    
    private void StartScan(object sender, RoutedEventArgs args)
    {
        _simpleSerialRead.StartScan();
    }

    private void PlaySampleSound(object sender, RoutedEventArgs args)
    {
    }
}
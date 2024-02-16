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
            _simpleSerialRead.StartSerialPortProgram("COM 4");
        }
        catch (Exception e)
        {
            Console.WriteLine("-ERROR- " + e.Message);
        }
    }

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selection = PortComboBox?.SelectedItem is ComboBoxItem item ? item : null;
        if (selection != null)
        {
            outputTextBox.Text = "";
            _simpleSerialRead.StartSerialPortProgram(selection.Content?.ToString() ?? "COM 4");
        }
    }

    private void StartScan(object sender, RoutedEventArgs args)
    {
        _simpleSerialRead.StartScan();
    }

    private void PlaySampleSound(object sender, RoutedEventArgs args)
    {
    }
}
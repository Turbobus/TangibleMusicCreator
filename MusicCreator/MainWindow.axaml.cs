using System;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using MusicCreator.ReaderCode;

namespace MusicCreator;

public partial class MainWindow : Window
{
    private SimpleSerialRead _simpleSerialRead = new SimpleSerialRead();
    private TextWriter originalConsoleOut;

    public MainWindow()
    {
        InitializeComponent();
        originalConsoleOut = Console.Out;
        Sound.SetupSound();

        // Redirect the Console.Out to a custom TextWriter
        Console.SetOut(new MultiTextWriter(originalConsoleOut, new MyTextBoxWriter(outputTextBox)));
        Console.WriteLine(SerialPort.GetPortNames());
        try
        {
            _simpleSerialRead.SerialPortProgram();
        }
        catch (Exception e)
        {
            Console.WriteLine("-ERROR- " + e.Message);
        }
    }

    private void StartScan(object sender, RoutedEventArgs args)
    {
        _simpleSerialRead.StartScan();
    }

    private void PlaySampleSound(object sender, RoutedEventArgs args)
    {
    }


// For console window to textbox
    public class MultiTextWriter : TextWriter
    {
        private TextWriter originalWriter;
        private TextWriter customWriter;

        public MultiTextWriter(TextWriter original, TextWriter custom)
        {
            originalWriter = original;
            customWriter = custom;
        }

        public override void Write(char value)
        {
            originalWriter.Write(value);
            customWriter.Write(value);
        }

        public override void Write(string value)
        {
            originalWriter.Write(value);
            customWriter.Write(value);
        }

        public override Encoding Encoding => originalWriter.Encoding;
    }

    public class MyTextBoxWriter : TextWriter
    {
        private TextBox textBox;

        public MyTextBoxWriter(TextBox outputTextBox)
        {
            textBox = outputTextBox;
        }

        public override void Write(char value)
        {
            // Update the UI with the character
            Dispatcher.UIThread.Post(() => textBox.Text += value.ToString());
        }

        public override void Write(string value)
        {
            // Update the UI with the string
            Dispatcher.UIThread.Post(() => textBox.Text += value);
        }

        public override Encoding Encoding => Encoding.UTF8; // Change the encoding as needed
    }
}
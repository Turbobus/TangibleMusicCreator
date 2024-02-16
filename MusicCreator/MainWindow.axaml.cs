using Avalonia.Controls;
using MusicCreator.ReaderCode;

namespace MusicCreator;

public partial class MainWindow : Window
{
    private SimpleSerialRead _serialRead = new SimpleSerialRead();

    public MainWindow()
    {
        InitializeComponent();
        ConfigureView();
        Sound.SetupSound();
        Sound.PlaySound("NoLanding.mp3");
       // _serialRead.SerialPortProgram();
    }

    private void ConfigureView()
    {
        Greeting.Text = "Hello World";
    }
}
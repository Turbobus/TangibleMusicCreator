using Avalonia.Controls;
using MusicCreator.Utils;

namespace MusicCreator;

public partial class MainWindow : Window
{
    private SimpleSerialRead _serialRead = new SimpleSerialRead();

    public MainWindow()
    {
        InitializeComponent();
        ConfigureView();
        _serialRead.SerialPortProgram();
    }

    private void ConfigureView()
    {
        Greeting.Text = "Hello World";
    }
}
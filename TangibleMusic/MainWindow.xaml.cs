using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TangibleMusic.ReaderCode;
using System.IO;


namespace TangibleMusic;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private SimpleSerialRead _simpleSerialRead = new SimpleSerialRead();
    private TextWriter originalConsoleOut;
    public MainWindow()
    {
        InitializeComponent();
        originalConsoleOut = Console.Out;

        // Redirect the Console.Out to a custom TextWriter
        Console.SetOut(new MultiTextWriter(originalConsoleOut, new MyTextBoxWriter(outputTextBox)));
        try
        {
            _simpleSerialRead.SerialPortProgram();
        }
        catch (Exception e)
        {
            Console.WriteLine("-ERROR- " + e.Message);
        }
    }
    
    private void ButtonStartScan(object sender, RoutedEventArgs e)
    {
        _simpleSerialRead.StartScan();
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
            Application.Current.Dispatcher.Invoke(() => textBox.AppendText(value.ToString()));
        }

        public override void Write(string value)
        {
            // Update the UI with the string
            Application.Current.Dispatcher.Invoke(() => textBox.AppendText(value));
        }

        public override Encoding Encoding => Encoding.UTF8; // Change the encoding as needed
    }
    
}
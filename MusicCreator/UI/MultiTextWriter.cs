using System.IO;
using System.Text;
using Avalonia.Controls;
using Avalonia.Threading;

namespace MusicCreator.UI;

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

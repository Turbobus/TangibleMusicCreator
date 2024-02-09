using System.IO.Ports;

namespace TangibleMusic.ReaderCode;

public class SimpleSerialRead
{
    //private SerialPort com4 = new SerialPort("COM4", 9600);
    
    // Create the serial port with basic settings 
    private SerialPort port = new   SerialPort("COM4", 9600, Parity.None); 
    
    public void SerialPortProgram() 
    { 
        Console.WriteLine("Incoming Data:");
        // Attach a method to be called when there
        // is data waiting in the port's buffer 
        port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived); 
        // Begin communications 
        port.Open(); 
        // Enter an application loop to keep this thread alive 
        Console.ReadLine();
    } 

    private void port_DataReceived(object sender, SerialDataReceivedEventArgs e) 
    { 
        // Show all the incoming data in the port's buffer
        Console.WriteLine(port.ReadExisting()); 
    } 
}
using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Collections;
using System.Text;

namespace TangibleMusic.ReaderCode;

public class SimpleSerialRead
{
    //private SerialPort com4 = new SerialPort("COM4", 9600);
    
    // Create the serial port with basic settings 
    private SerialPort port = new SerialPort("COM4", 9600, Parity.None);
    private StringBuilder stringBuilder = new StringBuilder();
    private bool haveFoundStart = false;
    private bool storeReads = false;
    private List<char> inputBuffer = new ();
    private ArrayList timeScans = new ArrayList();
    private ArrayList completeReads = new ArrayList();
    
    public void SerialPortProgram()
    { 
        Console.WriteLine("Incoming Data:");
        // Attach a method to be called when there
        // is data waiting in the port's buffer 
        port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
        
        // Begin communications 
        port.Open(); 
        // Enter an application loop to keep this thread alive 
    } 

    private void port_DataReceived(object sender, SerialDataReceivedEventArgs e) 
    { 
        // Show all the incoming data in the port's buffer
        string test = port.ReadExisting();

        foreach (char c in test)
        {
            if (!haveFoundStart)
            {
                if (c == (char)2)
                {
                    haveFoundStart = true;
                }
            }
            else
            {
                if (c == (char)3)
                {
                    string finalRead = stringBuilder.ToString();
                    Console.WriteLine(finalRead);
                    if (storeReads)
                    {
                        completeReads.Add(finalRead);
                    }
                    stringBuilder.Clear();
                    haveFoundStart = false;
                    break;
                }
                stringBuilder.Append(c);
            }
        }
        
        
        /*
        if (test.Contains((char)2))
        {
            Console.WriteLine("START!!!!");
            
        }
        if (test.Contains((char)3))
        {
            Console.WriteLine("SLUTTT!!!!");
        }
        Console.WriteLine(test);
        */
        /*
        for (int i = 0; i < port.BytesToRead; i++)
        {
            byte b = (byte) port.ReadByte();
            if (b == 0x2)
            {
                inputBuffer = new Char[128];
            }
            else if (b == 0x3)
            {
                Console.WriteLine(new string(inputBuffer));
                inputBuffer = new Char[128];
            }
            else
            {
                inputBuffer[i] = (char)b;
            }
        }
        */
    }

    public void StartScan()
    {
        Console.WriteLine("Scan starting");
        timeScans.Clear();
        storeReads = true;
        var stopwatch = Stopwatch.StartNew();
        while (stopwatch.Elapsed < TimeSpan.FromSeconds(6))
        {
            if (completeReads.Count > 0)
            {
                var active = completeReads[0];
                completeReads.RemoveAt(0);
                string timedHex = $"{active} {stopwatch.Elapsed}";
                Console.WriteLine(timedHex);
                timeScans.Add(timedHex);
            }
        }
        stopwatch.Stop();
        Console.WriteLine(timeScans);
    }
}
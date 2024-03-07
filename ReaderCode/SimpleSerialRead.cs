using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;


namespace MusicCreator.ReaderCode;

public class SimpleSerialRead
{
    //private SerialPort com4 = new SerialPort("COM4", 9600);
    
    // Create the serial port with basic settings 
    private SerialPort port;
    private StringBuilder stringBuilder = new StringBuilder();
    private bool haveFoundStart = false;
    private bool storeReads = false;
    private List<char> inputBuffer = new ();
    private List<(string,TimeSpan)> timeScans = new List<(string,TimeSpan)>();
    private List<string> completeReads = new List<string>();

    private Dictionary<string, string> tagConverter = new Dictionary<string, string>();
    
    public void StartSerialPortProgram(string portName)
    {
        try
        {
            port = new SerialPort(portName, 9600, Parity.None, 8, StopBits.One);
        }
        catch (Exception e)
        {
            Console.WriteLine("Couldn't open port, error: ", e);
        }

        Console.WriteLine("Incoming Data:");
        
        // tagConverter.Add("3D004AE3B622", "MainMenuMusic.mp3");
        // tagConverter.Add("3C0090A34649", "Yeah.mp3");
        // tagConverter.Add("3E000A6FBBE0", "toggle.mp3");
        
        
        // Attach a method to be called when there
        // is data waiting in the port's buffer 
        port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
        
        // Begin communications 
        port.Open(); 
        
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
                    CompleteRead();
                    haveFoundStart = false;
                    break;
                }
                stringBuilder.Append(c);
            }
        }
    }

    private void CompleteRead()
    {
        // Reads completed input string
        string finalRead = stringBuilder.ToString();
        
        // Plays sound for that tag (will be removed later)
        //Sound.PlaySound(tagConverter[finalRead]);
        
        // Read storage
        if (storeReads)
        {
            completeReads.Add(finalRead);
        }
        
        // Clear stringbuilder
        stringBuilder.Clear();
    }

    public void StartScan()
    {
        Console.WriteLine("Scan starting");
        timeScans.Clear();
        storeReads = true;
        var stopwatch = Stopwatch.StartNew();
        while (stopwatch.Elapsed < TimeSpan.FromSeconds(6))
        {
            //Console.WriteLine(stopwatch.Elapsed);
            //Console.WriteLine("Complete reads: " + completeReads.Count);
            if (completeReads.Count > 0)
            {
                var active = completeReads[0];
                completeReads.RemoveAt(0);
                TimeSpan timespan = stopwatch.Elapsed;
                string elapsedTime = $"{timespan.Seconds:00}:{timespan.Milliseconds:00}";
                Console.WriteLine($"{active} {elapsedTime}");
                timeScans.Add((active, timespan));
            }
        }
        
        stopwatch.Stop();
        Console.WriteLine("Listing all Items:");
        foreach ((string, TimeSpan) scan in timeScans)
        {
            string elapsedTime = $"{scan.Item2.Seconds:00}:{scan.Item2.Milliseconds:00}";
            Console.WriteLine(scan.Item1 + " " + elapsedTime);
        }
        Console.WriteLine("Scan complete");
        //Console.WriteLine(timeScans);
        PaceTimeScans(1.0/16.0);
    }

    private void PaceTimeScans(double tempo)
    {
        Console.WriteLine("Start Pace");
        List<(string, TimeSpan)> newList = new List<(string,TimeSpan)>();
        int index = 0;
        foreach ((string, TimeSpan) scan in timeScans)
        {
            double mili = scan.Item2.Milliseconds / 1000.0;
            double fraction = mili / tempo;
            int rounded = (int)Math.Round(fraction);
            Console.WriteLine("Org: " + mili + ", Rounded: " + rounded + ", Tempo: " + tempo );
            
            TimeSpan newTimeSpan = 
                new TimeSpan(0, 0, 0, (int)scan.Item2.TotalSeconds, (int)(rounded*tempo*10000));
            newList.Add((scan.Item1, newTimeSpan));
            index++;
        }
        
        foreach ((string, TimeSpan) scan in newList)
        {
            string elapsedTime = $"{scan.Item2.Seconds:00}:{scan.Item2.Milliseconds:00}";
            Console.WriteLine(scan.Item1 + " " + elapsedTime);
        }
    }
}
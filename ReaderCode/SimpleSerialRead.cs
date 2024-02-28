using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
            port = new SerialPort(portName, 115200, Parity.None, 8, StopBits.One);
        }
        catch (Exception e)
        {
            Console.WriteLine("Couldn't open port, error: ", e);
        }

        Console.WriteLine("Incoming Data:");
        
        tagConverter.Add("3D004AE3B622", "NoLanding.mp3");
        tagConverter.Add("3C0090A34649", "Yeah.mp3");
        tagConverter.Add("3E000A6FBBE0", "toggle.mp3");
        
        
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
        Console.WriteLine(finalRead);
        
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

    private void PlayScannedSound()
    {
        Console.WriteLine("\nStaring scanned playback");
        TimeSpan timeDiff = TimeSpan.Zero;
        
        foreach ((string, TimeSpan) tag in timeScans)
        {
            // I dont know why this sometimes throws an null pointer exception for the key
            // Added null check and hope it fixes it
            if (tag.Item1 == null || !tagConverter.ContainsKey(tag.Item1))
            { continue; }
            
            // Sleeps thread until next sound should play
            Thread.Sleep(tag.Item2 - timeDiff);
            
            Sound.PlaySound(tagConverter[tag.Item1]);
            timeDiff = tag.Item2;
        }
        Console.WriteLine("Finished scanned playback\n");
    }

    public void StartScan()
    {
        Console.WriteLine("Scan starting");
        timeScans.Clear();
        storeReads = true;
        var stopwatch = Stopwatch.StartNew();
        while (stopwatch.Elapsed < TimeSpan.FromSeconds(10))
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

        
        // Start playback thread
        Thread playbackThread = new Thread(PlayScannedSound);
        playbackThread.Start();
    }

    private void PaceTimeScans(int tempo)
    {
        int index = 0;
        foreach ((string, TimeSpan) scan in timeScans)
        {
            double fraction = 1.0 / tempo;
            double seconds = scan.Item2.TotalSeconds;
            double milliseconds = scan.Item2.TotalMilliseconds;
            double rest = milliseconds % fraction;

            // Calculate the adjusted milliseconds based on the closest tempo
            double adjustedMilliseconds;
            if (rest < fraction / 2)
            {
                adjustedMilliseconds = milliseconds - rest;
            }
            else
            {
                adjustedMilliseconds = milliseconds + (fraction - rest);
            }
            
            //TimeSpan newTimeSpan = TimeSpan.FromSeconds(scan.Item2.TotalSeconds + TimeSpan.FromMilliseconds(adjustedMilliseconds);
            //timeScans[index] = (scan.Item1, newTimeSpan);
        }
    }
}
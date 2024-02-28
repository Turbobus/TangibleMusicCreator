using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace MusicCreator.ReaderCode;

public class ReadSerialMusic
{
    // Create the serial port with basic settings 
    private SerialPort port;
    private StringBuilder stringBuilder = new StringBuilder();
    private bool haveFoundStart = false;
    private bool storeReads = false;
    private List<char> inputBuffer = new ();
    private List<(string,TimeSpan)> timeScans = new List<(string,TimeSpan)>();
    private List<string> completeReads = new List<string>();
    
    public void StartSerialMusicProgram(string portName)
    {
        try
        {
            port = new SerialPort(portName, 115200, Parity.None, 8, StopBits.One);
        }
        catch (Exception e)
        {
            Console.WriteLine("Couldn't open port, error: ", e);
        }

        Console.WriteLine("Incoming Data:\n");
        
        
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
        
        switch(finalRead) 
        {
            case "programOn":
                // Reset everything and be ready
                break;
            case "start":
                StartScan();
                break;
            case "end":
                // EndScan
                break;
            case "play":
                PlayScannedSound();
                break;
            case "error":
                // Play error sound
                break;
            default:
                // If we should save the reads, add to list
                if (storeReads)
                {
                    completeReads.Add(finalRead);
                }
                break;
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
            
            // Sleeps thread until next sound should play
            Thread.Sleep(tag.Item2 - timeDiff);
            
            Sound.PlaySound(tag.Item1);
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
}
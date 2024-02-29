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
    
    // Parsing helpers
    private readonly StringBuilder stringBuilder = new StringBuilder();
    private bool haveFoundStart = false;
    
    // Scanning helpers
    private bool storeReads = false;
    private readonly Stopwatch stopwatch = new Stopwatch();
    private readonly List<(string,TimeSpan)> timeScans = new List<(string,TimeSpan)>();
    
    // Playback helper
    private bool isPlaying = false;
    
    /// <summary>
    /// Starts music program on given serial port
    /// </summary>
    /// <param name="portName">Serial port</param>
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
        string incomingData = port.ReadExisting();

        foreach (char c in incomingData)
        {
            // If we haven't found start token (0x02)
            if (!haveFoundStart)
            {
                // Look for it
                if (c == (char)2)
                {
                    haveFoundStart = true;
                }
            }
            else
            {
                // If we found end token (0x03), complete read
                if (c == (char)3)
                {
                    CompleteRead();
                    haveFoundStart = false;
                    break;
                }
                
                // Otherwise, add token to string builder
                stringBuilder.Append(c);
            }
        }
    }

    private void CompleteRead()
    {
        // Reads completed input string
        string finalRead = stringBuilder.ToString();
        
        // Sends message to decoder
        DecodeCommand(finalRead);
        
        // Clear string builder
        stringBuilder.Clear();
    }

    internal void DecodeCommand(string finalRead)
    {
        Console.WriteLine(finalRead);
        switch(finalRead) 
        {
            case "programOn":
                stopwatch.Reset();
                timeScans.Clear();
                storeReads = false;
                break;
            case "start":
                if (storeReads) { break; } // Break if we already have started
                // Start stopwatch and clear timeScans
                stopwatch.Start();
                timeScans.Clear();
                storeReads = true;
                break;
            case "end":
                // Reset stopwatch and stop storing reads
                stopwatch.Reset();
                storeReads = false;
                break;
            case "play":
                if (isPlaying) { break; } // If we are already playing, break
                // Start a playback thread
                Thread playbackThread = new Thread(PlayScannedSound);
                isPlaying = true;
                playbackThread.Start();
                break;
            case "error":
                // Play error sound
                Sound.PlaySound("NoLanding.mp3");
                break;
            default:
                HandleIncomingSound(finalRead);
                break;
        }
    }

    private void HandleIncomingSound(string finalRead)
    {
        if (storeReads)
        {
            // Print value to console
            TimeSpan timeCode = stopwatch.Elapsed;
            string elapsedTime = $"{timeCode.Seconds:00}:{timeCode.Milliseconds:00}";
            Console.WriteLine($"{finalRead} at {elapsedTime}");
            
            // Add the sound with time code
            timeScans.Add((finalRead, timeCode));
            
            // Play the scanned sound
            Sound.PlaySound(finalRead);
        }
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

        isPlaying = false;
        Console.WriteLine("Finished scanned playback\n");
    }
}
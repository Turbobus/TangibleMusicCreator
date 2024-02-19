﻿using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using NAudio.Wave;

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
    private List<string> timeScans = new List<string>();
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
                    string finalRead = stringBuilder.ToString();
                    Sound.PlaySound(tagConverter[finalRead]);
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
                string timedHex = $"{active} {stopwatch.Elapsed}";
                Console.WriteLine(timedHex);
                timeScans.Add(timedHex);
            }
        }
        
        stopwatch.Stop();
        Console.WriteLine("List Items:");
        foreach (string scan in timeScans)
        {
            Console.WriteLine(scan);
        }
        Console.WriteLine("Scan complete");
        //Console.WriteLine(timeScans);
    }
}
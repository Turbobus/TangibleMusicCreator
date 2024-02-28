using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace MusicCreator.ReaderCode;

public static class Sound
{
    private static string soundFolderpath;
    private static readonly Dictionary<string,AudioPlaybackEngine.CachedSound> assets = new ();
    private static readonly AudioPlaybackEngine audioEngine = AudioPlaybackEngine.Instance;
    
    public static void SetupSound()
    {
        if (MacCheck()) { return; }
        soundFolderpath = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).FullName).FullName).FullName;
        soundFolderpath = soundFolderpath.Replace("\\", Path.DirectorySeparatorChar.ToString()) + Path.DirectorySeparatorChar + "Sounds";
        Console.WriteLine("Adding sound files:\n");
        WalkDirectory(soundFolderpath);
        Console.WriteLine("\nFinished adding sounds\n");
    }
    
    private static string GetName (string path) {
        string[] bits = path.Split (Path.DirectorySeparatorChar);
        return bits[bits.Length - 1];
    }
    
    private static void WalkDirectory(string dir) {
        string[] files = Directory.GetFiles(dir);
        string[] dirs = Directory.GetDirectories (dir);

        foreach(string d in dirs) {
            WalkDirectory (d);
        }

        foreach (string f in files) {
            string filename_raw = GetName(f);
            string filename = filename_raw;
            int counter = 0;

            while (assets.ContainsKey(filename)) {
                counter += 1;
                filename = filename_raw + counter;
            }

            try
            {
                assets.Add(filename, new AudioPlaybackEngine.CachedSound(f));
                Console.WriteLine("Adding " + filename + " : " + f);
            }
            catch (Exception e)
            {
                Console.WriteLine("FAILED to add sound clip : " + filename + " : " + "Invalid sound file");
            }
        }
    }
    
    public static void PlaySound(string soundFile)
    {
        Console.WriteLine("Playing sound: " + soundFile);
        
        if (MacCheck()) { return; }
        
        if (assets.TryGetValue(soundFile, out AudioPlaybackEngine.CachedSound asset))
        {
            audioEngine.PlaySound(asset);
        }
    }

    private static bool MacCheck()
    {
        Console.WriteLine(Path.DirectorySeparatorChar.ToString() != "\\");
        return Path.DirectorySeparatorChar.ToString() != "\\";
    }
}
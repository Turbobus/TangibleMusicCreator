using System.IO;
using NAudio.Wave;

namespace TangibleMusic.ReaderCode;

public static class Sound
{
    private static string soundFolderpath;
    private static readonly Dictionary<string,string> assets = new Dictionary<string, string>();
    private static WaveOutEvent soundPlayer = new WaveOutEvent();
    public static void SetupSound()
    {
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

            assets.Add(filename, f);
            Console.WriteLine("Adding " + filename + " : " + f);
        }
    }

    public static string GetSound(string soundFile)
    {
        if (assets.ContainsKey(soundFile))
        {
            return assets[soundFile];
        }
        
        Console.WriteLine("No entry for " + soundFile);
        return null;
    }
    
}
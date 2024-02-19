using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;

namespace MusicCreator.UI;

public class MusicExplorerViewModel: INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    private ObservableCollection<string> _items;

    public ObservableCollection<string> Items
    {
        get { return _items; }
        set
        {
            _items = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Items)));
        }
    }

    public MusicExplorerViewModel()
    {
        Items = new ObservableCollection<string>(GetSoundFiles());
    }
    
    private List<string> GetSoundFiles()
    {
        var workingDirectory = Environment.CurrentDirectory;
        var basePath = Directory.GetParent(workingDirectory)?.Parent?.Parent?.FullName ?? "";
        var musicPath = basePath + "/Sounds/";
        var d = new DirectoryInfo(musicPath);

        var files = d.GetFiles("*.mp3");
        var fileNames = new List<string>{};
        foreach(var file in files )
        {
            fileNames.Add(file.Name);
        }

        return fileNames;
    }
}
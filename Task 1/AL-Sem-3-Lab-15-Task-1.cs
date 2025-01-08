using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;

public class DirectoryWatcher
{
    private readonly string _directoryPath;
    private readonly System.Timers.Timer _timer;
    private readonly HashSet<string> _existingFiles;
    private readonly HashSet<string> _existingDirectories;

    public event Action<string> FileChanged;
    public event Action<string> DirectoryChanged;

    public DirectoryWatcher(string directoryPath, double checkInterval = 1000)
    {
        _directoryPath = directoryPath;
        _existingFiles = new HashSet<string>(Directory.GetFiles(directoryPath).Select(Path.GetFileName));
        _existingDirectories = new HashSet<string>(Directory.GetDirectories(directoryPath).Select(Path.GetFileName));

        _timer = new System.Timers.Timer(checkInterval);
        _timer.Elapsed += CheckForChanges;
        _timer.AutoReset = true;
        _timer.Start();
    }

    private void CheckForChanges(object sender, ElapsedEventArgs e)
    {
        var currentFiles = new HashSet<string>(Directory.GetFiles(_directoryPath).Select(Path.GetFileName));
        var currentDirectories = new HashSet<string>(Directory.GetDirectories(_directoryPath).Select(Path.GetFileName));

        // Проверка на изменения в файлах
        foreach (var file in currentFiles)
        {
            if (!_existingFiles.Contains(file))
            {
                FileChanged?.Invoke(file); // Файл был добавлен
            }
        }
        foreach (var file in _existingFiles)
        {
            if (!currentFiles.Contains(file))
            {
                FileChanged?.Invoke(file); // Файл был удален
            }
        }

        // Проверка на изменения в директориях
        foreach (var directory in currentDirectories)
        {
            if (!_existingDirectories.Contains(directory))
            {
                DirectoryChanged?.Invoke(directory); // Директория была добавлена
            }
        }
        foreach (var directory in _existingDirectories)
        {
            if (!currentDirectories.Contains(directory))
            {
                DirectoryChanged?.Invoke(directory); // Директория была удалена
            }
        }

        // Обновление существующих файлов и директорий
        _existingFiles.Clear();
        _existingFiles.UnionWith(currentFiles);
        _existingDirectories.Clear();
        _existingDirectories.UnionWith(currentDirectories);
    }

    public void Stop()
    {
        _timer.Stop();
        _timer.Dispose();
    }
}

class Program
{
    static void Main(string[] args)
    {
        string pathToWatch = @"C:\Users\User\Desktop"; // Укажите путь к директории

        DirectoryWatcher watcher = new DirectoryWatcher(pathToWatch);

        watcher.FileChanged += file => Console.WriteLine($"File changed: {file}");
        watcher.DirectoryChanged += directory => Console.WriteLine($"Directory changed: {directory}");

        Console.WriteLine("Press [Enter] to exit...");
        Console.ReadLine();

        watcher.Stop();
    }
}
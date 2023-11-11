using System;
using System.Collections.Generic;
using System.IO;

public static class FileExplorer
{
    private static string currentDirectory;
    private static bool isMenuOpen;
    private static int selectedIndex;
    private static List<string> menuItems;
    private static DirectoryInfo[] directories;
    private static FileInfo[] files;

    public static void Run()
    {
        currentDirectory = "";
        isMenuOpen = true;
        selectedIndex = 0;

        while (true)
        {
            Console.Clear();

            if (isMenuOpen)
            {
                ShowMenu();
            }
            else
            {
                ShowDirectoryContents();
            }

            ConsoleKeyInfo keyInfo = Console.ReadKey();
            if (isMenuOpen)
            {
                ProcessMenuInput(keyInfo);
            }
            else
            {
                ProcessDirectoryInput(keyInfo);
            }
        }
    }

    private static void ShowMenu()
    {
        menuItems = GetDriveNames();
        Console.WriteLine("Меню:");

        for (int i = 0; i < menuItems.Count; i++)
        {
            if (i == selectedIndex)
            {
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine(menuItems[i]);
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine(menuItems[i]);
            }
        }
    }

    private static void ShowDirectoryContents()
    {
        Console.WriteLine("Текущий путь: " + currentDirectory);
        Console.WriteLine();

        for (int i = 0; i < directories.Length; i++)
        {
            Console.WriteLine("[Папка] " + directories[i].Name);
        }

        for (int i = 0; i < files.Length; i++)
        {
            Console.WriteLine("[Файл] " + files[i].Name);
        }

        Console.WriteLine();
        Console.WriteLine("Выберите папку или файл или нажмите ESC для возврата в меню:");
    }

    private static void ProcessMenuInput(ConsoleKeyInfo keyInfo)
    {
        switch (keyInfo.Key)
        {
            case ConsoleKey.UpArrow:
                if (selectedIndex > 0)
                {
                    selectedIndex--;
                }
                break;
            case ConsoleKey.DownArrow:
                if (selectedIndex < menuItems.Count - 1)
                {
                    selectedIndex++;
                }
                break;
            case ConsoleKey.Enter:
                string selectedDrive = menuItems[selectedIndex];
                currentDirectory = selectedDrive;
                LoadDirectoryContents(selectedDrive);
                isMenuOpen = false;
                break;
        }
    }

    private static void ProcessDirectoryInput(ConsoleKeyInfo keyInfo)
    {
        switch (keyInfo.Key)
        {
            case ConsoleKey.Escape:
                if (currentDirectory != "")
                {
                    currentDirectory = Directory.GetParent(currentDirectory).FullName;
                    LoadDirectoryContents(currentDirectory);
                }
                else
                {
                    isMenuOpen = true;
                }
                break;
            case ConsoleKey.Insert:
                CreateNewFolder();
                break;
            case ConsoleKey.Delete:
                DeleteSelectedItem();
                break;
            case ConsoleKey.Enter:
                OpenSelectedFile();
                break;
            case ConsoleKey.UpArrow:
                if (selectedIndex > 0)
                {
                    selectedIndex--;
                }
                break;
            case ConsoleKey.DownArrow:
                if (selectedIndex < directories.Length + files.Length - 1)
                {
                    selectedIndex++;
                }
                break;
        }
    }

    private static List<string> GetDriveNames()
    {
        DriveInfo[] drives = DriveInfo.GetDrives();
        List<string> driveNames = new List<string>();

        foreach (DriveInfo drive in drives)
        {
            driveNames.Add(drive.Name);
        }

        return driveNames;
    }

    private static void LoadDirectoryContents(string path)
    {
        directories = new DirectoryInfo(path).GetDirectories();
        files = new DirectoryInfo(path).GetFiles();
        selectedIndex = 0;
    }

    private static void CreateNewFolder()
    {
        Console.Clear();
        Console.WriteLine("Введите имя новой папки:");
        string folderName = Console.ReadLine();

        Directory.CreateDirectory(Path.Combine(currentDirectory, folderName));
        LoadDirectoryContents(currentDirectory);
    }

    private static void DeleteSelectedItem()
    {
        string selectedItemPath = GetSelectedPath();
        bool isDirectory = Directory.Exists(selectedItemPath);
        bool isFile = File.Exists(selectedItemPath);

        if (isDirectory)
        {
            Directory.Delete(selectedItemPath, true);
        }
        else if (isFile)
        {
            File.Delete(selectedItemPath);
        }

        LoadDirectoryContents(currentDirectory);
    }

    private static void OpenSelectedFile()
    {
        string selectedItemPath = GetSelectedPath();
        bool isFile = File.Exists(selectedItemPath);

        if (isFile)
        {
            Console.WriteLine();
            Console.WriteLine("Выбран файл: " + selectedItemPath);
            Console.WriteLine("Нажмите любую клавишу для запуска файла...");
            Console.ReadKey();
            System.Diagnostics.Process.Start(selectedItemPath);
        }
    }

    private static string GetSelectedPath()
    {
        if (selectedIndex < directories.Length)
        {
            return directories[selectedIndex].FullName;
        }
        else
        {
            return files[selectedIndex - directories.Length].FullName;
        }
    }

    public static void ShowDriveInfo(string driveName)
    {
        DriveInfo drive = new DriveInfo(driveName);
        Console.WriteLine("Информация о диске:");
        Console.WriteLine("Имя: " + drive.Name);
        Console.WriteLine("Тип: " + drive.DriveType);
        Console.WriteLine("Общий объем: " + drive.TotalSize / (1024.0 * 1024 * 1024) + " ГБ");
        Console.WriteLine("Свободное пространство: " + drive.AvailableFreeSpace / (1024.0 * 1024 * 1024) + " ГБ");
    }
}

class Program
{
    static void Main()
    {
        FileExplorer.Run();
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace FileWatcher
{
    public partial class Service : ServiceBase
    {
        public Service()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // Creating a list of FileSystemWatchers and dynamically populating - one for each of my drives.
            List<FileSystemWatcher> fileSystemWatchers = new List<FileSystemWatcher>();
            fileSystemWatchers.Add(new FileSystemWatcher() { Path = @"C:\" });
            fileSystemWatchers.Add(new FileSystemWatcher() { Path = @"D:\" });
            fileSystemWatchers.Add(new FileSystemWatcher() { Path = @"E:\" });

            foreach (FileSystemWatcher fsw in fileSystemWatchers)
            {
                // I will be watching changes if any files/directories are read or written to.
                fsw.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
                fsw.Changed += new FileSystemEventHandler(OnChanged);
                fsw.Created += new FileSystemEventHandler(OnChanged);
                fsw.Deleted += new FileSystemEventHandler(OnChanged);
                fsw.Renamed += new RenamedEventHandler(OnRenamed);

                // Watch all files in folder and subfolders.
                fsw.Filter = "*.*";
                fsw.IncludeSubdirectories = true;

                // Start watching for the above events.
                fsw.EnableRaisingEvents = true;
            }
        }

        // Perhaps we can incorporate either verbose/minimalist options.
        // Verbose being all properties are being tracked. Minimalist being things like file and timestamp.
        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            string[] info = new string[] { "Timestamp: " + DateTime.UtcNow.ToString("MM-dd-yyyy HH:mm:ss"),
                                           "File/Directory Name: " + e.Name,
                                           "File/Directory URL: " + e.FullPath,
                                           "File/Directory Change: " + e.ChangeType.ToString()};
            // Adding in a condition to resolve issue of FileTracker recursively tracking itself on WriteToFile().
            if (!e.FullPath.Contains("FileWatcher.txt"))
                WriteToFile(info);
        }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            string[] info = new string[] { "Timestamp: " + DateTime.UtcNow.ToString("MM-dd-yyyy HH:mm:ss"),
                                           "File/Directory Original Name: " + e.OldName,
                                           "File/Directory New Name: " + e.Name,
                                           "File/Directory Original URL: " + e.OldFullPath,
                                           "File/Directory New URL: " + e.FullPath,
                                           "File/Directory Change: " + e.ChangeType.ToString()};
            // Adding in a condition to resolve issue of FileTracker recursively tracking itself on WriteToFile().
            if (!e.FullPath.Contains("FileWatcher.txt"))
                WriteToFile(info);
        }

        private void WriteToFile(string[] info)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/FileWatcher.txt";
            try
            {
                if (!File.Exists(path))
                {
                    // Creates a hidden, read-only text file.
                    File.SetAttributes(path, FileAttributes.Hidden | FileAttributes.ReadOnly);
                    File.CreateText(path);
                }

                File.AppendAllLines(path, info);
                // Without having to write an extension to System.IO.File, we can prepend latest changes with code below.
                // TODO: Test the code below.

                //if (File.Exists(path))
                //{
                //    string newPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/FileWatcher.temp.txt";
                //    string[] previousContent = File.ReadAllLines(path);
                //    File.CreateText(newPath);
                //    Array.Resize(ref info, info.Length + previousContent.Length);
                //    Array.Copy(previousContent, info, previousContent.Length);
                //    File.WriteAllLines(newPath, info);
                //    File.Replace("FileWatcher.temp.txt", "FileWatcher.txt", "FileWatcher.bk.txt");
                //}
            }
            catch (Exception ex)
            {
                Console.Write(ex.InnerException);
            }
        }
    }
}

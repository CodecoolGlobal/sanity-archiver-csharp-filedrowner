using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanityArchiver.Application.Models
{
    /// <summary>
    /// Dolgok
    /// </summary>
    public class DriveExplorer
    {
        /// <summary>
        /// All Files list
        /// </summary>
        private ObservableCollection<MenuItem> _allFiles = new ObservableCollection<MenuItem>();

        private ObservableCollection<MenuItem> _drives;

        /// <summary>
        /// Gets Folders
        /// </summary>
        public ObservableCollection<MenuItem> Drives
        {
            get
            {
                _drives = new ObservableCollection<MenuItem>();

                DriveInfo[] drives = DriveInfo.GetDrives();
                foreach (DriveInfo drive in drives)
                {
                    if (drive.DriveType == DriveType.Fixed)
                    {
                        MenuItem newFolder = new MenuItem { Title = drive.Name };
                        _drives.Add(newFolder);
                    }
                }

                return _drives;
            }
        }

        /// <summary>
        /// Gets E Drives folders and files
        /// </summary>
        public ObservableCollection<MenuItem> E_Drive
        {
            get
            {
                ObservableCollection<MenuItem> localFiles = new ObservableCollection<MenuItem>();
                try
                {
                    /*DriveInfo dir = new DriveInfo(@"E:\");
                    var directories = Directory.GetDirectories(@"E:\", "*", SearchOption.AllDirectories);
                    foreach (string directory1 in directories)
                    {
                        localFiles.Add(new MenuItem { Title = directory1 });
                    }

                    DirectoryInfo directory = dir.RootDirectory;
                    FileInfo[] files = directory.GetFiles("*.*");
                    foreach (FileInfo file in files)
                    {
                        localFiles.Add(new MenuItem { Title = file.Name });
                    }*/
                }
                catch (UnauthorizedAccessException e)
                {
                    string errorMessage = e.Message;
                }

                return localFiles;
            }
        }

        /// <summary>
        /// Recursive function to walk over my hard drive, <paramref name="rootDir"/>
        /// </summary>
        /// <returns>List of MenuItems</returns>
#pragma warning disable SA1611 // Element parameters should be documented
        public ObservableCollection<MenuItem> WalkLocalDrive(DirectoryInfo rootDir, MenuItem placeHolder)
#pragma warning restore SA1611 // Element parameters should be documented
        {
            FileInfo[] files = null;
            DirectoryInfo[] subDirs = null;

            ObservableCollection<MenuItem> result = new ObservableCollection<MenuItem>();

            try
            {
                files = rootDir.GetFiles("*");
                subDirs = rootDir.GetDirectories();
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine(e.Message + " | ACCESS DENIED");
            }
            catch (System.IO.DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }

            if (files != null)
            {
                foreach (FileInfo file in files)
                {
                    placeHolder.AddToItems(new MenuItem { Title = file.Name });
                }
            }

            if (subDirs != null)
            {
                foreach (DirectoryInfo dir in subDirs)
                {
                    MenuItem theDirectory = new MenuItem { Title = dir.Name };
                    try
                    {
                        if (dir.GetDirectories() != null)
                        {
                            WalkLocalDrive(dir, theDirectory);
                        }
                    }
                    catch (UnauthorizedAccessException e)
                    {
                        string errorMessage = e.Message;
                    }

                    placeHolder.AddToItems(theDirectory);
                }
            }

            return _allFiles;
        }
    }
}

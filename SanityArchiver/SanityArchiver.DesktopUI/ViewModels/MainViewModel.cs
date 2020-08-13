using System;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows.Controls;
using SanityArchiver.Application.Models;

namespace SanityArchiver.DesktopUI.ViewModels
{
    /// <summary>
    /// Main View Model Controller
    /// </summary>
    public class MainViewModel : PropertyNotifier
    {
        private const string TempFolderPath = @"E:\TestForArchive";
        private static MainViewModel _instance;
        private FileSystemObjectInfo _selectedItem;
        private ObservableCollection<FileSystemObjectInfo> _searchedObjects;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        private MainViewModel()
        {
            if (_instance == null)
            {
                _searchedObjects = new ObservableCollection<FileSystemObjectInfo>();
                _instance = this;
            }
        }

        /// <summary>
        /// Gets or sets selected Folder
        /// </summary>
        public FileSystemObjectInfo SelectedItem
        {
            get
            {
                return _selectedItem;
            }

            set
            {
                _selectedItem = value;
            }
        }

        /// <summary>
        /// Gets collection of Objects to Display
        /// </summary>
        public ObservableCollection<FileSystemObjectInfo> SearchedObjects
        {
            get
            {
                return _searchedObjects;
            }
        }

        /// <summary>
        /// Get the Instance of the ViewModel
        /// </summary>
        /// <returns>The ViewModel</returns>
        public static MainViewModel GetInstance()
        {
            if (_instance == null)
            {
                _instance = new MainViewModel();
            }

            return _instance;
        }

        /// <summary>
        /// Search for specific Folder to Show
        /// </summary>
        /// <param name="path">Path of the clicked Folder</param>
        public void SearchForFolder(string path)
        {
            try
            {
                FileAttributes attributes = File.GetAttributes(path);
                if (attributes.HasFlag(FileAttributes.Directory))
                {
                    var dirs = Directory.GetFileSystemEntries(path, "*");
                    foreach (string dir in dirs)
                    {
                        FileAttributes fileAttribute = File.GetAttributes(dir);
                        if (fileAttribute.HasFlag(FileAttributes.Directory))
                        {
                            DirectoryInfo info = new DirectoryInfo(dir);
                            bool isHidden = false;
                            if (info.Attributes.HasFlag(FileAttributes.Hidden))
                            {
                                isHidden = true;
                            }

                            AddToSearchedObjectList(new FileSystemObjectInfo(info.Root) { Title = info.Name, CreationDate = info.CreationTime, Path = info.FullName, Hidden = isHidden });
                        }
                        else
                        {
                            FileInfo info = new FileInfo(dir);
                            bool isHidden = false;
                            if (info.Attributes.HasFlag(FileAttributes.Hidden))
                            {
                                isHidden = true;
                            }

                            AddToSearchedObjectList(new FileSystemObjectInfo(info.Directory.Root) { Title = info.Name, CreationDate = info.CreationTime, Size = info.Length, Path = info.FullName, Hidden = isHidden });
                        }
                    }
                }
                else
                {
                    var fileName = Path.GetFileName(path);

                    FileInfo info = new FileInfo(fileName);
                    bool isHidden = false;
                    if (info.Attributes.HasFlag(FileAttributes.Hidden))
                    {
                        isHidden = true;
                    }

                    AddToSearchedObjectList(new FileSystemObjectInfo(info.Directory.Root) { Title = info.Name, CreationDate = info.CreationTime, Path = info.FullName, Hidden = isHidden });
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
                return;
            }
        }

        /// <summary>
        /// Adds to the Seached List
        /// </summary>
        /// <param name="item">Item we want to add</param>
        public void AddToSearchedObjectList(FileSystemObjectInfo item)
        {
            _searchedObjects.Add(item);

            OnPropertyChanged(item.Title.ToString());
        }

        /// <summary>
        /// Clears the seached Objects
        /// </summary>
        public void ClearSearchedObjects()
        {
            _searchedObjects.Clear();
        }

        /// <summary>
        /// Builds up and saves the Selected Item
        /// </summary>
        /// <param name="path">Full Path of the File</param>
        public void SelectAnItem(string path)
        {
            foreach (FileSystemObjectInfo item in _searchedObjects)
            {
                if (item.Path.Equals(path))
                {
                    SelectedItem = item;
                }
            }
        }

        /// <summary>
        /// Deletes the given File Attribute from the File
        /// </summary>
        /// <param name="attributes">File's attribute</param>
        /// <param name="attributesToRemove">Attribute you want to delete</param>
        /// <returns>The attributes without the chosen one</returns>
        internal static FileAttributes RemoveAttribute(FileAttributes attributes, FileAttributes attributesToRemove)
        {
            return attributes & ~attributesToRemove;
        }

        /// <summary>
        /// Rename file
        /// </summary>
        /// <param name="newName">New name/path for the file</param>
        internal void ModifyFile(string newName)
        {
            string oldPath = SelectedItem.Path;
            int lengthOfOldName = SelectedItem.Title.Length;
            string subStringOldPath = oldPath.Substring(0, oldPath.Length - lengthOfOldName);
            string newPath = subStringOldPath + newName;

            FileAttributes attributes = File.GetAttributes(SelectedItem.Path);
            if ((attributes & FileAttributes.Directory) != FileAttributes.Directory)
            {
                File.Move(oldPath, newPath);
                SelectedItem.Path = newPath;
                SelectedItem.Title = newName;
            }

            if ((attributes & FileAttributes.Hidden) == FileAttributes.Hidden && !SelectedItem.Hidden)
            {
                attributes = RemoveAttribute(attributes, FileAttributes.Hidden);
                File.SetAttributes(SelectedItem.Path, attributes);
                SelectedItem.Hidden = false;
            }
            else
            {
                File.SetAttributes(SelectedItem.Path, File.GetAttributes(SelectedItem.Path) | FileAttributes.Hidden);
                SelectedItem.Hidden = true;
            }
        }

        /// <summary>
        /// Deletes File
        /// </summary>
        internal void DeleteFile()
        {
            if (SelectedItem.FileSystemInfo.Attributes.HasFlag(FileAttributes.Directory))
            {
                Directory.Delete(SelectedItem.Path);
            }
            else
            {
                File.Delete(SelectedItem.Path);
            }

            OnPropertyChanged("Delete");
        }

        /// <summary>
        /// Save the selected item to the Field
        /// </summary>
        /// <param name="item">Item to save</param>
        internal void SelectedFileSaveToField(ListViewItem item)
        {
            foreach (FileSystemObjectInfo file in _searchedObjects)
            {
                if (file.Path.Equals(item.Tag.ToString()))
                {
                    SelectedItem = file;
                }
            }
        }

        /// <summary>
        /// Compress
        /// </summary>
        internal void CompressFile()
        {
            string oldPath = SelectedItem.Path;
            int lengthOfOldName = SelectedItem.Title.Length;
            string folderToExtract = oldPath.Substring(0, oldPath.Length - lengthOfOldName);
            string tempFolder = TempFolderPath + "\\" + SelectedItem.Title;
            string zipFileName = SelectedItem.Title.Substring(0, SelectedItem.Title.Length - 4) + ".zip";

            FileAttributes attributes = File.GetAttributes(SelectedItem.Path);
            if ((attributes & FileAttributes.Directory) != FileAttributes.Directory)
            {
                File.Move(oldPath, tempFolder);
                ZipFile.CreateFromDirectory(TempFolderPath, folderToExtract + zipFileName);

                DirectoryInfo tempFolderToClear = new DirectoryInfo(TempFolderPath);
                foreach (FileInfo file in tempFolderToClear.GetFiles())
                {
                    file.Delete();
                }
            }
        }

        /// <summary>
        /// Decompress a file
        /// </summary>
        internal void DeCompressFile()
        {
            if (Path.GetExtension(SelectedItem.Path).Equals(".zip"))
            {
                string extractDestination = SelectedItem.Path.Substring(0, SelectedItem.Path.Length - SelectedItem.Title.Length);
                ZipFile.ExtractToDirectory(SelectedItem.Path, extractDestination);
            }
        }
    }
}

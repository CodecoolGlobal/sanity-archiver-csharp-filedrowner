using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SanityArchiver.Application.Models
{
    /// <summary>
    /// Most important class
    /// </summary>
    public class FileSystemObjectInfo : BaseObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemObjectInfo"/> class.
        /// Construktor for Drivers
        /// </summary>
        /// <param name="drive">Local Hard Drive</param>
        public FileSystemObjectInfo(DriveInfo drive)
        : this(drive.RootDirectory)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemObjectInfo"/> class.
        /// Construktor for Folders and Files
        /// </summary>
        /// <param name="info">The file or folder</param>
        public FileSystemObjectInfo(FileSystemInfo info)
        {
            if (this is DummyFileSystemObjectInfo)
            {
                return;
            }

            Children = new ObservableCollection<FileSystemObjectInfo>();
            FileSystemInfo = info;
            Title = info.Name;
            Path = info.FullName;
            CreationDate = info.CreationTime;

            FileInfo file = new FileInfo(Path);
            if (file.Attributes.HasFlag(FileAttributes.Hidden))
            {
                Hidden = true;
            }
            else
            {
                Hidden = false;
            }

            if (info is DirectoryInfo)
            {
                AddDummy();
            }

            PropertyChanged += new PropertyChangedEventHandler(FileSystemObjectInfo_PropertyChanged);
        }

        /// <summary>
        /// BeforeExpand Event
        /// </summary>
        public event EventHandler BeforeExpand;

        /// <summary>
        /// AfterExpand event
        /// </summary>
        public event EventHandler AfterExpand;

        /// <summary>
        /// Event handler before exploring a driver or folder
        /// </summary>
        public event EventHandler BeforeExplore;

        /// <summary>
        /// Event handler after exploring a driver or folder
        /// </summary>
        public event EventHandler AfterExplore;

        /// <summary>
        /// Gets childrens of the Element
        /// </summary>
        public ObservableCollection<FileSystemObjectInfo> Children
        {
            get { return GetValue<ObservableCollection<FileSystemObjectInfo>>("Children"); }
            private set { SetValue("Children", value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the boolean value of Expanded
        /// </summary>
        public bool IsExpanded
        {
            get { return GetValue<bool>("IsExpanded"); }
            set { SetValue("IsExpanded", value); }
        }

        /// <summary>
        /// Gets or sets the tag of the TreeView Item
        /// </summary>
        public string Title
        {
            get
            {
                return GetValue<string>("Title");
            }

            set
            {
                SetValue("Title", value);
            }
        }

        /// <summary>
        /// Gets or sets the creation Date of the Folder/File
        /// </summary>
        public DateTime CreationDate
        {
            get { return GetValue<DateTime>("CreationDate"); }
            set { SetValue("CreationDate", value); }
        }

        /// <summary>
        /// Gets or sets path of the File
        /// </summary>
        public string Path
        {
            get { return GetValue<string>("Path"); }
            set { SetValue("Path", value); }
        }

        /// <summary>
        /// Gets or sets size of the File, only works for Files!!
        /// </summary>
        public long Size
        {
            get { return GetValue<long>("Size"); }
            set { SetValue("Size", value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the file hide property
        /// </summary>
        public bool Hidden
        {
            get { return GetValue<bool>("Hidden"); }
            set { SetValue("Hidden", value);  }
        }

        /// <summary>
        /// Gets the Info
        /// </summary>
        public FileSystemInfo FileSystemInfo
        {
            get { return GetValue<FileSystemInfo>("FileSystemInfo"); }
            private set { SetValue("FileSystemInfo", value); }
        }

        /// <summary>
        /// Gets or sets the Drive
        /// </summary>
        private DriveInfo Drive
        {
            get { return GetValue<DriveInfo>("Drive"); }
            set { SetValue("Drive", value); }
        }

        private void RaiseBeforeExpand()
        {
            BeforeExpand?.Invoke(this, EventArgs.Empty);
        }

        private void RaiseAfterExpand()
        {
            AfterExpand?.Invoke(this, EventArgs.Empty);
        }

        private void RaiseBeforeExplore()
        {
            BeforeExplore?.Invoke(this, EventArgs.Empty);
        }

        private void RaiseAfterExplore()
        {
            AfterExplore?.Invoke(this, EventArgs.Empty);
        }

        private void AddDummy()
        {
            Children.Add(new DummyFileSystemObjectInfo());
        }

        private bool HasDummy()
        {
            return !object.ReferenceEquals(GetDummy(), null);
        }

        private DummyFileSystemObjectInfo GetDummy()
        {
            var list = Children.OfType<DummyFileSystemObjectInfo>().ToList();
            if (list.Count > 0)
            {
                return list.First();
            }

            return null;
        }

        private void RemoveDummy()
        {
            Children.Remove(GetDummy());
        }

        private void ExploreDirectories()
        {
            if (Drive?.IsReady == false)
            {
                return;
            }

            if (FileSystemInfo is DirectoryInfo)
            {
                var directories = ((DirectoryInfo)FileSystemInfo).GetDirectories();
                foreach (var directory in directories.OrderBy(d => d.Name))
                {
                    if ((directory.Attributes & FileAttributes.System) != FileAttributes.System &&
                        (directory.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                    {
                        var fileSystemObject = new FileSystemObjectInfo(directory);
                        fileSystemObject.BeforeExplore += FileSystemObject_BeforeExplore;
                        fileSystemObject.AfterExplore += FileSystemObject_AfterExplore;
                        Children.Add(fileSystemObject);
                    }
                }
            }
        }

        private void FileSystemObject_AfterExplore(object sender, EventArgs e)
        {
            RaiseAfterExplore();
        }

        private void FileSystemObject_BeforeExplore(object sender, EventArgs e)
        {
            RaiseBeforeExplore();
        }

        private void ExploreFiles()
        {
            if (Drive?.IsReady == false)
            {
                return;
            }

            if (FileSystemInfo is DirectoryInfo)
            {
                var files = ((DirectoryInfo)FileSystemInfo).GetFiles();
                foreach (var file in files.OrderBy(d => d.Name))
                {
                    if ((file.Attributes & FileAttributes.System) != FileAttributes.System &&
                        (file.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                    {
                        Children.Add(new FileSystemObjectInfo(file));
                    }
                }
            }
        }

        private void FileSystemObjectInfo_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (FileSystemInfo is DirectoryInfo)
            {
                if (string.Equals(e.PropertyName, "IsExpanded", StringComparison.CurrentCultureIgnoreCase))
                {
                    RaiseBeforeExpand();
                    if (IsExpanded)
                    {
                        if (HasDummy())
                        {
                            RaiseBeforeExplore();
                            RemoveDummy();
                            ExploreDirectories();
                            ExploreFiles();
                            RaiseAfterExplore();
                        }
                    }

                    RaiseAfterExpand();
                }
            }
        }
    }
}

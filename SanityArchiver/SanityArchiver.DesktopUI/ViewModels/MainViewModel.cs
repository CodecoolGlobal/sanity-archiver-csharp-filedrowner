using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SanityArchiver.Application.Models;

namespace SanityArchiver.DesktopUI.ViewModels
{
    /// <summary>
    /// MainViewModel dolog Class
    /// </summary>
    public class MainViewModel
    {
        /// <summary>
        /// Instance of DriveExplorer
        /// </summary>
        public DriveExplorer _driveExplorer;

        /// <summary>
        /// List of Directories
        /// </summary>
        private ObservableCollection<MenuItem> _menuItems = new ObservableCollection<MenuItem>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        public MainViewModel()
        {
            _driveExplorer = new DriveExplorer();
        }

        /// <summary>
        /// Gets menuItems Property
        /// </summary>
        public ObservableCollection<MenuItem> MenuItems
        {
            get
            {
                return _menuItems;
            }
        }

        /// <summary>
        /// Fills Up the MenuItems with the local drives
        /// </summary>
        public void FillUpMenuItems()
        {
            MenuItem root = new MenuItem { Title = "Local Drive" };
            DirectoryInfo driveE = new DirectoryInfo(@"E:\");
            foreach (MenuItem item in _driveExplorer.WalkLocalDrive(driveE, root))
            {
                root.Items.Add(item);
            }

            _menuItems.Add(root);
            }
        }
    }

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using SanityArchiver.Application.Models;
using SanityArchiver.DesktopUI.ViewModels;

namespace SanityArchiver.DesktopUI.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel _vm;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            _vm = MainViewModel.GetInstance();
            InitializeComponent();
            InitializeFileSystemObjects();
            DataContext = _vm;
        }

        private void InitializeFileSystemObjects()
        {
            var drives = DriveInfo.GetDrives();
            DriveInfo.GetDrives().ToList().ForEach(drive =>
            {
                var fileSystemObject = new FileSystemObjectInfo(drive);
                fileSystemObject.BeforeExplore += FileSystemObject_BeforeExplore;
                fileSystemObject.AfterExplore += FileSystemObject_AfterExplore;
                treeView.Items.Add(fileSystemObject);
                /*treeView.Tag = fileSystemObject.Path;*/
            });
        }

        private void FileSystemObject_AfterExplore(object sender, System.EventArgs e)
        {
            Cursor = Cursors.Arrow;
        }

        private void FileSystemObject_BeforeExplore(object sender, System.EventArgs e)
        {
            Cursor = Cursors.Wait;
        }

        private void TreeViewItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem = sender as TreeViewItem;
            e.Handled = true;
            string selectedItemTag = treeViewItem.Tag.ToString();
            _vm.ClearSearchedObjects();
            _vm.SearchForFolder(selectedItemTag);
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListViewItem item = sender as ListViewItem;
            _vm.SelectAnItem(item.Tag.ToString());
            RenameWindow popUp = new RenameWindow();
            popUp.Show();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            _vm.DeleteFileOrFolder();
        }

        private void ListViewItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListViewItem item = sender as ListViewItem;
            _vm.SelectedFileSaveToField(item);
        }

        private void Compress_File(object sender, RoutedEventArgs e)
        {
            _vm.CompressFile();
        }

        private void Decompress_File(object sender, RoutedEventArgs e)
        {
            _vm.DeCompressFile();
        }
    }
}

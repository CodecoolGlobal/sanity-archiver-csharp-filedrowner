using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SanityArchiver.DesktopUI.ViewModels;

namespace SanityArchiver.DesktopUI.Views
{
    /// <summary>
    /// Interaction logic for RenameWindow.xaml
    /// </summary>
    public partial class RenameWindow : Window
    {
        private MainViewModel _vm;

        /// <summary>
        /// Initializes a new instance of the <see cref="RenameWindow"/> class.
        /// PopUp Window's Code Behind
        /// </summary>
        public RenameWindow()
        {
            _vm = MainViewModel.GetInstance();
            InitializeComponent();
            DataContext = _vm;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _vm.RenameFile(input.Text);
            Close();
        }
    }
}

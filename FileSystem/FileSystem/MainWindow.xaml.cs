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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace FileSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            List<File> filesList = new List<File>();
            filesList.Add(new File("a.txt", "1Kb"));
            filesList.Add(new File("b.txt", "2Kb"));
            ObservableCollection<File> filesView = new ObservableCollection<File>(filesList);
            FilesListView.DataContext = filesView;

            MenuItem root = new MenuItem() { Title = "Menu" };
            MenuItem childItem1 = new MenuItem() { Title = "Child item #1" };
            childItem1.Items.Add(new MenuItem() { Title = "Child item #1.1" });
            childItem1.Items.Add(new MenuItem() { Title = "Child item #1.2" });
            root.Items.Add(childItem1);
            root.Items.Add(new MenuItem() { Title = "Child item #2" });
            TreeViewMenu.Items.Add(root);
        }

        private void DataGridCell_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridCell cell = (DataGridCell)sender;
            File f = (File)cell.DataContext;
        }

        private void DataGridCell_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            ContextMenu cm = this.FindResource("RightClickMenu") as ContextMenu;
            cm.PlacementTarget = sender as DataGridCell;
            cm.IsOpen = true;
        }

        
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Custom Command Executed");
        }
    }


}

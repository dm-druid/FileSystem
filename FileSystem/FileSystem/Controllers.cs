using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FileSystem
{
    public partial class MainWindow : Window
    {
        private CatalogItem copyCache;

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (folderStack.Count() == 1)
            {
                return;
            }
            folderStack.Pop();
            current_item = folderStack.Peek();
            UpdateListView(current_item);
            if (folderStack.Count() == 1) { backButton.IsEnabled = false; }
        }

        private void DataGridCell_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            ContextMenu cm = this.FindResource("RightClickMenu") as ContextMenu;
            cm.PlacementTarget = sender as DataGridCell;
            cm.IsOpen = true;
        }

        private void DataGridCell_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridCell cell = (DataGridCell)sender;
            File f = (File)cell.DataContext;
            CatalogItem it = catalog_table.getCatelogItem(f);
            openOperation(it, f);
        }
        private void openOperation(CatalogItem it, File f)
        {
            if (it.type == CatalogItem.FileType.FOLDER)
            {
                current_item = it;
                folderStack.Push(current_item);
                backButton.IsEnabled = true;
                UpdateListView(current_item);
            }
            else if (it.type == CatalogItem.FileType.TXT)
            {
                textEditorWindow textWindow = new textEditorWindow(f);
                textWindow.Show();
                textWindow.CallBackMethod = UpdateView;
            }
        }

        private void MenuItemOpen_Click(object sender, RoutedEventArgs e)
        {
            File f = FilesListView.SelectedItem as File;
            if (f == null) { MessageBox.Show("Please select the item first."); return; }
            CatalogItem it = catalog_table.getCatelogItem(f);
            openOperation(it, f);
        }
        private void MenuItemRename_Click(object sender, RoutedEventArgs e)
        {
            File f = FilesListView.SelectedItem as File;
            if (f == null) { MessageBox.Show("Please select the item first."); return; }
            System.Windows.Controls.MenuItem item = sender as System.Windows.Controls.MenuItem;
            string input = Microsoft.VisualBasic.Interaction.InputBox("new name", "Rename", "Default");
            do {
                if (checkName(input)) break;
                input = Microsoft.VisualBasic.Interaction.InputBox("Give a correct name which is a unique name without the forbidden characters:\"\\ /  * ? \" < > → | ~\"", "Rename", "Default");
            } while (true);
            f.Name = input;
            CatalogItem tmp_item = catalog_table.getCatelogItem(f);
            tmp_item.fileName = input;
            UpdateView();
        }
        private void MenuItemProperty_Click(object sender, RoutedEventArgs e)
        {
            File f = FilesListView.SelectedItem as File;
            if (f == null) { MessageBox.Show("Please select the item first."); return; }
            propertyWindow proWindow = new propertyWindow(f);
            proWindow.Show();

        }
        private void MenuItemDelete_Click(object sender, RoutedEventArgs e)
        {
            File f = FilesListView.SelectedItem as File;
            if (f == null) { MessageBox.Show("Please select the item first."); return; }
            List<int> seq = f.indexTablePointer.fetchContent();
            disk.deallocate(seq); // free disk

            CatalogItem it = catalog_table.getCatelogItem(f);
            it.remove();
            catalog_table.remove(it);
            UpdateView();
        }
        private void MenuItemCopy_Click(object sender, RoutedEventArgs e)
        {
            File f = FilesListView.SelectedItem as File;
            if (f == null) { MessageBox.Show("Please select the item first."); return; }
            CatalogItem it = catalog_table.getCatelogItem(f);
            // copy the catalogItem
            copyCache = it;
        }
        private void MenuItemShortcut_Click(object sender, RoutedEventArgs e)
        {
            File f = FilesListView.SelectedItem as File;
            if (f == null) { MessageBox.Show("Please select the item first."); return; }
            CatalogItem it = catalog_table.getCatelogItem(f);
            CatalogItem alias = it.alias();
            // copy the catalogItem
            current_item.addSon(alias);
            UpdateView();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            UpdateListView(current_item);
        }
        private void Paste_Click(object sender, RoutedEventArgs e)
        {
            CatalogItem cata = copyCache.copy();
            File originalFile = catalog_table.map(copyCache);
            File copyFile = originalFile.copy(copyCache);

            // duplocate a new file
            // get the content string
            List<int> seq = originalFile.fetchContent();
            string result = "";
            foreach (int i in seq)
            {
                result += disk.getDataBlock(i);
            }
            // write the content
            IndexTable table = disk.writeDisk(result);
            copyFile.indexTablePointer = table;

            // map the new file
            catalog_table.map(cata, copyFile);
            current_item.addSon(cata);
            UpdateListView(current_item);
        }

        private void FilesListView_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            ContextMenu cm = this.FindResource("BlankRightClickMenu") as ContextMenu;
            cm.PlacementTarget = sender as DataGrid;
            cm.IsOpen = true;
        }

        private void NewFile_Click(object sender, RoutedEventArgs e)
        {
            string tmp_name = findName("New Text", ".TXT"); // deal with the same name
            CatalogItem item = new CatalogItem(tmp_name, CatalogItem.FileType.TXT);

            current_item.addSon(item);
            File fatherFile = catalog_table.map(current_item);
            string fatherPath = "root";
            if (fatherFile != null) { fatherPath = fatherFile.Path; }
            File file = new File(item, fatherPath);
            catalog_table.map(item, file);
            UpdateView();
        }
        private void NewFolder_Click(object sender, RoutedEventArgs e)
        {
            string tmp_name = findName("New Folder"); // deal with the same name
            CatalogItem item = new CatalogItem(tmp_name, CatalogItem.FileType.FOLDER);
            current_item.addSon(item);
            File fatherFile = catalog_table.map(current_item);
            string fatherPath = "root";
            if (fatherFile != null) { fatherPath = fatherFile.Path; }
            File file = new File(item, fatherPath);
            catalog_table.map(item, file);
            UpdateView();
        }
        private string findName(string s, string ext = "")
        {
            CatalogItem dir = current_item.son;
            while (dir != null)
            {
                if (dir.fileName == s + ext) { s += "_副本"; dir = current_item.son; continue; }
                dir = dir.next;
            }
            return s + ext;
        }
        private bool checkName(string s)
        {
            CatalogItem dir = current_item.son;
            foreach (String c in ConstValue.forbiddenChar)
            {
                if (s.Contains(c)) { return false; }
            }
            while (dir != null)
            {
                if (dir.fileName == s) { return false; }
                dir = dir.next;
            }
            return true;
        }
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string s = searchTextBox.Text;
            CatalogItem dir = current_item.son;
            List<File> filesList = new List<File>();
            while (dir != null)
            {
                if (dir.fileName.Contains(s)) { filesList.Add(catalog_table.map(dir)); }
                dir = dir.next;
            }
            if (filesList.Count() == 0) { MessageBox.Show("There is no such files containg the input string."); }
            else
            {
                ObservableCollection<File> filesView = new ObservableCollection<File>(filesList);
                FilesListView.DataContext = filesView;
            }
        }
        private void DiskFormatting_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Do you want to clear your disk record?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                SerializeNow();
                UpdateView();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.ComponentModel;

namespace FileSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CatalogItem root_item = new CatalogItem();
        MenuItem rootMenu;
        public CatalogItem current_item;
        CatalogTable catalog_table = new CatalogTable();
        Stack<CatalogItem> folderStack = new Stack<CatalogItem>();
        StateRecord stateRecord = new StateRecord();
        static public Disk disk = new Disk();
        public string dir = System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(Directory.GetCurrentDirectory()));

        public MainWindow()
        {
            InitializeComponent();
            //SerializeNow(); // initiate the workpalce
            DeSerializeNow();
            current_item = root_item;
            folderStack.Push(current_item);
            InitiateView();
        }
        public void InitiateView()
        {
            rootMenu = new MenuItem() { Title = "root" };
            dfs(rootMenu, root_item);
            TreeViewMenu.Items.Clear();
            TreeViewMenu.Items.Add(rootMenu);
            UpdateListView(current_item);
        }

        public void UpdateView()
        {
            InitiateView();
        }
        public void UpdateListView(CatalogItem dir)
        {
            List<File> filesList = new List<File>();
            if (dir.son != null)
            {
                CatalogItem son = dir.son;
                do
                {
                    filesList.Add(catalog_table.map(son));
                    son = son.next;
                } while (son != null);
            }
            ObservableCollection<File> filesView = new ObservableCollection<File>(filesList);
            FilesListView.DataContext = filesView;
            addressBar.Text = catalog_table.map(current_item).Path;
        }

        private void dfs(MenuItem item, CatalogItem node)
        {
            if (node.son != null)
            {
                CatalogItem son_node = node.son;
                do
                {
                    if (son_node.type == CatalogItem.FileType.FOLDER)
                    {
                        MenuItem son_item = new MenuItem() { Title = son_node.fileName};
                        dfs(son_item, son_node);
                        item.Items.Add(son_item);
                    }
                    son_node = son_node.next;
                } while (son_node != null);
            }
        }
        public void SerializeNow(object sender, CancelEventArgs e)
        {
            MessageBox.Show("bye!");

            FileStream fileStream = new FileStream(System.IO.Path.Combine(dir, "catalogTree.dat"), FileMode.Create);
            BinaryFormatter b = new BinaryFormatter();
            b.Serialize(fileStream, root_item);
            fileStream.Close();

            FileStream fileStream2 = new FileStream(System.IO.Path.Combine(dir, "catalogTable.dat"), FileMode.Create);
            b.Serialize(fileStream2, catalog_table);
            fileStream2.Close();

            FileStream fileStream3 = new FileStream(System.IO.Path.Combine(dir, "disk.dat"), FileMode.Create);
            b.Serialize(fileStream3, disk);
            fileStream3.Close();
            stateRecord.fileCount = CatalogItem.fileCount;
            
            FileStream fileStream4 = new FileStream(System.IO.Path.Combine(dir, "stateRecord.dat"), FileMode.Create);
            b.Serialize(fileStream4, stateRecord);
            fileStream4.Close();
        }
        public void SerializeNow()
        {
            // for init
            root_item.fileName = "root";
            File root_file = new File(root_item);
            root_file.Path = "root";
            catalog_table.map(root_item, root_file);

            FileStream fileStream = new FileStream(System.IO.Path.Combine(dir, "catalogTree.dat"), FileMode.Create);
            BinaryFormatter b = new BinaryFormatter();
            b.Serialize(fileStream, root_item);
            fileStream.Close();

            FileStream fileStream2 = new FileStream(System.IO.Path.Combine(dir, "catalogTable.dat"), FileMode.Create);
            b.Serialize(fileStream2, catalog_table);
            fileStream2.Close();

            FileStream fileStream3 = new FileStream(System.IO.Path.Combine(dir, "disk.dat"), FileMode.Create);
            b.Serialize(fileStream3, disk);
            fileStream3.Close();
            stateRecord.fileCount = CatalogItem.fileCount;

            FileStream fileStream4 = new FileStream(System.IO.Path.Combine(dir, "stateRecord.dat"), FileMode.Create);
            b.Serialize(fileStream4, stateRecord);
            fileStream4.Close();
        }
        public void DeSerializeNow()
        {
            FileStream fileStream = new FileStream(System.IO.Path.Combine(dir, "catalogTree.dat"), FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryFormatter b = new BinaryFormatter();
            root_item = b.Deserialize(fileStream) as CatalogItem;
            fileStream.Close();

            FileStream fileStream2 = new FileStream(System.IO.Path.Combine(dir, "catalogTable.dat"), FileMode.Open, FileAccess.Read, FileShare.Read);
            catalog_table = b.Deserialize(fileStream2) as CatalogTable;
            fileStream2.Close();

            FileStream fileStream3 = new FileStream(System.IO.Path.Combine(dir, "disk.dat"), FileMode.Open, FileAccess.Read, FileShare.Read);
            disk = b.Deserialize(fileStream3) as Disk;
            fileStream3.Close();

            FileStream fileStream4 = new FileStream(System.IO.Path.Combine(dir, "stateRecord.dat"), FileMode.Open, FileAccess.Read, FileShare.Read);
            stateRecord = b.Deserialize(fileStream4) as StateRecord;
            fileStream3.Close();
            CatalogItem.fileCount = stateRecord.fileCount;
        }

        
    }
}

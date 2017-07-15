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

namespace FileSystem
{
    /// <summary>
    /// Interaction logic for textEditorWindow.xaml
    /// </summary>
    /// 
    public class helper
    {
        /// 
        public delegate void delgateResetParentMethod();
    }
    public partial class textEditorWindow : Window
    {
        public helper.delgateResetParentMethod CallBackMethod;

        public textEditorWindow()
        {
            InitializeComponent();
        }
        private File textFile;
        private Disk disk = MainWindow.disk;
        public textEditorWindow(File f)
        {
            InitializeComponent();
            textFile = f;
            ShowContent();
        }

        private void ShowContent()
        {
            IndexTable table = textFile.indexTablePointer;
            List<int> seq = table.fetchContent();
            string result = "";
            foreach (int i in seq)
            {
                result += disk.getDataBlock(i);
            }
            textArea.Text = result;
        }

        private void EditorWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Do you want to save the changes?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                writeBack();
            }
        }

        private void writeBack()
        {
            string s = textArea.Text;
            textFile.Size = (Math.Ceiling(s.Length / 8.0)).ToString() + " B";
            // free the old content
            List<int> seq = textFile.indexTablePointer.fetchContent();
            disk.deallocate(seq);
            IndexTable table = disk.writeDisk(s);
            if (table == null)
            {
                MessageBox.Show("The file is so large that it can't be restored.");
            }
            else { textFile.indexTablePointer = table; }

            // callback
            if (CallBackMethod != null)
            {
                this.CallBackMethod();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            writeBack();
        }
    }
}

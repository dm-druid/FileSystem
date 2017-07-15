using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace FileSystem
{
    /// <summary>
    /// Interaction logic for propertyWindow.xaml
    /// </summary>
    public partial class propertyWindow : Window
    {
        public propertyWindow()
        {
            InitializeComponent();
        }
        public propertyWindow(File f)
        {
            InitializeComponent();
            Name.Text = f.Name;
            CreateTime.Text = f.CreatedTime;
            Path.Text = f.Path;
            Type.Text = f.Type;
            Size.Text = f.Size;
            
            if (f.Type == "TXT") {
                FileImage.Source = new BitmapImage(new Uri("Resource/textFile.ico", UriKind.Relative));
            }
            else{
                FileImage.Source = new BitmapImage(new Uri("Resource/folder.ico", UriKind.Relative));

            }
        }
    }
}

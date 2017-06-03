using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;


namespace FileSystem
{

    public class MenuItem
    {
        public MenuItem()
        {
            this.Items = new ObservableCollection<MenuItem>();
        }
        public string Title { get; set; }
        public ObservableCollection<MenuItem> Items { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystem
{
    [Serializable]
    public class CatalogTable
    {
        private Dictionary<int, File> table = new Dictionary<int, File>();
        private Dictionary<int, CatalogItem> reverse_table = new Dictionary<int, CatalogItem>();
        public File map(CatalogItem item)
        {
            int key = item.filePointer;
            if (table.ContainsKey(key))
            {
                return table[item.filePointer];
            }
            else return null;
        }
        public void map(CatalogItem item, File f)
        {
            int key = item.filePointer;
            table[key] = f;
            reverse_table[key] = item;
        }
        public CatalogItem getCatelogItem(File f)
        {
            int key = f.filePointer;
            return reverse_table[key];
        }
        public void remove(CatalogItem item)
        {
            table.Remove(item.filePointer);
        }
    }
}

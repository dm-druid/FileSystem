
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystem
{
    [Serializable]
    public class CatalogItem
    {
        public enum FileType { FOLDER, TXT };
        public string fileName;
        public int filePointer;
        public FileType type = FileType.FOLDER;
        public CatalogItem son { get; set; }
        public CatalogItem next { get; set; }
        public CatalogItem pred { get; set; }
        public CatalogItem father { get; set; }


        public void addSon(CatalogItem item)
        {
            if (this.son == null)
            {
                son = item;
                item.father = this;
            }
            else
            {
                CatalogItem t = son;
                while (t.next != null) { t = t.next; }
                t.next = item;
                item.pred = t;
            }
        }
        public static int fileCount = 0;
        public CatalogItem(string name, FileType t = FileType.FOLDER)
        {
            fileName = name;
            type = t;
            filePointer = fileCount;
            fileCount++;
        }
        public CatalogItem()
        {
            filePointer = fileCount;
            fileCount++;
        }
        public CatalogItem alias()
        {
            CatalogItem cata = new CatalogItem();
            //if (type == FileType.FOLDER) { cata.fileName = fileName + "_Alias"; }
            //else { cata.fileName = fileName.Split('.')[0] + "_Alias.TXT"; }
            cata.type = type;
            cata.filePointer = filePointer;
            return cata;
        }
        public CatalogItem copy()
        {
            CatalogItem cata = new CatalogItem();
            if (type == FileType.FOLDER) { cata.fileName = fileName + "_Copy"; }
            else { cata.fileName = fileName.Split('.')[0] + "_Copy.TXT"; }
            cata.type = type;
            return cata;
        }
        public void remove()
        {
            if (father != null) { father.son = next; }
            else if (pred != null) { pred.next = next; }
        }
    }

    [Serializable]
    public class StateRecord
    {
        public int fileCount = 0;
    }
}

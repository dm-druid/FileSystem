using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml;
using System.IO;

namespace FileSystem
{
    
    [Serializable]
    public class File // Denotes the FCB(File Control Block)
    {
        public string Name { get; set; }
        public string Size { get; set; }
        public string CreatedTime {
            get { return created_time.ToString(System.Globalization.CultureInfo.InvariantCulture); }
            set { created_time = DateTime.Parse(value); }
        } 
        public string Type { get; set; }
        public string Path;
        public int filePointer; // point to the catalog
        public IndexTable indexTablePointer; // point to the index table
        public List<int> fetchContent()
        {
            return indexTablePointer.fetchContent();
        }
        private DateTime created_time { get; set; }
        public File(string name, string size)
        {
            Name = name;
            Size = size;
            created_time = DateTime.Now;
            indexTablePointer = new IndexTable();
        }
        public File(CatalogItem item, string fatherPath = "")
        {
            Name = item.fileName;
            Size = "0";
            created_time = DateTime.Now;
            Type = item.type.ToString();
            Path = fatherPath + '/' + Name;
            filePointer = item.filePointer;
            indexTablePointer = new IndexTable();
        }
        public File copy(CatalogItem item)
        {
            File f = new File(item);
            f.Name = item.fileName.Split('.')[0] + "_Copy.TXT";
            f.Size = Size;
            f.created_time = created_time;
            f.Type = Type;
            f.Path = Path;
            f.filePointer = item.filePointer;
            f.indexTablePointer = new IndexTable();
            return f;
        }

    }
    
}

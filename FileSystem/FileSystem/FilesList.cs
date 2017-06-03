using System;

namespace FileSystem
{
    [Serializable]
    public class File
    {
        public string Name { get; set; }
        public string Size { get; set; }
        public string CreatedTime {
            get { return created_time.ToString(System.Globalization.CultureInfo.InvariantCulture); }
            set { created_time = DateTime.Parse(value); }
        }
        private DateTime created_time { get; set; }
        public File(string name, string size)
        {
            Name = name;
            Size = size;
            created_time = DateTime.Now;
        }
    }
}

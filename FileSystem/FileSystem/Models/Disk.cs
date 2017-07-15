using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystem
{
    [Serializable]
    public class Disk
    {
        static int diskCapcity = 10 * 1024;
        private DiskBlock[] diskData = new DiskBlock[diskCapcity];
        private bool[] bitMap = new bool[diskCapcity];
        private int p = 0; // record the disk block that has just been allocated
        public Disk()
        {
            for (int i = 0; i < diskCapcity; ++i)
            {
                bitMap[i] = true; // all the block is empty
            }
            p = 0;
        }
        public string getDataBlock(int index)
        {
            return diskData[index].getData();
        }
        public int allocate(string s)
        {
            p = p % diskCapcity;
            int pp = p;
            do
            {
                if (bitMap[pp])
                {
                    diskData[pp] = new DiskBlock();
                    diskData[pp].setData(s);
                    bitMap[pp] = false;
                    p = pp + 1;
                    return pp;
                }
                pp = (pp + 1) % diskCapcity;
            } while (pp != p);
            return -1;
        }
        public void deallocate(List<int> seq)
        {
            //if (seq.Count > 0) { p = seq[0]; }
            foreach (int i in seq)
            {
                bitMap[i] = true;
            }
        }
        public IndexTable writeDisk(string s)
        {
            IndexTable table = new IndexTable();
            while (s.Count() > 16)
            {
                int index = allocate(s.Substring(0, 15));
                if (!table.add(index))
                {
                    // MARK: remember to deallocate
                    deallocate(table.fetchContent());
                    return null;
                }
                s = s.Remove(0, 15);
            }
            if (s.Count() > 0)
            {
                int index = allocate(s);
                if (!table.add(index))
                {
                    deallocate(table.fetchContent());
                    return null;
                }
            }
            return table;
        }
    }


    [Serializable]
    public class DiskBlock
    {
        private char[] data;
        private int l; // length
        public DiskBlock()
        {
            data = new char[16];
        }
        public String getData()
        {
            string ans = "";
            for (int i = 0; i < l; ++i)
            {
                ans += data[i];
            }
            return ans;
        }
        public void setData(string s)
        {
            l = (s.Length < 16) ? s.Length : 16;
            for (int i = 0; i < l; ++i)
            {
                data[i] = s[i];
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystem
{
    [Serializable]
    public class IndexTable
    {
        private int[] index = new int[10];
        private int indexUsing; // begin write 
        private SubDirectIndex firstIndex; // 256
        private SubIndirectIndex secondIndex; // 256*256
        [Serializable]
        private class SubDirectIndex
        { // sub index for direct mapping
            public int[] index;
            public int indexUsing;
            public SubDirectIndex()
            {
                index = new int[256];
                indexUsing = 0;
            }
            public bool notFull()
            {
                return indexUsing < 256;
            }
            public void add(int i)
            {
                index[indexUsing] = i;
                indexUsing++;
            }
        } // 256
        [Serializable]
        private class SubIndirectIndex
        {
            public List<SubDirectIndex> index;
            public int indexUsing; // <256
            public SubIndirectIndex()
            {
                index = new List<SubDirectIndex>();
                index.Add(new SubDirectIndex());
                indexUsing = 0;
            }
            public void add(int i)
            {
                SubDirectIndex sub = index[indexUsing];
                if (sub.notFull())
                {
                    sub.add(i);
                    if (!sub.notFull())
                    {
                        index.Add(new SubDirectIndex());
                        indexUsing++;
                    }
                }
            }
            public bool notFull()
            {
                return indexUsing < 256;
            }
        }
        public IndexTable()
        {
            for (int i = 0; i < 10; ++i)
            {
                index[i] = -1;
            }
            indexUsing = 0;
        }
        public bool add(int i)
        {
            if (indexUsing < 10)
            {
                index[indexUsing] = i;
                indexUsing++;
                if (indexUsing == 10) { firstIndex = new SubDirectIndex(); }
            }
            else if (firstIndex.notFull())
            {
                firstIndex.add(i);
                if (!firstIndex.notFull()) { secondIndex = new SubIndirectIndex(); }
            }
            else if (secondIndex.notFull())
            {
                secondIndex.add(i);
            }
            else { return false; }
            return true;
        }

        public List<int> fetchContent()
        {
            List<int> ans = new List<int>();
            for (int i = 0; i < indexUsing; ++i)
            {
                int block = index[i];
                ans.Add(block);
            }
            if (indexUsing == 10)
            {
                for (int i = 0; i < firstIndex.indexUsing; ++i)
                {
                    int block = firstIndex.index[i];
                    ans.Add(block);
                }
            }
            if (firstIndex != null && !firstIndex.notFull())
            {
                foreach (SubDirectIndex first in secondIndex.index)
                {
                    for (int i = 0; i < first.indexUsing; ++i)
                    {
                        int block = first.index[i];
                        ans.Add(block);
                    }
                }
            }
            return ans;
        }
    }

}

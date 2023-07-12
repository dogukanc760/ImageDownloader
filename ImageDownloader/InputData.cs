using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageDownloader
{
    public class InputData
    {
        public int Count { get; }
        public int Parallelism { get; }
        public string SavePath { get; }

        public InputData(int count, int parallelism, string savePath)
        {
            Count = count;
            Parallelism = parallelism;
            SavePath = savePath;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageDownloader
{
    public class ProgressEventArgs : EventArgs
    {
        public int CurrentProgress { get; }
        public int TotalCount { get; }

        public ProgressEventArgs(int currentProgress, int totalCount)
        {
            CurrentProgress = currentProgress;
            TotalCount = totalCount;
        }
    }
}

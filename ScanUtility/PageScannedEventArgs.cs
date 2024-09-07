using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanUtility
{
    public class PageScannedEventArgs : EventArgs
    {
        public string ImageFilePath { get; }

        public PageScannedEventArgs(string imageFilePath)
        {
            ImageFilePath = imageFilePath;
        }
    }
}

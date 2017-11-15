using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PhotoImport
{
    public interface IImageDiscovery
    {
        Dictionary<DirectoryInfo, DateTime> GetImageDirectories(string baseDirectory);
    }

    public class ImageDiscovery : IImageDiscovery
    {
        public ImageDiscovery()
        {
        }

        public Dictionary<DirectoryInfo, DateTime> GetImageDirectories(string baseDirectory)
        {
            DirectoryInfo di = new DirectoryInfo(baseDirectory);
            var directories = di.GetDirectories();
            return directories.Select(x => new KeyValuePair<DirectoryInfo, DateTime>(x, x.LastAccessTime)).ToDictionary(x => x.Key, x => x.Value);
        }
    }
}
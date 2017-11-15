using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PhotoImport
{
    public interface IImageDiscovery
    {
        List<string> GetImageDirectories(string baseDirectory);
    }

    public class ImageDiscovery : IImageDiscovery
    {
        public ImageDiscovery()
        {
        }

        public List<string> GetImageDirectories(string baseDirectory)
        {
            DirectoryInfo di = new DirectoryInfo(baseDirectory);
            var directories = di.GetDirectories();
            return directories.Select(x => x.FullName).ToList();
        }
    }
}
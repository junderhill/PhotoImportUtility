using System;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("PhotoImport.Tests")]
namespace PhotoImport
{
    class Program
    {
        static void Main(string[] args)
        {
            bool overwriteFileName = !args.Contains("--no-rename");

            string source = args.Single(s => s.StartsWith("--source:'"));
            source = source.Substring(source.IndexOf('\'')).Replace("\'", "");
            
            string dest = args.Single(s => s.StartsWith("--dest:'"));
            dest = dest.Substring(dest.IndexOf('\'')).Replace("\'", "");
            
            PhotoImport import = new PhotoImport(new UserIOWrapper(), new UserRespondsPositively(), new ImageDiscovery());
            import.OverwriteOutputFilenames = overwriteFileName;
            import.Run(source,dest);
        }

    }
}
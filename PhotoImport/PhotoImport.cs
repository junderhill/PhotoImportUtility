using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace PhotoImport
{
    public class PhotoImport
    {
        private readonly IUserIO _userIo;
        private readonly IUserRespondsPositively _userRespondsPositively;
        private readonly IImageDiscovery _imageDiscovery;
        private static readonly string[] SUB_DIRECTORIES = {"RAW", "Processed RAW", "Edits"};
        private readonly string _directorySeperator;
        private PerformanceTracker _performanceStats;

        public bool OverwriteOutputFilenames { get; set; }
        
        public PhotoImport(IUserIO userIO, IUserRespondsPositively userRespondsPositively, IImageDiscovery imageDiscovery)
        {
            _userIo = userIO;
            _userRespondsPositively = userRespondsPositively;
            _imageDiscovery = imageDiscovery;
            _directorySeperator = "/";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                _directorySeperator = "\\";
            }

            _performanceStats = new PerformanceTracker();
        }

        public void Run(string sourceDirectory, string targetDirectory)
        {
            _performanceStats.StartProcessingTimer();
            var imageDirectories = _imageDiscovery.GetImageDirectories(sourceDirectory);
            _performanceStats.TotalSourceDirectories = imageDirectories.Count;
            foreach (var imageDirectory in imageDirectories)
            {
                ProcessImageDirectory(imageDirectory, targetDirectory);
            }
            _performanceStats.StopProcessingTimer();
            _performanceStats.Report();
        }

        internal void ProcessImageDirectory(KeyValuePair<DirectoryInfo, DateTime> imageDirectory, string targetDirectory)
        {
            if (_userRespondsPositively.ToQuestion($"Do you want to process {imageDirectory.Key}? (Y/N)"))
            {
                var directoryDescription = GetADescriptionForTheDirectoryFromUser();
                var destinationFolder =
                    GenerateDestinationFolderName(targetDirectory, imageDirectory, directoryDescription);


                if (Directory.Exists(destinationFolder))
                {
                    _userIo.WriteLine($"Directory {destinationFolder} already exists - ignoring");
                    return;
                }

                Directory.CreateDirectory(destinationFolder);
                var subDirectories = CreateSubDirectories(destinationFolder);

                CopyAllImagesToRawDirectory(imageDirectory.Key, subDirectories[0]);

            }
            else
            {
                _performanceStats.SourceDirectoriesIgnoredByUser += 1;
            }
        }

        private void CopyAllImagesToRawDirectory(DirectoryInfo sourceDirectoryInfo, DirectoryInfo rawOutputDirectory)
        {
            var files = sourceDirectoryInfo.GetFiles();
            int counter = 1;
            _performanceStats.StartCopyTimer();
            foreach (var f in files)
            {
                string outputfilename = f.Name;
                if (OverwriteOutputFilenames)
                    outputfilename = $"{rawOutputDirectory.Parent.Name.Replace(" ", "_")}_{counter:###}{f.Extension}";

                string outputFullPath = $"{rawOutputDirectory.FullName}{_directorySeperator}{outputfilename}";
               
                _userIo.WriteLine($"Copying: {f.FullName} to {outputFullPath}");
                f.CopyTo(outputFullPath);

                _performanceStats.TotalFilesCopied += 1;
                _performanceStats.TotalBytesCopied += f.Length;
                
                counter++;
            }
            _performanceStats.StopCopyTimer();
            
        }

        private List<DirectoryInfo> CreateSubDirectories(string destinationFolder)
        {
            var subdirectories = SUB_DIRECTORIES;
            var directories = new List<DirectoryInfo>();
            
            foreach (var s in subdirectories)
            {
                var d = Directory.CreateDirectory($"{destinationFolder}{_directorySeperator}{s}");
                directories.Add(d);
            }
            return directories;
        }

        private string GenerateDestinationFolderName(string targetDirectory, KeyValuePair<DirectoryInfo, DateTime> imageDirectory, string directoryDescription)
        {
            if (!targetDirectory.EndsWith(_directorySeperator))
            {
                targetDirectory = targetDirectory + _directorySeperator;
            }

            var year = imageDirectory.Value.Year;
            var datedFolderName = imageDirectory.Value.ToString("yyyyMMdd");

            var fullPath =
                $"{targetDirectory}{year}{_directorySeperator}{datedFolderName}";

            if (!string.IsNullOrWhiteSpace(directoryDescription))
            {
                fullPath += $" {directoryDescription}";
            }
            
            return fullPath;
        }

        private string GetADescriptionForTheDirectoryFromUser()
        {
            _userIo.WriteLine("Please enter a description for the directory:");
            return _userIo.ReadLine();
        }
    }

    public interface IUserRespondsPositively
    {
        bool ToQuestion(string question);
    }
}
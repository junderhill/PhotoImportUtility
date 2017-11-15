namespace PhotoImport
{
    public class PhotoImport
    {
        private readonly IUserRespondsPositively _userRespondsPositively;
        private readonly IImageDiscovery _imageDiscovery;

        public PhotoImport(IUserRespondsPositively userRespondsPositively, IImageDiscovery imageDiscovery)
        {
            _userRespondsPositively = userRespondsPositively;
            _imageDiscovery = imageDiscovery;
        }

        public void Run(string baseDirectory)
        {
            var imageDirectories = _imageDiscovery.GetImageDirectories(baseDirectory);
            foreach (var imageDirectory in imageDirectories)
            {
                ProcessImageDirectory(imageDirectory);
            }
        }

        private void ProcessImageDirectory(string imageDirectory)
        {
            if (_userRespondsPositively.ToQuestion($"Do you want to process {imageDirectory}? (Y/N)"))
            {
                
            }
        }
    }

    public interface IUserRespondsPositively
    {
        bool ToQuestion(string question);
    }
}
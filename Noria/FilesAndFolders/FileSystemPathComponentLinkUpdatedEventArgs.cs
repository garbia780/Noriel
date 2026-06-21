namespace Noria.FilesAndFolders
{
    public class FileSystemPathComponentLinkUpdatedEventArgs
    {
        public string Path { get; private set; }

        public FileSystemPathComponentLinkUpdatedEventArgs(string path)
        {
            Path = path;
        }
    }
}
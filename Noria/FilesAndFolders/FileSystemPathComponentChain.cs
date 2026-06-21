using Noria.ViewModel;
using System;
using System.Runtime.CompilerServices;
using System.Security.RightsManagement;
using System.Windows.Markup;
using System.Windows.Media.TextFormatting;

namespace Noria.FilesAndFolders
{
    public class FileSystemPathComponentChainViewItemProvider : IFileSystemViewItemProvider
    {
        private object _syncRoot = new object();
        private FileSystemPathComponentChain _pathComponentChain;
        public FileSystemPathComponentChain PathComponentChain
        {
            get
            {
                lock (_syncRoot)
                {
                    return _pathComponentChain;
                }
            }

            set
            {
                lock (_syncRoot)
                {
                    _pathComponentChain = value;
                }
            }
        }

        public IFileSystemViewItem GetFileSystemViewItem(string path)
        {
            if (PathComponentChain == null)
                return null;

            var link = PathComponentChain.FirstLink;
            while (link != null)
            {
                if (link.PathComponent.Path == path)
                    return link;

                link = link.NextLink;
            }

            return null;
        }

    }
    public class FileSystemPathComponentChain
    {
        public FileSystemPathComponentLink FirstLink { get; private set; }

        public FileSystemPathComponentChain(string path)
        {
            var fileSystemPath = new FileSystemPath(path);

            var link = (FileSystemPathComponentLink)null;
            for (int i = fileSystemPath.PathComponents.Length - 1; i >= 0; i--)
            {
                link = new FileSystemPathComponentLink(
                    fileSystemPath.PathComponents[i],
                    link);

                if (i == fileSystemPath.PathComponents.Length - 1)
                {
                    link.Updated += (sender, e) =>
                    {
                        OnLastLinkUpdated((FileSystemPathComponentLink)sender);
                    };

                    link.Deleted += (sender, e) =>
                    {
                        OnLastLinkDeleted((FileSystemPathComponentLink)sender);
                    };

                }
            }

            FirstLink = link;
        }

        public event EventHandler<FileSystemPathComponentLinkUpdatedEventArgs> Updated;
        public event EventHandler<FileSystemPathComponentLinkDeletedEventArgs> Deleted;
        private void OnLastLinkUpdated(FileSystemPathComponentLink pathComponentLink)
        {
            var e = new FileSystemPathComponentLinkUpdatedEventArgs(
                pathComponentLink.PathComponent.Path);

            Updated?.Invoke(this, e);
        }

        private void OnLastLinkDeleted(FileSystemPathComponentLink lastLinkDeleted)
        {
            var e = new FileSystemPathComponentLinkDeletedEventArgs(
                lastLinkDeleted.FileSystemItemPath);

            Deleted?.Invoke(this, e);
        }

        public FileSystemPathComponentLink GetLastLink()
        {
            FileSystemPathComponentLink link = FirstLink;

            while (link.NextLink != null)
            {
                link = link.NextLink;
            }

            return link;
        }
    }



    public class FileSystemPathComponentLinkDeletedEventArgs : EventArgs
    {
        string path;

        public string Path
        {
            get { return path; }
        }

        public FileSystemPathComponentLinkDeletedEventArgs(string path)
        {
            this.path = path;   
        }
    }
}
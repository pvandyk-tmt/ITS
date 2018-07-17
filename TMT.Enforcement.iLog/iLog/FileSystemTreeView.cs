using System.Collections;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using C2C.FileSystem;

namespace TMT.Enforcement.iLog
{
    /// <summary>
    /// Summary description for DirectoryTreeView.
    /// </summary> 
    ///    
    public class FileSystemTreeView : TreeView
    {
        #region Delegates

        public delegate void FolderSelectedDelegate(string path);

        #endregion

        public static readonly int Folder;
        private ImageList _imageList = new ImageList();
        private bool _showFiles = true;
        private Hashtable _systemIcons = new Hashtable();

        public FileSystemTreeView()
        {
            ImageList = _imageList;
            MouseDown += FileSystemTreeView_MouseDown;
            BeforeExpand += FileSystemTreeView_BeforeExpand;
        }

        public bool ShowFiles
        {
            get { return _showFiles; }
            set { _showFiles = value; }
        }

        public event FolderSelectedDelegate FolderSelected;

        private void FileSystemTreeView_MouseDown(object sender, MouseEventArgs e)
        {
            TreeNode node = GetNodeAt(e.X, e.Y);

            if (node == null)
                return;

            SelectedNode = node; //select the node under the mouse  

            if (FolderSelected != null)
            {
                FolderSelected(SelectedNode.FullPath);
            }
        }

        private void FileSystemTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node is FileNode) return;

            DirectoryNode node = (DirectoryNode) e.Node;

            if (!node.Loaded)
            {
                node.Nodes[0].Remove(); //remove the fake child node used for virtualization
                node.LoadDirectory();
                if (_showFiles)
                    node.LoadFiles();
            }
        }

        public void Load(string directoryPath)
        {
            if (Directory.Exists(directoryPath) == false)
                throw new DirectoryNotFoundException("Directory Not Found");

            _systemIcons.Clear();
            _imageList.Images.Clear();
            Nodes.Clear();

            Icon folderIcon = TMT.Enforcement.iLog.Properties.Resources.folder;

            _imageList.Images.Add(folderIcon);
            _systemIcons.Add(FileSystemTreeView.Folder, 0);

            DirectoryNode node = new DirectoryNode(this, directoryPath);
            node.Expand();
        }

        public int GetIconImageIndex(string path)
        {
            string extension = Path.GetExtension(path);

            if (_systemIcons.ContainsKey(extension) == false)
            {
                Icon icon = ShellIcon.GetSmallIcon(path);
                _imageList.Images.Add(icon);
                _systemIcons.Add(extension, _imageList.Images.Count - 1);
            }

            return (int) _systemIcons[Path.GetExtension(path)];
        }
    }

    public class DirectoryNode : TreeNode
    {
        private readonly string _directory;
        private UserFileAccessRights _rights;

        public DirectoryNode(DirectoryNode parent, string directory)
            : base(directory)
        {
            _rights = new UserFileAccessRights(directory);
            if (_rights.canWrite() && _rights.canRead())
            {
                //"R/W access";
                _directory = directory;
                ForeColor = Color.DarkGreen;
            }
            else
            {
                if (_rights.canWrite())
                {
                    //"Only Write access";
                    _directory = directory;
                    ForeColor = Color.DarkBlue;
                }
                else if (_rights.canRead())
                {
                    //"Only Read access";
                    _directory = directory;
                    ForeColor = Color.DarkRed;
                }
                else
                {
                    //No Rights
                    return;
                }
            }

            DirectoryInfo dir = new DirectoryInfo(_directory);
            this.Text = dir.Name;
            this.FullPath = _directory;

            ImageIndex = FileSystemTreeView.Folder;
            SelectedImageIndex = ImageIndex;
            parent.Nodes.Add(this);
            Virtualize();
        }

        public DirectoryNode(FileSystemTreeView treeView, string directory) : base(directory)
        {
            _rights = new UserFileAccessRights(directory);
            if (_rights.canWrite() && _rights.canRead())
            {
                //"R/W access";
                _directory = directory;
                ForeColor = Color.DarkGreen;
            }
            else
            {
                if (_rights.canWrite())
                {
                    //"Only Write access";
                    _directory = directory;
                    ForeColor = Color.DarkBlue;
                }
                else if (_rights.canRead())
                {
                    //"Only Read access";
                    _directory = directory;
                    ForeColor = Color.DarkRed;
                }
                else
                {
                    //No Rights
                    return;
                }
            }

            DirectoryInfo dir = new DirectoryInfo(_directory);
            this.Text = dir.Name;
            this.FullPath = _directory;

            ImageIndex = FileSystemTreeView.Folder;
            SelectedImageIndex = ImageIndex;
            treeView.Nodes.Add(this);
            Virtualize();
        }

        public bool Loaded
        {
            get
            {
                if (Nodes.Count != 0)
                {
                    if (Nodes[0] is FakeChildNode)
                        return false;
                }

                return true;
            }
        }

        public new FileSystemTreeView TreeView
        {
            get { return (FileSystemTreeView) base.TreeView; }
        }

        private void Virtualize()
        {
            int fileCount = 0;

            try
            {
                if (TreeView.ShowFiles)
                {
                    fileCount = Directory.GetFiles(_directory).Length;
                }

                if ((fileCount + Directory.GetDirectories(_directory).Length) > 0)
                {
                    new FakeChildNode(this);
                }
            }
            catch
            {
                return;
            }
        }

        public void LoadDirectory()
        {
            string[] dirs = Directory.GetDirectories(_directory);

            foreach (string dir in dirs)
            {
                if (!dir.ToUpper().Contains("AVGSPEED") && !dir.ToUpper().Contains("VOSI"))
                    new DirectoryNode(this, dir);
            }
        }

        public void LoadFiles()
        {
            string[] files = Directory.GetFiles(_directory);

            foreach (string file in files)
            {
                new FileNode(this, file);
            }
        }

        public UserFileAccessRights UserSecurity
        {
            get
            {
                return _rights;
            }
            internal set
            {
                _rights = value; 
            }
        }

        public new string FullPath { get; internal set; }
    }

    public class FileNode : TreeNode
    {
        private readonly DirectoryNode _directoryNode;
        private readonly string _file;

        public FileNode(DirectoryNode directoryNode, string file)
            : base(file)
        {
            _directoryNode = directoryNode;
            _file = file;

            ImageIndex = (_directoryNode.TreeView).GetIconImageIndex(_file);
            SelectedImageIndex = ImageIndex;

            _directoryNode.Nodes.Add(this);
        }
    }

    public class FakeChildNode : TreeNode
    {
        public FakeChildNode(TreeNode parent)
        {
            parent.Nodes.Add(this);
        }
    }
}
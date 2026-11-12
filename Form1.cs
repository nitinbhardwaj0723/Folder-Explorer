
using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

namespace FolderExplorer
{
    public partial class Form1 : Form
    {
        TreeView tree = new TreeView();
        ListView list = new ListView();

        public Form1()
        {
            InitializeComponent();

            this.Text = "My Folder Explorer";
            this.Size = new Size(850, 500);

            tree.Dock = DockStyle.Left;
            tree.Width = 250;
            tree.BeforeExpand += ExpandFolder;
            tree.AfterSelect += ShowFiles;

            list.Dock = DockStyle.Fill;
            list.View = View.Details;
            list.FullRowSelect = true;

            list.Columns.Add("Name", 220);
            list.Columns.Add("Size", 100);
            list.Columns.Add("Date Modified", 180);

            this.Controls.Add(list);
            this.Controls.Add(tree);

            foreach (DriveInfo d in DriveInfo.GetDrives())
            {
                if (d.IsReady)
                {
                    TreeNode node = tree.Nodes.Add(d.Name);
                    node.Tag = d.Name;
                    node.Nodes.Add("Loading");
                }
            }
        }

        void ExpandFolder(object sender, TreeViewCancelEventArgs e)
        {
            TreeNode n = e.Node;

            if (n.Nodes.Count == 1 && n.Nodes[0].Tag == null)
            {
                n.Nodes.Clear();

                try
                {
                    foreach (string f in Directory.GetDirectories(n.Tag.ToString()))
                    {
                        TreeNode child = n.Nodes.Add(Path.GetFileName(f));
                        child.Tag = f;
                        child.Nodes.Add("Loading");
                    }
                }
                catch { }
            }
        }

        void ShowFiles(object sender, TreeViewEventArgs e)
        {
            list.Items.Clear();

            try
            {
                foreach (string f in Directory.GetFiles(e.Node.Tag.ToString()))
                {
                    FileInfo info = new FileInfo(f);

                    ListViewItem item = new ListViewItem(info.Name);
                    item.SubItems.Add((info.Length / 1024) + " KB");
                    item.SubItems.Add(info.LastWriteTime.ToString("dd-MM-yyyy"));

                    list.Items.Add(item);
                }

                foreach (string f in Directory.GetDirectories(e.Node.Tag.ToString()))
                {
                    DirectoryInfo info = new DirectoryInfo(f);

                    ListViewItem item = new ListViewItem("[Folder] " + info.Name);
                    item.SubItems.Add("-");
                    item.SubItems.Add(info.LastWriteTime.ToString("dd-MM-yyyy"));

                    list.Items.Add(item);
                }
            }
            catch
            {
                MessageBox.Show("Cannot access this folder!");
            }
        }
    }
}

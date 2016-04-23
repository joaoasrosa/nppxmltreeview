using System;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using NppPluginNET;
using NppXmlTreeviewPlugin.Parsers;
using NppXmlTreviewPlugin.Extensions;

namespace NppXmlTreeviewPlugin.Forms
{
    public partial class FormTreeView : Form
    {
        #region CONSTRUCTORS

        public FormTreeView()
        {
            InitializeComponent();

            // Get the values from the open document.
            UpdateUserInterface();
        }

        #endregion


        #region PUBLIC METHODS

        /// <summary>
        /// Updates the plugin user interface.
        /// </summary>
        public void UpdateUserInterface()
        {
            // Start the background worker.
            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.RunWorkerAsync();
        }

        #endregion


        #region PRIVATE METHODS

        /// <summary>
        /// Background worker method to do the async work.
        /// </summary>
        /// <param name="sender">The background worker object.</param>
        /// <param name="e">The event arguments.</param>
        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                // Clear the treeview.
                this.treeView.Invoke(new ClearNodesDelegate(ClearNodes));
                
                NppXmlNode node;

                // Do validations.
                if (!NppXmlNode.TryParse(GetDocumentText(PluginBase.GetCurrentScintilla()), out node))
                {
                    return;
                }

                this.treeView.Invoke(new AddNodeToTreeViewDelegate(AddNodeToTreeView),
                  new TreeNode(node.Name)
                  {
                      ToolTipText = $"{node.Name} [{node.StartPosition}:{node.EndPosition}]",
                      Tag = new TextBoundary(node.StartPosition, node.EndPosition)
                  });

                // Add the rest of the nodes.
                var treeNode = this.treeView.Nodes[0];
                AddNode(node, treeNode);

                // Finish up the operation.
                this.treeView.Invoke(new ExpandTreeViewDelegate(ExpandTreeView));
            }
            catch (Exception ex)
            {
                // TODO: log it somewhere
                Main.ErrorOut(ex);
                throw;
            }
        }

        /// <summary>
        /// Method to add a npp xml node to a tree node.
        /// </summary>
        /// <param name="inXmlNode">The npp xml node.</param>
        /// <param name="inTreeNode">The tree node.</param>
        private void AddNode(NppXmlNode inXmlNode, TreeNode inTreeNode)
        {
            if (!inXmlNode.HasChildNodes) { return; }
            for (var i = 0; i < inXmlNode.ChildNodes.Count; i++)
            {
                var xNode = inXmlNode.ChildNodes[i];
                
                this.treeView.Invoke(new AddNodeToNodeDelegate(AddNodeToNode),
                    new TreeNode(xNode.Name)
                    {
                        ToolTipText = $"{xNode.Name} [{xNode.StartPosition}:{xNode.EndPosition}]",
                        Tag = new TextBoundary(xNode.StartPosition, xNode.EndPosition)
                    }, inTreeNode);
                AddNode(xNode, inTreeNode.Nodes[i]);
            }
        }

        /// <summary>
        /// Method to clear the nodes of the tree view.
        /// </summary>
        private void ClearNodes()
        {
            this.treeView.Nodes.Clear();
        }

        /// <summary>
        /// Method to add a node to the tree view.
        /// </summary>
        /// <param name="treeNode">The node to add.</param>
        private void AddNodeToTreeView(TreeNode treeNode)
        {
            this.treeView.Nodes.Add(treeNode);
        }

        /// <summary>
        /// Method to add a tree node to a parent tree node.
        /// </summary>
        /// <param name="treeNode">The tree node to add.</param>
        /// <param name="parent">The parent tree node.</param>
        private void AddNodeToNode(TreeNode treeNode, TreeNode parent)
        {
            parent.Nodes.Add(treeNode);
        }

        /// <summary>
        /// Method to expand the tree view.
        /// </summary>
        private void ExpandTreeView()
        {
            this.treeView.ShowNodeToolTips = true;
            this.treeView.ExpandAll();
            this.treeView.Nodes[0].EnsureVisible();
        }

        /// <summary>
        /// Method to handle the tree node click.
        /// </summary>
        /// <param name="sender">The treeview.</param>
        /// <param name="e">The event arguments</param>
        private void treeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var textBoundary = (TextBoundary)e.Node.Tag;

            // Highlight the text.
            var currentScintilla = PluginBase.GetCurrentScintilla();
            Win32.SendMessage(currentScintilla, SciMsg.SCI_SETSELECTIONSTART, textBoundary.StartIndex, 0);
            Win32.SendMessage(currentScintilla, SciMsg.SCI_SETSELECTIONEND, textBoundary.EndIndex, 0);
            Win32.SendMessage(currentScintilla, SciMsg.SCI_SCROLLCARET, 0, 0);
        }

        #endregion


        #region PRIVATE STATIC METHODS

        /// <summary>
        /// Method to get the text from the Scintilla component.
        /// </summary>
        /// <param name="currentScintilla">The current Scintilla.</param>
        /// <returns>The document text.</returns>
        private static string GetDocumentText(IntPtr currentScintilla)
        {
            var length = (int)Win32.SendMessage(currentScintilla, SciMsg.SCI_GETLENGTH, 0, 0) + 1;
            var sb = new byte[length];

            unsafe
            {
                fixed (byte* p = sb)
                {
                    var ptr = (IntPtr)p;
                    Win32.SendMessage(currentScintilla, SciMsg.SCI_GETTEXT, length, ptr);
                }

                return Encoding.UTF8.GetString(sb).TrimEnd('\0');
            }
        }

        #endregion


        #region PRIVATE FIELDS

        private delegate void ClearNodesDelegate();
        private delegate void AddNodeToTreeViewDelegate(TreeNode treeNode);
        private delegate void AddNodeToNodeDelegate(TreeNode treeNode, TreeNode parent);
        private delegate void ExpandTreeViewDelegate();

        #endregion
    }
}

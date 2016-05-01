using System;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using NppPluginNET;
using NppXmlTreeviewPlugin.Parsers;
using NppXmlTreeviewPlugin.Properties;

namespace NppXmlTreeviewPlugin.Forms
{
    public partial class FormTreeView : Form
    {
        private bool _expanded;
        private bool _workerIsRunning;

        #region CONSTRUCTORS

        public FormTreeView()
        {
            InitializeComponent();

            this.TooltipButtonToogle.SetToolTip(this.ButtonToggle, "Collapse treeview");

            // Get the values from the open document.
            UpdateUserInterface();
        }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        ///     Updates the plugin user interface.
        /// </summary>
        public void UpdateUserInterface()
        {
            if (this._workerIsRunning)
            {
                return;
            }

            // Start the background worker.
            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += BackgroundWorker_UpdateUserInterfaceDoWork;
            backgroundWorker.RunWorkerAsync();
            this._workerIsRunning = true;
        }

        #endregion

        #region PRIVATE METHODS

        /// <summary>
        ///     Background worker method to do the user interface updated.
        /// </summary>
        /// <param name="sender">The background worker object.</param>
        /// <param name="e">The event arguments.</param>
        private void BackgroundWorker_UpdateUserInterfaceDoWork(object sender, DoWorkEventArgs e)
        {
            if (this.treeView.InvokeRequired)
            {
                this.treeView.Invoke(new ClearNodesDelegate(ClearNodes));
            }
            else
            {
                ClearNodes();
            }

            NppXmlNode node;

            // Do validations.
            if (!NppXmlNode.TryParse(GetDocumentText(PluginBase.GetCurrentScintilla()), out node))
            {
                this._workerIsRunning = false;
                return;
            }

            if (this.treeView.InvokeRequired)
            {
                this.treeView.Invoke(new AddNodeToTreeViewDelegate(AddNodeToTreeView), GenerateTreeNode(node));
            }
            else
            {
                this.treeView.Nodes.Add(GenerateTreeNode(node));
            }

            // Add the rest of the nodes.
            var treeNode = this.treeView.Nodes[0];
            AddNode(node, treeNode);

            // Finish up the operation.
            if (this.treeView.InvokeRequired)
            {
                this.treeView.Invoke(new ExpandTreeViewDelegate(ExpandTreeView));
            }
            else
            {
                ExpandTreeView();
            }

            this._workerIsRunning = false;
        }

        /// <summary>
        ///     Background worker method to do the user interface updated.
        /// </summary>
        /// <param name="sender">The background worker object.</param>
        /// <param name="e">The event arguments.</param>
        private void BackgroundWorker_ToggleTreeViewDoWork(object sender, DoWorkEventArgs e)
        {
            if (this._expanded)
            {
                if (this.treeView.InvokeRequired)
                {
                    this.treeView.Invoke(new CollapseTreeViewDelegate(CollapseTreeView));
                }
                else
                {
                    CollapseTreeView();
                }
            }
            else
            {
                if (this.treeView.InvokeRequired)
                {
                    this.treeView.Invoke(new ExpandTreeViewDelegate(ExpandTreeView));
                }
                else
                {
                    ExpandTreeView();
                }
            }

            this._workerIsRunning = false;
        }

        /// <summary>
        ///     Clear the treeview nodes.
        /// </summary>
        private void ClearNodes()
        {
            this.treeView.Nodes.Clear();
        }

        /// <summary>
        ///     Method to add a npp xml node to a tree node.
        /// </summary>
        /// <param name="inXmlNode">The npp xml node.</param>
        /// <param name="inTreeNode">The tree node.</param>
        private void AddNode(NppXmlNode inXmlNode, TreeNode inTreeNode)
        {
            if (!inXmlNode.HasChildNodes)
            {
                return;
            }
            for (var i = 0; i < inXmlNode.ChildNodes.Count; i++)
            {
                var xNode = inXmlNode.ChildNodes[i];

                if (this.treeView.InvokeRequired)
                {
                    this.treeView.Invoke(new AddNodeToNodeDelegate(AddNodeToNode), GenerateTreeNode(xNode), inTreeNode);
                }
                else
                {
                    AddNodeToNode(GenerateTreeNode(xNode), inTreeNode);
                }
                AddNode(xNode, inTreeNode.Nodes[i]);
            }
        }

        /// <summary>
        ///     Method to add a node to the tree view.
        /// </summary>
        /// <param name="treeNode">The node to add.</param>
        private void AddNodeToTreeView(TreeNode treeNode)
        {
            this.treeView.Nodes.Add(treeNode);
        }

        /// <summary>
        ///     Method to expand the tree view.
        /// </summary>
        private void ExpandTreeView()
        {
            this.treeView.ShowNodeToolTips = true;
            this.treeView.ExpandAll();
            this.treeView.Nodes[0].EnsureVisible();
            this._expanded = true;

            this.ButtonToggle.Image = Resources.toggle;
            this.TooltipButtonToogle.SetToolTip(this.ButtonToggle, "Collapse treeview");
        }

        /// <summary>
        ///     Method to collapse the tree view.
        /// </summary>
        private void CollapseTreeView()
        {
            this.treeView.ShowNodeToolTips = true;
            this.treeView.CollapseAll();
            this.treeView.Nodes[0].EnsureVisible();
            this._expanded = false;

            this.ButtonToggle.Image = Resources.toggle_expand;
            this.TooltipButtonToogle.SetToolTip(this.ButtonToggle, "Expand treeview");
        }

        /// <summary>
        ///     Method to handle the tree node click.
        /// </summary>
        /// <param name="sender">The treeview.</param>
        /// <param name="e">The event arguments.</param>
        private void TreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var textBoundary = (TextBoundary)e.Node.Tag;

            // Highlight the text.
            var currentScintilla = PluginBase.GetCurrentScintilla();

            var startLineStartPos = (int)Win32.SendMessage(currentScintilla, SciMsg.SCI_POSITIONFROMLINE,
                textBoundary.StartNodePosition.LineNumber, 0);
            var endLineStartPos = (int)Win32.SendMessage(currentScintilla, SciMsg.SCI_POSITIONFROMLINE,
                textBoundary.EndNodePosition.LineNumber, 0);

            Win32.SendMessage(currentScintilla, SciMsg.SCI_SETSELECTIONSTART,
                startLineStartPos + textBoundary.StartNodePosition.LinePosition, 0);
            Win32.SendMessage(currentScintilla, SciMsg.SCI_SETSELECTIONEND,
                endLineStartPos + textBoundary.EndNodePosition.LinePosition, 0);
            Win32.SendMessage(currentScintilla, SciMsg.SCI_SCROLLCARET, 0, 0);
        }

        /// <summary>
        ///     Method to handle the toggle button click.
        /// </summary>
        /// <param name="sender">The button.</param>
        /// <param name="e">The event arguments.</param>
        private void ButtonToggle_Click(object sender, EventArgs e)
        {
            if (this._workerIsRunning)
            {
                return;
            }

            // Start the background worker.
            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += BackgroundWorker_ToggleTreeViewDoWork;
            backgroundWorker.RunWorkerAsync();
            this._workerIsRunning = true;
        }

        #endregion

        #region PRIVATE STATIC METHODS

        /// <summary>
        ///     Method to add a tree node to a parent tree node.
        /// </summary>
        /// <param name="treeNode">The tree node to add.</param>
        /// <param name="parent">The parent tree node.</param>
        private static void AddNodeToNode(TreeNode treeNode, TreeNode parent)
        {
            parent.Nodes.Add(treeNode);
        }

        /// <summary>
        ///     Method to get the text from the Scintilla component.
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

        /// <summary>
        /// Method to genrate a treenode from a npp tree node.
        /// </summary>
        /// <param name="node">The npp tree node.</param>
        /// <returns>The tree node.</returns>
        private static TreeNode GenerateTreeNode(NppXmlNode node)
        {
            return new TreeNode(node.Name)
            {
                ToolTipText = $"{node.Name}",
                Tag = new TextBoundary(node.StartPosition, node.EndPosition)
            };
        }

        #endregion

        #region PRIVATE DELEGATES

        private delegate void ClearNodesDelegate();

        private delegate void AddNodeToTreeViewDelegate(TreeNode treeNode);

        private delegate void AddNodeToNodeDelegate(TreeNode treeNode, TreeNode parent);

        private delegate void ExpandTreeViewDelegate();

        private delegate void CollapseTreeViewDelegate();

        #endregion
    }
}
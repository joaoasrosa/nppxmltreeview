using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NppPluginNET;
using NppXmlTreeviewPlugin.Parsers;
using NppXmlTreeviewPlugin.Properties;

using Serilog;

namespace NppXmlTreeviewPlugin.Forms
{
    public partial class FormTreeView : Form
    {
        private bool _expanded;
        private bool _workerIsRunning;
        private NppXmlNode _rootNode;
        private readonly ILogger _logger;

        #region CONSTRUCTORS

        public FormTreeView()
        {
            InitializeComponent();

            _logger = new LoggerConfiguration()
                      .WriteTo.RollingFile("log-{Date}.txt")
                      .CreateLogger();

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


        public void SetNodeSelection()
        {
            if (this._workerIsRunning)
            {
                return;
            }

            // Start the background worker.
            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += BackgroundWorker_SetNodeSelectionDoWork;
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
            this._rootNode = null;

            if (this.ButtonToggle.InvokeRequired)
            {
                this.ButtonToggle.Invoke(new EnableToggleButtonDelegate(EnableToggleButton), false);
            }
            else
            {
                EnableToggleButton(false);
            }

            if (this.treeView.InvokeRequired)
            {
                this.treeView.Invoke(new SetTreeviewVisibilityDelegate(SetTreeviewVisibility), false);
            }
            else
            {
                SetTreeviewVisibility(false);
            }

            if (this.LabelStatus.InvokeRequired)
            {
                this.LabelStatus.Invoke(new SetStatusLabelTextDelegate(SetStatusLabelText), "Parsing the document");
            }
            else
            {
                SetStatusLabelText("Parsing the document");
            }

            string attributeName = attributeNameTextBox.Enabled ? attributeNameTextBox.Text : null;

            // Do validations.
            if (!NppXmlNode.TryParse(GetDocumentText(PluginBase.GetCurrentScintilla()), out this._rootNode, attributeName))
            {
                if (this.LabelStatus.InvokeRequired)
                {
                    this.LabelStatus.Invoke(new SetStatusLabelTextDelegate(SetStatusLabelText), "Document is not valid.");
                }
                else
                {
                    SetStatusLabelText("Document is not valid.");
                }

                this._workerIsRunning = false;
                return;
            }

            if (this.treeView.InvokeRequired)
            {
                this.treeView.Invoke(new ClearNodesDelegate(ClearNodes));
            }
            else
            {
                ClearNodes();
            }

            if (this.treeView.InvokeRequired)
            {
                this.treeView.Invoke(new AddNodeToTreeViewDelegate(AddNodeToTreeView), GenerateTreeNode(this._rootNode));
            }
            else
            {
                this.treeView.Nodes.Add(GenerateTreeNode(this._rootNode));
            }

            // Add the rest of the nodes.
            var treeNode = this.treeView.Nodes[0];
            AddNode(this._rootNode, treeNode);

            // Finish up the operation.
            if (this.treeView.InvokeRequired)
            {
                this.treeView.Invoke(new ExpandTreeViewDelegate(ExpandTreeView));
            }
            else
            {
                ExpandTreeView();
            }

            if (this.LabelStatus.InvokeRequired)
            {
                this.LabelStatus.Invoke(new SetStatusLabelTextDelegate(SetStatusLabelText), string.Empty);
            }
            else
            {
                SetStatusLabelText(string.Empty);
            }

            if (this.ButtonToggle.InvokeRequired)
            {
                this.ButtonToggle.Invoke(new EnableToggleButtonDelegate(EnableToggleButton), true);
            }
            else
            {
                EnableToggleButton(true);
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
        ///     Background worker method to do the user selects text.
        /// </summary>
        /// <param name="sender">The background worker object.</param>
        /// <param name="e">The event arguments.</param>
        private void BackgroundWorker_SetNodeSelectionDoWork(object sender, DoWorkEventArgs e)
        {
            // Highlight the text.
            var currentScintilla = PluginBase.GetCurrentScintilla();

            var selectionStartPos = (int)Win32.SendMessage(currentScintilla, SciMsg.SCI_GETSELECTIONSTART, 0, 0);
            var selectionStartLine =
                (int)Win32.SendMessage(currentScintilla, SciMsg.SCI_LINEFROMPOSITION, selectionStartPos, 0);
            var selectionStartPosInLine =
                (int)Win32.SendMessage(currentScintilla, SciMsg.SCI_GETCOLUMN, selectionStartPos, 0);

            var node = this._rootNode.FindNppXmlNodeByLine(selectionStartLine, selectionStartPosInLine);

            if (null != node)
            {
                if (this.treeView.InvokeRequired)
                {
                    this.treeView.Invoke(new SetTreeviewSelectionDelegate(SetTreeviewSelection), node.Id.ToString());
                }
                else
                {
                    SetTreeviewSelection(node.Id.ToString());
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
                var xNode = inXmlNode.ChildNodes.ElementAt(i);

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
            this.treeView.Visible = true;

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
        /// Method to set the label status text.
        /// </summary>
        /// <param name="text">The text.</param>
        private void SetStatusLabelText(string text)
        {
            this.LabelStatus.Text = text;
        }

        /// <summary>
        /// Method to set the label visibility.
        /// </summary>
        /// <param name="visible">The visibility.</param>
        private void SetTreeviewVisibility(bool visible)
        {
            this.treeView.Visible = visible;
        }

        /// <summary>
        /// Method to enable the toggle button.
        /// </summary>
        /// <param name="enable">Flag with the enable value.</param>
        private void EnableToggleButton(bool enable)
        {
            this.ButtonToggle.Enabled = enable;
        }

        /// <summary>
        /// Selects the treenode in the treeview.
        /// </summary>
        /// <param name="id">The node id</param>
        private void SetTreeviewSelection(string id)
        {
            this.treeView.SelectedNode = this.treeView.Nodes.Find(id, true)[0];
            this.treeView.Focus();
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

        private void tagNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            attributeToolStripMenuItem.Checked = false;
            tagNameToolStripMenuItem.Checked = true;
            attributeNameTextBox.Enabled = false;
            UpdateUserInterface();
        }

        private void attributeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            attributeToolStripMenuItem.Checked = true;
            tagNameToolStripMenuItem.Checked = false;
            attributeNameTextBox.Enabled = true;
            UpdateUserInterface();
        }

        private void attributeNameTextBox_TextChanged(object sender, EventArgs e)
        {
            UpdateUserInterface();
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
        /// Method to generate a treenode from a npp tree node.
        /// </summary>
        /// <param name="node">The npp tree node.</param>
        /// <returns>The tree node.</returns>
        private static TreeNode GenerateTreeNode(NppXmlNode node)
        {
            return new TreeNode
            {
                ToolTipText = $"{node.Name}",
                Tag = new TextBoundary(node.StartPosition, node.EndPosition, node.Id),
                Text = node.Name,
                // The name is the key.
                Name = node.Id.ToString()
            };
        }

        #endregion

        #region PRIVATE DELEGATES

        private delegate void ClearNodesDelegate();

        private delegate void AddNodeToTreeViewDelegate(TreeNode treeNode);

        private delegate void AddNodeToNodeDelegate(TreeNode treeNode, TreeNode parent);

        private delegate void ExpandTreeViewDelegate();

        private delegate void CollapseTreeViewDelegate();

        private delegate void SetTreeviewVisibilityDelegate(bool visible);

        private delegate void SetStatusLabelTextDelegate(string text);

        private delegate void EnableToggleButtonDelegate(bool enable);

        private delegate void SetTreeviewSelectionDelegate(string id);

        #endregion
    }
}
using System;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using NppPluginNET;
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
            // Restart the counter.
            this._lastPosition = 0;

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

                // Get the text.
                this._text = GetDocumentText(PluginBase.GetCurrentScintilla());

                // Do validations.
                if (!this._text.IsValidXml(out this._xmlDocument) || null == this._xmlDocument.DocumentElement)
                {
                    return;
                }

                // Add the base treenode.
                this.treeView.Invoke(new AddNodeToTreeViewDelegate(AddNodeToTreeView),
                    new TreeNode(this._xmlDocument.DocumentElement.Name)
                    {
                        ToolTipText = DoToolTip(this._xmlDocument.DocumentElement),
                        Tag = GetTextBoundary(this._xmlDocument.DocumentElement)
                    });

                // Add the rest of the nodes.
                var treeNode = this.treeView.Nodes[0];
                AddNode(this._xmlDocument.DocumentElement, treeNode);

                // Finish up the operation.
                this.treeView.Invoke(new ExpandTreeViewDelegate(ExpandTreeView));
            }
            catch
            {
                // TODO: log it somewhere
                throw;
            }
            finally
            {
                this._text = null;
                this._xmlDocument = null;
            }
        }

        /// <summary>
        /// Method to get the text boundary for the tree node.
        /// </summary>
        /// <param name="xmlNode">The xml element to do the text search</param>
        /// <returns>The text boundary.</returns>
        private TextBoundary GetTextBoundary(XmlNode xmlNode)
        {
            // Set the boundaries.
            var textBoundary = new TextBoundary
            {
                StartIndex = this._lastPosition + this._text.IndexOf('<'),
                EndIndex = this._lastPosition + this._text.IndexOf('>') + 1
            };

            // Sub string the text and assign the last position.
            this._text = this._text.Substring(textBoundary.EndIndex - this._lastPosition);
            this._lastPosition = textBoundary.EndIndex;

            // Return the information.
            return textBoundary;
        }

        /// <summary>
        /// Method to add a xml node to a tree node.
        /// </summary>
        /// <param name="inXmlNode">The xml node.</param>
        /// <param name="inTreeNode">The tree node.</param>
        private void AddNode(XmlNode inXmlNode, TreeNode inTreeNode)
        {
            if (!inXmlNode.HasChildNodes) { return; }
            for (var i = 0; i < inXmlNode.ChildNodes.Count; i++)
            {
                var xNode = inXmlNode.ChildNodes[i] as XmlElement;

                // If isn't a xml element, we do not add it.
                if (null == xNode) { continue; }
                this.treeView.Invoke(new AddNodeToNodeDelegate(AddNodeToNode),
                    new TreeNode(xNode.Name)
                    {
                        ToolTipText = DoToolTip(xNode),
                        Tag = GetTextBoundary(xNode)
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

                return System.Text.Encoding.UTF8.GetString(sb).TrimEnd('\0');
            }
        }

        /// <summary>
        /// Method to do the tooltip of a xml node.
        /// </summary>
        /// <param name="xNode">The xml node.</param>
        /// <returns>The xml node tooltip.</returns>
        private static string DoToolTip(XmlNode xNode)
        {
            var sb = new StringBuilder("<");
            sb.Append(xNode.Name);
            if (null != xNode.Attributes)
            {
                foreach (XmlAttribute attribute in xNode.Attributes)
                {
                    sb.AppendFormat(" {0}=\"{1}\"", attribute.Name, attribute.Value);
                }
            }
            sb.Append("/>");
            return sb.ToString();
        }

        #endregion


        #region PRIVATE FIELDS

        private XmlDocument _xmlDocument;
        private string _text;
        private int _lastPosition;
        private delegate void ClearNodesDelegate();
        private delegate void AddNodeToTreeViewDelegate(TreeNode treeNode);
        private delegate void AddNodeToNodeDelegate(TreeNode treeNode, TreeNode parent);
        private delegate void ExpandTreeViewDelegate();

        #endregion
    }
}

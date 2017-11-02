using System.ComponentModel;
using System.Windows.Forms;

namespace NppXmlTreeviewPlugin.Forms
{
    partial class FormTreeView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.treeView = new System.Windows.Forms.TreeView();
            this.LabelStatus = new System.Windows.Forms.Label();
            this.TooltipButtonToogle = new System.Windows.Forms.ToolTip(this.components);
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.ButtonToggle = new System.Windows.Forms.Button();
            this.nodeNameMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tagNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.attributeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.attributeNameTextBox = new System.Windows.Forms.TextBox();
            this.menuButton1 = new NppXmlTreeviewPlugin.Forms.MenuButton();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.nodeNameMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.treeView, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.LabelStatus, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(515, 506);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // treeView
            // 
            this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView.Location = new System.Drawing.Point(3, 47);
            this.treeView.Name = "treeView";
            this.treeView.Size = new System.Drawing.Size(509, 456);
            this.treeView.TabIndex = 2;
            this.treeView.Visible = false;
            this.treeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.TreeView_NodeMouseClick);
            // 
            // LabelStatus
            // 
            this.LabelStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.LabelStatus.AutoSize = true;
            this.LabelStatus.Location = new System.Drawing.Point(3, 25);
            this.LabelStatus.Margin = new System.Windows.Forms.Padding(3);
            this.LabelStatus.Name = "LabelStatus";
            this.LabelStatus.Size = new System.Drawing.Size(0, 16);
            this.LabelStatus.TabIndex = 3;
            this.LabelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.ButtonToggle);
            this.flowLayoutPanel1.Controls.Add(this.menuButton1);
            this.flowLayoutPanel1.Controls.Add(this.attributeNameTextBox);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(515, 22);
            this.flowLayoutPanel1.TabIndex = 4;
            // 
            // ButtonToggle
            // 
            this.ButtonToggle.BackColor = System.Drawing.Color.Transparent;
            this.ButtonToggle.FlatAppearance.BorderSize = 0;
            this.ButtonToggle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ButtonToggle.Image = global::NppXmlTreeviewPlugin.Properties.Resources.toggle;
            this.ButtonToggle.Location = new System.Drawing.Point(3, 3);
            this.ButtonToggle.Name = "ButtonToggle";
            this.ButtonToggle.Size = new System.Drawing.Size(16, 16);
            this.ButtonToggle.TabIndex = 0;
            this.ButtonToggle.UseVisualStyleBackColor = false;
            this.ButtonToggle.Click += new System.EventHandler(this.ButtonToggle_Click);
            // 
            // nodeNameMenu
            // 
            this.nodeNameMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tagNameToolStripMenuItem,
            this.attributeToolStripMenuItem});
            this.nodeNameMenu.Name = "nodeNameMenu";
            this.nodeNameMenu.Size = new System.Drawing.Size(129, 48);
            // 
            // tagNameToolStripMenuItem
            // 
            this.tagNameToolStripMenuItem.Checked = true;
            this.tagNameToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tagNameToolStripMenuItem.Name = "tagNameToolStripMenuItem";
            this.tagNameToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.tagNameToolStripMenuItem.Text = "Tag Name";
            this.tagNameToolStripMenuItem.Click += new System.EventHandler(this.tagNameToolStripMenuItem_Click);
            // 
            // attributeToolStripMenuItem
            // 
            this.attributeToolStripMenuItem.Name = "attributeToolStripMenuItem";
            this.attributeToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.attributeToolStripMenuItem.Text = "Attribute";
            this.attributeToolStripMenuItem.Click += new System.EventHandler(this.attributeToolStripMenuItem_Click);
            // 
            // attributeNameTextBox
            // 
            this.attributeNameTextBox.Enabled = false;
            this.attributeNameTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 5.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.attributeNameTextBox.Location = new System.Drawing.Point(68, 3);
            this.attributeNameTextBox.Name = "attributeNameTextBox";
            this.attributeNameTextBox.Size = new System.Drawing.Size(100, 16);
            this.attributeNameTextBox.TabIndex = 2;
            this.attributeNameTextBox.TextChanged += new System.EventHandler(this.attributeNameTextBox_TextChanged);
            // 
            // menuButton1
            // 
            this.menuButton1.FlatAppearance.BorderSize = 0;
            this.menuButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.menuButton1.Image = global::NppXmlTreeviewPlugin.Properties.Resources.node_name;
            this.menuButton1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.menuButton1.Location = new System.Drawing.Point(25, 3);
            this.menuButton1.Menu = this.nodeNameMenu;
            this.menuButton1.Name = "menuButton1";
            this.menuButton1.Size = new System.Drawing.Size(37, 16);
            this.menuButton1.TabIndex = 1;
            this.menuButton1.UseVisualStyleBackColor = true;
            // 
            // FormTreeView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(515, 506);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "FormTreeView";
            this.Text = "frmMyDlg";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.nodeNameMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private Button ButtonToggle;
        private ToolTip TooltipButtonToogle;
        private TreeView treeView;
        private Label LabelStatus;
        private FlowLayoutPanel flowLayoutPanel1;
        private MenuButton menuButton1;
        private ContextMenuStrip nodeNameMenu;
        private ToolStripMenuItem tagNameToolStripMenuItem;
        private ToolStripMenuItem attributeToolStripMenuItem;
        private TextBox attributeNameTextBox;
    }
}
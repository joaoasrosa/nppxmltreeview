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
            this.ButtonToggle = new System.Windows.Forms.Button();
            this.TooltipButtonToogle = new System.Windows.Forms.ToolTip(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.ButtonToggle, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.treeView, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(284, 262);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // treeView
            // 
            this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView.Location = new System.Drawing.Point(3, 25);
            this.treeView.Name = "treeView";
            this.treeView.Size = new System.Drawing.Size(278, 234);
            this.treeView.TabIndex = 1;
            this.treeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.TreeView_NodeMouseClick);
            // 
            // ButtonToggle
            // 
            this.ButtonToggle.BackColor = System.Drawing.Color.Transparent;
            this.ButtonToggle.FlatAppearance.BorderSize = 0;
            this.ButtonToggle.Image = global::NppXmlTreeviewPlugin.Properties.Resources.toggle;
            this.ButtonToggle.Location = new System.Drawing.Point(3, 3);
            this.ButtonToggle.Name = "ButtonToggle";
            this.ButtonToggle.Size = new System.Drawing.Size(16, 16);
            this.ButtonToggle.TabIndex = 0;
            this.ButtonToggle.UseVisualStyleBackColor = false;
            this.ButtonToggle.Click += new System.EventHandler(this.ButtonToggle_Click);
            // 
            // FormTreeView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "FormTreeView";
            this.Text = "frmMyDlg";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private TreeView treeView;
        private Button ButtonToggle;
        private ToolTip TooltipButtonToogle;
    }
}
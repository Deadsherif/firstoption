
using System.Windows.Forms;

namespace License
{
    public partial class ActivateForm : Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
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
            this.txtHardwareID = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.btnCopyCode = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtHardwareID
            // 
            this.txtHardwareID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtHardwareID.BackColor = System.Drawing.Color.LightGray;
            this.txtHardwareID.Font = new System.Drawing.Font("Consolas", 10F);
            this.txtHardwareID.ForeColor = System.Drawing.Color.Black;
            this.txtHardwareID.Location = new System.Drawing.Point(20, 37);
            this.txtHardwareID.Name = "txtHardwareID";
            this.txtHardwareID.ReadOnly = true;
            this.txtHardwareID.Size = new System.Drawing.Size(408, 27);
            this.txtHardwareID.TabIndex = 4;
            this.txtHardwareID.TextChanged += new System.EventHandler(this.txtHardwareID_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(271, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "Please send the machine code below to";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.LinkColor = System.Drawing.Color.DodgerBlue;
            this.linkLabel1.Location = new System.Drawing.Point(288, 17);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(207, 20);
            this.linkLabel1.TabIndex = 6;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "ahmedsherif9220@gmail.com";
            this.toolTip.SetToolTip(this.linkLabel1, "Click to open Mail and send the code...");
            // 
            // btnCopyCode
            // 
            this.btnCopyCode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCopyCode.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.btnCopyCode.Location = new System.Drawing.Point(426, 36);
            this.btnCopyCode.Name = "btnCopyCode";
            this.btnCopyCode.Size = new System.Drawing.Size(62, 25);
            this.btnCopyCode.TabIndex = 7;
            this.btnCopyCode.Text = "Copy";
            this.btnCopyCode.UseVisualStyleBackColor = true;
            this.btnCopyCode.Click += new System.EventHandler(this.BtnCopyCode_Click);
            // 
            // ActivateForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(508, 82);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.txtHardwareID);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnCopyCode);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "ActivateForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Activate Premium License of Axe Plugins";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Activate_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txtHardwareID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Button btnCopyCode;
        private LinkLabel linkLabel1;
    }
}
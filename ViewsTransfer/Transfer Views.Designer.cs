// Decompiled with JetBrains decompiler
// Type: ViewsTransfer.Transfer_Views
// Assembly: ViewsTransfer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 097D957F-A3C8-4D58-A2D2-2E99F75DDB65
// Assembly location: C:\Users\ahmed\Downloads\ViewsTransfer\ViewsTransfer\bin\Debug\ViewsTransfer.dll

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;


namespace ViewsTransfer
{
    public class Transfer_Views : Form
    {
        private IContainer components = (IContainer)null;
        private Button button1;
        private Button button2;
        private Button button3;
        private Label label1;
        private TextBox textBox1;
        private Label label2;

        public Transfer_Views() => this.InitializeComponent();

        private void button1_Click(object sender, EventArgs e)
        {
            Transfer.state = 0;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Transfer.state = 1;
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Transfer.state = -1;
            this.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void Transfer_Views_Load(object sender, EventArgs e)
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
                this.components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(Transfer_Views));
            this.button1 = new Button();
            this.button2 = new Button();
            this.button3 = new Button();
            this.label1 = new Label();
            this.textBox1 = new TextBox();
            this.label2 = new Label();
            this.SuspendLayout();
            this.button1.BackColor = Color.OrangeRed;
            this.button1.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, (byte)0);
            this.button1.Location = new Point(12, 12);
            this.button1.Name = "button1";
            this.button1.Size = new Size(153, 58);
            this.button1.TabIndex = 0;
            this.button1.Text = "COPY!";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new EventHandler(this.button1_Click);
            this.button2.BackColor = SystemColors.MenuHighlight;
            this.button2.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold, GraphicsUnit.Point, (byte)0);
            this.button2.Location = new Point(171, 12);
            this.button2.Name = "button2";
            this.button2.Size = new Size(153, 58);
            this.button2.TabIndex = 1;
            this.button2.Text = "PASTE!";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new EventHandler(this.button2_Click);
            this.button3.BackColor = Color.Red;
            this.button3.Location = new Point(249, 79);
            this.button3.Name = "button3";
            this.button3.Size = new Size(75, 23);
            this.button3.TabIndex = 2;
            this.button3.Text = "Cancel";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new EventHandler(this.button3_Click);
            this.label1.AutoSize = true;
            this.label1.Location = new Point(12, 139);
            this.label1.Name = "label1";
            this.label1.Size = new Size(94, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Development Key:";
            this.textBox1.Location = new Point(12, 165);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Size(309, 20);
            this.textBox1.TabIndex = 4;
            this.textBox1.TextChanged += new EventHandler(this.textBox1_TextChanged);
            this.label2.AutoSize = true;
            this.label2.ForeColor = SystemColors.AppWorkspace;
            this.label2.Location = new Point(12, 107);
            this.label2.Name = "label2";
            this.label2.Size = new Size(41, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "V 1.1.2";
            this.AutoScaleDimensions = new SizeF(6f, 13f);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(334, 124);
            this.ControlBox = false;
            this.Controls.Add((Control)this.label2);
            this.Controls.Add((Control)this.textBox1);
            this.Controls.Add((Control)this.label1);
            this.Controls.Add((Control)this.button3);
            this.Controls.Add((Control)this.button2);
            this.Controls.Add((Control)this.button1);
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            this.Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = nameof(Transfer_Views);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Transfer Views";
            this.TopMost = true;
            this.Width = 350;
            this.Load += new EventHandler(this.Transfer_Views_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}

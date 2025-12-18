// Decompiled with JetBrains decompiler
// Type: ViewsTransfer.Progress
// Assembly: ViewsTransfer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 097D957F-A3C8-4D58-A2D2-2E99F75DDB65
// Assembly location: C:\Users\ahmed\Downloads\ViewsTransfer\ViewsTransfer\bin\Debug\ViewsTransfer.dll

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;


namespace ViewsTransfer
{
  public class Progress : Form
  {
    private IContainer components = (IContainer) null;
    private Label label1;
    private Label label2;
    private Label label3;
    private Label label4;
    private Label label5;
    private Timer timer1;
    private Label label6;
    private Timer timer2;
    private Label label7;

    public Progress()
    {
      this.InitializeComponent();
      this.label4.Text = DB.DBALL_Views.Count.ToString();
    }

    private void label2_Click(object sender, EventArgs e)
    {
    }

    private void label4_Click(object sender, EventArgs e)
    {
    }

    private void timer1_Tick(object sender, EventArgs e)
    {
      this.label2.Text = Transfer.count.ToString();
    }

    private void timer2_Tick(object sender, EventArgs e)
    {
      if (this.label6.Text == ".")
        this.label6.Text = "..";
      else if (this.label6.Text == "..")
        this.label6.Text = "...";
      else
        this.label6.Text = ".";
    }

    private void label6_Click(object sender, EventArgs e)
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
      this.components = (IContainer) new System.ComponentModel.Container();
      this.label1 = new Label();
      this.label2 = new Label();
      this.label3 = new Label();
      this.label4 = new Label();
      this.label5 = new Label();
      this.timer1 = new Timer(this.components);
      this.label6 = new Label();
      this.timer2 = new Timer(this.components);
      this.label7 = new Label();
      this.SuspendLayout();
      this.label1.AutoSize = true;
      this.label1.Font = new Font("Microsoft Sans Serif", 12f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.label1.Location = new Point(53, 43);
      this.label1.Name = "label1";
      this.label1.Size = new Size(76, 20);
      this.label1.TabIndex = 0;
      this.label1.Text = "Progress:";
      this.label2.AutoSize = true;
      this.label2.Font = new Font("Microsoft Sans Serif", 12f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.label2.Location = new Point(125, 43);
      this.label2.Name = "label2";
      this.label2.Size = new Size(19, 20);
      this.label2.TabIndex = 1;
      this.label2.Text = "0";
      this.label2.Click += new EventHandler(this.label2_Click);
      this.label3.AutoSize = true;
      this.label3.Font = new Font("Microsoft Sans Serif", 12f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.label3.Location = new Point(163, 43);
      this.label3.Name = "label3";
      this.label3.Size = new Size(13, 20);
      this.label3.TabIndex = 2;
      this.label3.Text = "/";
      this.label4.AutoSize = true;
      this.label4.Font = new Font("Microsoft Sans Serif", 12f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.label4.Location = new Point(181, 43);
      this.label4.Name = "label4";
      this.label4.Size = new Size(36, 20);
      this.label4.TabIndex = 3;
      this.label4.Text = "999";
      this.label4.Click += new EventHandler(this.label4_Click);
      this.label5.AutoSize = true;
      this.label5.Font = new Font("Microsoft Sans Serif", 12f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.label5.Location = new Point(216, 43);
      this.label5.Name = "label5";
      this.label5.Size = new Size(60, 20);
      this.label5.TabIndex = 4;
      this.label5.Text = "Sheets";
      this.timer1.Interval = 10000;
      this.timer1.Tick += new EventHandler(this.timer1_Tick);
      this.label6.AutoSize = true;
      this.label6.Font = new Font("Microsoft Sans Serif", 12f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.label6.Location = new Point(271, 44);
      this.label6.Name = "label6";
      this.label6.Size = new Size(13, 20);
      this.label6.TabIndex = 5;
      this.label6.Text = ".";
      this.label6.Click += new EventHandler(this.label6_Click);
      this.timer2.Interval = 1000;
      this.timer2.Tick += new EventHandler(this.timer2_Tick);
      this.label7.AutoSize = true;
      this.label7.Location = new Point(13, 93);
      this.label7.Name = "label7";
      this.label7.Size = new Size(158, 13);
      this.label7.TabIndex = 6;
      this.label7.Text = "*Please don't close this window!";
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(333, 118);
      this.ControlBox = false;
      this.Controls.Add((Control) this.label7);
      this.Controls.Add((Control) this.label6);
      this.Controls.Add((Control) this.label5);
      this.Controls.Add((Control) this.label4);
      this.Controls.Add((Control) this.label3);
      this.Controls.Add((Control) this.label2);
      this.Controls.Add((Control) this.label1);
      this.Cursor = Cursors.AppStarting;
      this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
      this.Name = nameof (Progress);
      this.Text = nameof (Progress);
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}

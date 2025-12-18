// Decompiled with JetBrains decompiler
// Type: CableOverTray.COT_Form_Main
// Assembly: COT, Version=2021.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 774E24BD-3C4D-44CA-9437-58E3900DE92B
// Assembly location: C:\Users\ahmed\AppData\Roaming\Autodesk\Revit\Addins\2023\CableOverTray.dll

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CableOverTray.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

 
namespace CableOverTray
{
  public class COT_Form_Main : System.Windows.Forms.Form
  {
    private ExternalEvent rste;
    private ExternalEvent sste;
    private ExternalEvent rmte;
    private ExternalEvent rcste;
    private ExternalEvent scte;
    private ExternalEvent scce;
    private ExternalEvent rmcte;
    private IContainer components = (IContainer) null;
    private Button button1;
    private Button button6;
    private Label label3;
    private Button button7;
    private Button button8;
    private Button button9;
    private Button button10;
    private PictureBox pictureBox1;
    private PictureBox pictureBox2;
    private PictureBox pictureBox3;
    private PictureBox pictureBox4;

    public COT_Form_Main(
      ExternalEvent rste,
      ExternalEvent sste,
      ExternalEvent rmte,
      ExternalEvent rcste,
      ExternalEvent scte,
      ExternalEvent scce,
      ExternalEvent rmcte,
      List<Element> types)
    {
      this.InitializeComponent();
      this.rste = rste;
      this.sste = sste;
      this.rmte = rmte;
      this.rcste = rcste;
      this.scte = scte;
      this.scce = scce;
      this.rmcte = rmcte;
    }

    private void textBox1_TextChanged(object sender, EventArgs e)
    {
    }

    private void button6_Click(object sender, EventArgs e) => this.Hide();

    private void button4_Click(object sender, EventArgs e)
    {
    }

    private void Form1_Load(object sender, EventArgs e)
    {
    }

    private void button3_Click(object sender, EventArgs e)
    {
    }

    private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
    {
    }

    private void checkBox1_CheckedChanged(object sender, EventArgs e)
    {
    }

    private void button2_Click(object sender, EventArgs e)
    {
    }

    private void button1_Click(object sender, EventArgs e)
    {
      int num = (int) this.sste.Raise();
      this.Hide();
    }

    private void button5_Click(object sender, EventArgs e)
    {
    }

    private void button7_Click(object sender, EventArgs e)
    {
      int num = (int) this.rcste.Raise();
      this.Hide();
    }

    private void button8_Click(object sender, EventArgs e)
    {
      this.Hide();
      int num = (int) this.scte.Raise();
    }

    private void button9_Click(object sender, EventArgs e)
    {
      this.Hide();
      int num = (int) this.scce.Raise();
    }

    private void button10_Click(object sender, EventArgs e)
    {
      this.Hide();
      int num = (int) this.rmcte.Raise();
    }

    private void label3_Click(object sender, EventArgs e)
    {
    }

    private void pictureBox1_Click(object sender, EventArgs e)
    {
    }

    private void button8_MouseMove(object sender, MouseEventArgs e)
    {
      this.pictureBox1.Image = (Image) Resources._1;
    }

    private void button8_MouseEnter(object sender, EventArgs e)
    {
      this.pictureBox1.Image = (Image) Resources._1;
    }

    private void button8_MouseHover(object sender, EventArgs e)
    {
      this.pictureBox1.Image = (Image) Resources._1;
    }

    private void button8_MouseLeave(object sender, EventArgs e)
    {
      this.pictureBox1.Image = (Image) Resources._0;
    }

    private void button9_MouseHover(object sender, EventArgs e)
    {
      this.pictureBox1.Image = (Image) Resources._2;
    }

    private void button9_MouseEnter(object sender, EventArgs e)
    {
      this.pictureBox1.Image = (Image) Resources._2;
    }

    private void button9_MouseMove(object sender, MouseEventArgs e)
    {
      this.pictureBox1.Image = (Image) Resources._2;
    }

    private void button9_MouseLeave(object sender, EventArgs e)
    {
      this.pictureBox1.Image = (Image) Resources._0;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.button1 = new Button();
      this.button6 = new Button();
      this.label3 = new Label();
      this.button7 = new Button();
      this.button8 = new Button();
      this.button9 = new Button();
      this.button10 = new Button();
      this.pictureBox4 = new PictureBox();
      this.pictureBox3 = new PictureBox();
      this.pictureBox2 = new PictureBox();
      this.pictureBox1 = new PictureBox();
      ((ISupportInitialize) this.pictureBox4).BeginInit();
      ((ISupportInitialize) this.pictureBox3).BeginInit();
      ((ISupportInitialize) this.pictureBox2).BeginInit();
      ((ISupportInitialize) this.pictureBox1).BeginInit();
      this.SuspendLayout();
      this.button1.Location = new System.Drawing.Point(15, 147);
      this.button1.Name = "button1";
      this.button1.Size = new Size(158, 55);
      this.button1.TabIndex = 11;
      this.button1.Text = "Select Multible Trays";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new EventHandler(this.button1_Click);
      this.button6.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.button6.BackColor = System.Drawing.Color.Red;
      this.button6.Location = new System.Drawing.Point(650, 396);
      this.button6.Name = "button6";
      this.button6.Size = new Size(75, 23);
      this.button6.TabIndex = 15;
      this.button6.Text = "Close";
      this.button6.UseVisualStyleBackColor = false;
      this.button6.Click += new EventHandler(this.button6_Click);
      this.label3.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      this.label3.AutoSize = true;
      this.label3.ForeColor = SystemColors.AppWorkspace;
      this.label3.Location = new System.Drawing.Point(12, 413);
      this.label3.Name = "label3";
      this.label3.Size = new Size(101, 13);
      this.label3.TabIndex = 16;
      this.label3.Text = "Version: alpha 0.0.2";
      this.label3.Click += new EventHandler(this.label3_Click);
      this.button7.Location = new System.Drawing.Point(15, 360);
      this.button7.Name = "button7";
      this.button7.Size = new Size(158, 42);
      this.button7.TabIndex = 49;
      this.button7.Text = "Custom Single Tray";
      this.button7.UseVisualStyleBackColor = true;
      this.button7.Click += new EventHandler(this.button7_Click);
      this.button8.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.button8.Location = new System.Drawing.Point(550, 147);
      this.button8.Name = "button8";
      this.button8.Size = new Size(175, 55);
      this.button8.TabIndex = 50;
      this.button8.Text = "Custom tray";
      this.button8.UseVisualStyleBackColor = true;
      this.button8.Click += new EventHandler(this.button8_Click);
      this.button8.MouseEnter += new EventHandler(this.button8_MouseEnter);
      this.button8.MouseLeave += new EventHandler(this.button8_MouseLeave);
      this.button8.MouseHover += new EventHandler(this.button8_MouseHover);
      this.button8.MouseMove += new MouseEventHandler(this.button8_MouseMove);
      this.button9.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.button9.Location = new System.Drawing.Point(375, 147);
      this.button9.Name = "button9";
      this.button9.Size = new Size(175, 55);
      this.button9.TabIndex = 51;
      this.button9.Text = "Custom conduits";
      this.button9.UseVisualStyleBackColor = true;
      this.button9.Click += new EventHandler(this.button9_Click);
      this.button9.MouseEnter += new EventHandler(this.button9_MouseEnter);
      this.button9.MouseLeave += new EventHandler(this.button9_MouseLeave);
      this.button9.MouseHover += new EventHandler(this.button9_MouseHover);
      this.button9.MouseMove += new MouseEventHandler(this.button9_MouseMove);
      this.button10.Location = new System.Drawing.Point(197, 147);
      this.button10.Name = "button10";
      this.button10.Size = new Size(158, 55);
      this.button10.TabIndex = 52;
      this.button10.Text = "Custom Multible Tray";
      this.button10.UseVisualStyleBackColor = true;
      this.button10.Click += new EventHandler(this.button10_Click);
      this.pictureBox4.Image = (Image) Resources.c;
      this.pictureBox4.Location = new System.Drawing.Point(197, 12);
      this.pictureBox4.Name = "pictureBox4";
      this.pictureBox4.Size = new Size(158, 129);
      this.pictureBox4.SizeMode = PictureBoxSizeMode.Zoom;
      this.pictureBox4.TabIndex = 56;
      this.pictureBox4.TabStop = false;
      this.pictureBox3.Image = (Image) Resources.b;
      this.pictureBox3.Location = new System.Drawing.Point(15, 12);
      this.pictureBox3.Name = "pictureBox3";
      this.pictureBox3.Size = new Size(158, 129);
      this.pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
      this.pictureBox3.TabIndex = 55;
      this.pictureBox3.TabStop = false;
      this.pictureBox2.Image = (Image) Resources.a;
      this.pictureBox2.Location = new System.Drawing.Point(15, 225);
      this.pictureBox2.Name = "pictureBox2";
      this.pictureBox2.Size = new Size(158, 129);
      this.pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
      this.pictureBox2.TabIndex = 54;
      this.pictureBox2.TabStop = false;
      this.pictureBox1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.pictureBox1.Image = (Image) Resources._0;
      this.pictureBox1.Location = new System.Drawing.Point(375, 12);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new Size(350, 129);
      this.pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
      this.pictureBox1.TabIndex = 53;
      this.pictureBox1.TabStop = false;
      this.pictureBox1.Click += new EventHandler(this.pictureBox1_Click);
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.White;
      this.ClientSize = new Size(737, 431);
      this.ControlBox = false;
      this.Controls.Add((System.Windows.Forms.Control) this.pictureBox4);
      this.Controls.Add((System.Windows.Forms.Control) this.pictureBox3);
      this.Controls.Add((System.Windows.Forms.Control) this.pictureBox2);
      this.Controls.Add((System.Windows.Forms.Control) this.pictureBox1);
      this.Controls.Add((System.Windows.Forms.Control) this.button10);
      this.Controls.Add((System.Windows.Forms.Control) this.button9);
      this.Controls.Add((System.Windows.Forms.Control) this.button8);
      this.Controls.Add((System.Windows.Forms.Control) this.button7);
      this.Controls.Add((System.Windows.Forms.Control) this.label3);
      this.Controls.Add((System.Windows.Forms.Control) this.button6);
      this.Controls.Add((System.Windows.Forms.Control) this.button1);
      this.MaximumSize = new Size(753, 470);
      this.MinimumSize = new Size(753, 470);
      this.Name = nameof (COT_Form_Main);
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = FormStartPosition.CenterScreen;
      this.Text = "FirstOption - C.O.T";
      this.TopMost = true;
      this.Load += new EventHandler(this.Form1_Load);
      ((ISupportInitialize) this.pictureBox4).EndInit();
      ((ISupportInitialize) this.pictureBox3).EndInit();
      ((ISupportInitialize) this.pictureBox2).EndInit();
      ((ISupportInitialize) this.pictureBox1).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}

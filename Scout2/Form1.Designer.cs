namespace Scout2 {
   partial class Form1 {
      /// <summary>
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.IContainer components = null;

      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
      /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
      protected override void Dispose(bool disposing) {
         if (disposing && (components != null)) {
            components.Dispose();
         }
         base.Dispose(disposing);
      }

      #region Windows Form Designer generated code

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent() {
         this.btnLegSite = new System.Windows.Forms.Button();
         this.btnZipFile = new System.Windows.Forms.Button();
         this.btnImport = new System.Windows.Forms.Button();
         this.btnReport = new System.Windows.Forms.Button();
         this.groupBox1 = new System.Windows.Forms.GroupBox();
         this.txtZipProgress = new System.Windows.Forms.TextBox();
         this.progressReport = new System.Windows.Forms.ProgressBar();
         this.progressImport = new System.Windows.Forms.ProgressBar();
         this.progressLegSite = new System.Windows.Forms.ProgressBar();
         this.groupBox2 = new System.Windows.Forms.GroupBox();
         this.progressTwoWordsNear = new System.Windows.Forms.ProgressBar();
         this.button1 = new System.Windows.Forms.Button();
         this.label3 = new System.Windows.Forms.Label();
         this.label2 = new System.Windows.Forms.Label();
         this.txtMaxDistance = new System.Windows.Forms.TextBox();
         this.txtMinDistance = new System.Windows.Forms.TextBox();
         this.label1 = new System.Windows.Forms.Label();
         this.txt2ndWord = new System.Windows.Forms.TextBox();
         this.txt1stWord = new System.Windows.Forms.TextBox();
         this.bgw_TwoWordsNear = new System.ComponentModel.BackgroundWorker();
         this.menuStrip1 = new System.Windows.Forms.MenuStrip();
         this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.helpToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
         this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.updateFoldersFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.groupBox1.SuspendLayout();
         this.groupBox2.SuspendLayout();
         this.menuStrip1.SuspendLayout();
         this.SuspendLayout();
         // 
         // btnLegSite
         // 
         this.btnLegSite.Location = new System.Drawing.Point(12, 16);
         this.btnLegSite.Name = "btnLegSite";
         this.btnLegSite.Size = new System.Drawing.Size(104, 23);
         this.btnLegSite.TabIndex = 0;
         this.btnLegSite.Text = "Start At Leg Site";
         this.btnLegSite.UseVisualStyleBackColor = true;
         this.btnLegSite.Click += new System.EventHandler(this.btnLegSite_Click);
         // 
         // btnZipFile
         // 
         this.btnZipFile.Location = new System.Drawing.Point(12, 53);
         this.btnZipFile.Name = "btnZipFile";
         this.btnZipFile.Size = new System.Drawing.Size(104, 23);
         this.btnZipFile.TabIndex = 1;
         this.btnZipFile.Text = "Start At Zip File";
         this.btnZipFile.UseVisualStyleBackColor = true;
         this.btnZipFile.Click += new System.EventHandler(this.btnZipFile_Click);
         // 
         // btnImport
         // 
         this.btnImport.Location = new System.Drawing.Point(12, 91);
         this.btnImport.Name = "btnImport";
         this.btnImport.Size = new System.Drawing.Size(104, 23);
         this.btnImport.TabIndex = 2;
         this.btnImport.Text = "Start at Import";
         this.btnImport.UseVisualStyleBackColor = true;
         this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
         // 
         // btnReport
         // 
         this.btnReport.Location = new System.Drawing.Point(12, 131);
         this.btnReport.Name = "btnReport";
         this.btnReport.Size = new System.Drawing.Size(104, 23);
         this.btnReport.TabIndex = 3;
         this.btnReport.Text = "Start At Report";
         this.btnReport.UseVisualStyleBackColor = true;
         this.btnReport.Click += new System.EventHandler(this.btnReport_Click);
         // 
         // groupBox1
         // 
         this.groupBox1.Controls.Add(this.txtZipProgress);
         this.groupBox1.Controls.Add(this.progressReport);
         this.groupBox1.Controls.Add(this.progressImport);
         this.groupBox1.Controls.Add(this.progressLegSite);
         this.groupBox1.Controls.Add(this.btnReport);
         this.groupBox1.Controls.Add(this.btnImport);
         this.groupBox1.Controls.Add(this.btnZipFile);
         this.groupBox1.Controls.Add(this.btnLegSite);
         this.groupBox1.Location = new System.Drawing.Point(38, 34);
         this.groupBox1.Name = "groupBox1";
         this.groupBox1.Size = new System.Drawing.Size(509, 176);
         this.groupBox1.TabIndex = 4;
         this.groupBox1.TabStop = false;
         this.groupBox1.Text = "Scout Starting Points";
         // 
         // txtZipProgress
         // 
         this.txtZipProgress.AccessibleName = "txtZipProgress";
         this.txtZipProgress.Location = new System.Drawing.Point(132, 53);
         this.txtZipProgress.Name = "txtZipProgress";
         this.txtZipProgress.Size = new System.Drawing.Size(347, 20);
         this.txtZipProgress.TabIndex = 8;
         // 
         // progressReport
         // 
         this.progressReport.Location = new System.Drawing.Point(132, 131);
         this.progressReport.Name = "progressReport";
         this.progressReport.Size = new System.Drawing.Size(347, 23);
         this.progressReport.TabIndex = 7;
         // 
         // progressImport
         // 
         this.progressImport.AccessibleName = "txtZipProgress";
         this.progressImport.Location = new System.Drawing.Point(132, 91);
         this.progressImport.Name = "progressImport";
         this.progressImport.Size = new System.Drawing.Size(347, 23);
         this.progressImport.TabIndex = 6;
         // 
         // progressLegSite
         // 
         this.progressLegSite.Location = new System.Drawing.Point(132, 16);
         this.progressLegSite.Name = "progressLegSite";
         this.progressLegSite.Size = new System.Drawing.Size(347, 23);
         this.progressLegSite.TabIndex = 4;
         // 
         // groupBox2
         // 
         this.groupBox2.Controls.Add(this.progressTwoWordsNear);
         this.groupBox2.Controls.Add(this.button1);
         this.groupBox2.Controls.Add(this.label3);
         this.groupBox2.Controls.Add(this.label2);
         this.groupBox2.Controls.Add(this.txtMaxDistance);
         this.groupBox2.Controls.Add(this.txtMinDistance);
         this.groupBox2.Controls.Add(this.label1);
         this.groupBox2.Controls.Add(this.txt2ndWord);
         this.groupBox2.Controls.Add(this.txt1stWord);
         this.groupBox2.Location = new System.Drawing.Point(38, 236);
         this.groupBox2.Name = "groupBox2";
         this.groupBox2.Size = new System.Drawing.Size(509, 162);
         this.groupBox2.TabIndex = 5;
         this.groupBox2.TabStop = false;
         this.groupBox2.Text = "Two Words Near Each Other";
         // 
         // progressTwoWordsNear
         // 
         this.progressTwoWordsNear.Location = new System.Drawing.Point(24, 121);
         this.progressTwoWordsNear.Name = "progressTwoWordsNear";
         this.progressTwoWordsNear.Size = new System.Drawing.Size(369, 23);
         this.progressTwoWordsNear.TabIndex = 8;
         // 
         // button1
         // 
         this.button1.Location = new System.Drawing.Point(318, 83);
         this.button1.Name = "button1";
         this.button1.Size = new System.Drawing.Size(75, 23);
         this.button1.TabIndex = 7;
         this.button1.Text = "Do Search";
         this.button1.UseVisualStyleBackColor = true;
         this.button1.Click += new System.EventHandler(this.button1_Click);
         // 
         // label3
         // 
         this.label3.AutoSize = true;
         this.label3.Location = new System.Drawing.Point(185, 64);
         this.label3.Name = "label3";
         this.label3.Size = new System.Drawing.Size(96, 13);
         this.label3.TabIndex = 6;
         this.label3.Text = "Maximum Distance";
         // 
         // label2
         // 
         this.label2.AutoSize = true;
         this.label2.Location = new System.Drawing.Point(24, 64);
         this.label2.Name = "label2";
         this.label2.Size = new System.Drawing.Size(93, 13);
         this.label2.TabIndex = 5;
         this.label2.Text = "Minimum Distance";
         // 
         // txtMaxDistance
         // 
         this.txtMaxDistance.Location = new System.Drawing.Point(185, 85);
         this.txtMaxDistance.Name = "txtMaxDistance";
         this.txtMaxDistance.Size = new System.Drawing.Size(100, 20);
         this.txtMaxDistance.TabIndex = 4;
         // 
         // txtMinDistance
         // 
         this.txtMinDistance.Location = new System.Drawing.Point(24, 85);
         this.txtMinDistance.Name = "txtMinDistance";
         this.txtMinDistance.Size = new System.Drawing.Size(100, 20);
         this.txtMinDistance.TabIndex = 3;
         // 
         // label1
         // 
         this.label1.AutoSize = true;
         this.label1.Location = new System.Drawing.Point(140, 40);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(30, 13);
         this.label1.TabIndex = 2;
         this.label1.Text = "Near";
         // 
         // txt2ndWord
         // 
         this.txt2ndWord.Location = new System.Drawing.Point(185, 33);
         this.txt2ndWord.Name = "txt2ndWord";
         this.txt2ndWord.Size = new System.Drawing.Size(100, 20);
         this.txt2ndWord.TabIndex = 1;
         // 
         // txt1stWord
         // 
         this.txt1stWord.Location = new System.Drawing.Point(24, 33);
         this.txt1stWord.Name = "txt1stWord";
         this.txt1stWord.Size = new System.Drawing.Size(100, 20);
         this.txt1stWord.TabIndex = 0;
         // 
         // bgw_TwoWordsNear
         // 
         this.bgw_TwoWordsNear.WorkerReportsProgress = true;
         this.bgw_TwoWordsNear.WorkerSupportsCancellation = true;
         // 
         // menuStrip1
         // 
         this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.updateFoldersFileToolStripMenuItem});
         this.menuStrip1.Location = new System.Drawing.Point(0, 0);
         this.menuStrip1.Name = "menuStrip1";
         this.menuStrip1.Size = new System.Drawing.Size(600, 24);
         this.menuStrip1.TabIndex = 6;
         this.menuStrip1.Text = "menuStrip1";
         // 
         // fileToolStripMenuItem
         // 
         this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpToolStripMenuItem1,
            this.exitToolStripMenuItem});
         this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
         this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
         this.fileToolStripMenuItem.Text = "File";
         // 
         // helpToolStripMenuItem1
         // 
         this.helpToolStripMenuItem1.Name = "helpToolStripMenuItem1";
         this.helpToolStripMenuItem1.Size = new System.Drawing.Size(99, 22);
         this.helpToolStripMenuItem1.Text = "Help";
         this.helpToolStripMenuItem1.Click += new System.EventHandler(this.helpToolStripMenuItem1_Click);
         // 
         // exitToolStripMenuItem
         // 
         this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
         this.exitToolStripMenuItem.Size = new System.Drawing.Size(99, 22);
         this.exitToolStripMenuItem.Text = "Exit";
         this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
         // 
         // updateFoldersFileToolStripMenuItem
         // 
         this.updateFoldersFileToolStripMenuItem.Name = "updateFoldersFileToolStripMenuItem";
         this.updateFoldersFileToolStripMenuItem.Size = new System.Drawing.Size(113, 20);
         this.updateFoldersFileToolStripMenuItem.Text = "Update Constants";
         this.updateFoldersFileToolStripMenuItem.Click += new System.EventHandler(this.updateFoldersFileToolStripMenuItem_Click);
         // 
         // Form1
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(600, 431);
         this.Controls.Add(this.groupBox2);
         this.Controls.Add(this.groupBox1);
         this.Controls.Add(this.menuStrip1);
         this.MainMenuStrip = this.menuStrip1;
         this.Name = "Form1";
         this.Text = "Scout California Legislature";
         this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
         this.groupBox1.ResumeLayout(false);
         this.groupBox1.PerformLayout();
         this.groupBox2.ResumeLayout(false);
         this.groupBox2.PerformLayout();
         this.menuStrip1.ResumeLayout(false);
         this.menuStrip1.PerformLayout();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Button btnLegSite;
      private System.Windows.Forms.Button btnZipFile;
      private System.Windows.Forms.Button btnImport;
      private System.Windows.Forms.Button btnReport;
      private System.Windows.Forms.GroupBox groupBox1;
      private System.Windows.Forms.GroupBox groupBox2;
      private System.Windows.Forms.Button button1;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.TextBox txtMaxDistance;
      private System.Windows.Forms.TextBox txtMinDistance;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.TextBox txt2ndWord;
      private System.Windows.Forms.TextBox txt1stWord;
      private System.ComponentModel.BackgroundWorker bgw_TwoWordsNear;
      public System.Windows.Forms.ProgressBar progressTwoWordsNear;
      public System.Windows.Forms.ProgressBar progressLegSite;
      public System.Windows.Forms.ProgressBar progressReport;
      public System.Windows.Forms.ProgressBar progressImport;
      private System.Windows.Forms.MenuStrip menuStrip1;
      private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem1;
      private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem updateFoldersFileToolStripMenuItem;
      public System.Windows.Forms.TextBox txtZipProgress;
   }
}


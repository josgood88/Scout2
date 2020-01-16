namespace Scout2 {
   partial class UnreportedBillsForm {
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
         this.label1 = new System.Windows.Forms.Label();
         this.ViewUnreportedBills = new System.Windows.Forms.DataGridView();
         ((System.ComponentModel.ISupportInitialize)(this.ViewUnreportedBills)).BeginInit();
         this.SuspendLayout();
         // 
         // label1
         // 
         this.label1.AutoSize = true;
         this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.label1.Location = new System.Drawing.Point(233, 14);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(250, 20);
         this.label1.TabIndex = 0;
         this.label1.Text = "There are no reports for these bills";
         // 
         // ViewUnreportedBills
         // 
         this.ViewUnreportedBills.AllowUserToAddRows = false;
         this.ViewUnreportedBills.AllowUserToDeleteRows = false;
         this.ViewUnreportedBills.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.ViewUnreportedBills.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
         this.ViewUnreportedBills.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
         this.ViewUnreportedBills.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
         this.ViewUnreportedBills.Location = new System.Drawing.Point(12, 46);
         this.ViewUnreportedBills.Name = "ViewUnreportedBills";
         this.ViewUnreportedBills.Size = new System.Drawing.Size(766, 392);
         this.ViewUnreportedBills.TabIndex = 1;
         this.ViewUnreportedBills.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.OnCellClick);
         // 
         // UnreportedBillsForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.AutoScroll = true;
         this.AutoSize = true;
         this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
         this.ClientSize = new System.Drawing.Size(800, 450);
         this.Controls.Add(this.ViewUnreportedBills);
         this.Controls.Add(this.label1);
         this.Name = "UnreportedBillsForm";
         this.Text = "Create These Bill Reports";
         ((System.ComponentModel.ISupportInitialize)(this.ViewUnreportedBills)).EndInit();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView ViewUnreportedBills;
    }
}
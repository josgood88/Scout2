namespace Scout2 {
   partial class NewBillsForm {
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
         this.ViewBillsRequiringUpdate = new System.Windows.Forms.DataGridView();
         this.label1 = new System.Windows.Forms.Label();
         ((System.ComponentModel.ISupportInitialize)(this.ViewBillsRequiringUpdate)).BeginInit();
         this.SuspendLayout();
         // 
         // ViewBillsRequiringUpdate
         // 
         this.ViewBillsRequiringUpdate.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
         this.ViewBillsRequiringUpdate.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
         this.ViewBillsRequiringUpdate.Location = new System.Drawing.Point(40, 92);
         this.ViewBillsRequiringUpdate.Name = "ViewBillsRequiringUpdate";
         this.ViewBillsRequiringUpdate.Size = new System.Drawing.Size(624, 309);
         this.ViewBillsRequiringUpdate.TabIndex = 2;
         // 
         // label1
         // 
         this.label1.AutoSize = true;
         this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.label1.Location = new System.Drawing.Point(215, 23);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(229, 20);
         this.label1.TabIndex = 3;
         this.label1.Text = "These bills do not have a report";
         // 
         // NewBillsForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(689, 450);
         this.Controls.Add(this.label1);
         this.Controls.Add(this.ViewBillsRequiringUpdate);
         this.Name = "NewBillsForm";
         this.Text = "Bills Currently Without a Position";
         ((System.ComponentModel.ISupportInitialize)(this.ViewBillsRequiringUpdate)).EndInit();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

        #endregion

        private System.Windows.Forms.DataGridView ViewBillsRequiringUpdate;
        private System.Windows.Forms.Label label1;
    }
}
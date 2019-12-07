namespace Scout2 {
   partial class UpdatedBillsForm {
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
         this.ViewBillsRequiringUpdate = new System.Windows.Forms.DataGridView();
         this.chkNonNoneOnly = new System.Windows.Forms.CheckBox();
         ((System.ComponentModel.ISupportInitialize)(this.ViewBillsRequiringUpdate)).BeginInit();
         this.SuspendLayout();
         // 
         // label1
         // 
         this.label1.AutoSize = true;
         this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.label1.Location = new System.Drawing.Point(215, 23);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(182, 20);
         this.label1.TabIndex = 0;
         this.label1.Text = "Update these bill reports";
         // 
         // ViewBillsRequiringUpdate
         // 
         this.ViewBillsRequiringUpdate.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
         this.ViewBillsRequiringUpdate.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
         this.ViewBillsRequiringUpdate.Location = new System.Drawing.Point(40, 92);
         this.ViewBillsRequiringUpdate.Name = "ViewBillsRequiringUpdate";
         this.ViewBillsRequiringUpdate.Size = new System.Drawing.Size(624, 309);
         this.ViewBillsRequiringUpdate.TabIndex = 1;
         // 
         // chkNonNoneOnly
         // 
         this.chkNonNoneOnly.AutoSize = true;
         this.chkNonNoneOnly.Location = new System.Drawing.Point(40, 58);
         this.chkNonNoneOnly.Name = "chkNonNoneOnly";
         this.chkNonNoneOnly.Size = new System.Drawing.Size(239, 17);
         this.chkNonNoneOnly.TabIndex = 2;
         this.chkNonNoneOnly.Text = "Show only reports who position is not \"None\"";
         this.chkNonNoneOnly.UseVisualStyleBackColor = true;
         this.chkNonNoneOnly.CheckStateChanged += new System.EventHandler(this.OnToggleCheckbox);
         // 
         // UpdatedBillsForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(687, 450);
         this.Controls.Add(this.chkNonNoneOnly);
         this.Controls.Add(this.ViewBillsRequiringUpdate);
         this.Controls.Add(this.label1);
         this.Name = "UpdatedBillsForm";
         this.Text = "Update These Bill Reports";
         ((System.ComponentModel.ISupportInitialize)(this.ViewBillsRequiringUpdate)).EndInit();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.DataGridView ViewBillsRequiringUpdate;
      private System.Windows.Forms.CheckBox chkNonNoneOnly;
   }
}
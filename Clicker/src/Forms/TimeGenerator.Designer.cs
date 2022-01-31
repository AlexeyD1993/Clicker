
namespace Clicker.src.Forms
{
    partial class TimeGenerator
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.dateTimePickerFirstStart = new System.Windows.Forms.DateTimePicker();
            this.dateTimePickerInterval = new System.Windows.Forms.DateTimePicker();
            this.buttonGenerate = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.dateTimePickerFirstStart, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.dateTimePickerInterval, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.buttonGenerate, 1, 2);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(13, 13);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(395, 142);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(189, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Начальное время первого задания:";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(135, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Интервал:";
            // 
            // dateTimePickerFirstStart
            // 
            this.dateTimePickerFirstStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerFirstStart.CustomFormat = "dd.MM.yyyy HH:mm";
            this.dateTimePickerFirstStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerFirstStart.Location = new System.Drawing.Point(200, 13);
            this.dateTimePickerFirstStart.Name = "dateTimePickerFirstStart";
            this.dateTimePickerFirstStart.Size = new System.Drawing.Size(192, 20);
            this.dateTimePickerFirstStart.TabIndex = 2;
            // 
            // dateTimePickerInterval
            // 
            this.dateTimePickerInterval.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.dateTimePickerInterval.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dateTimePickerInterval.Location = new System.Drawing.Point(200, 60);
            this.dateTimePickerInterval.Name = "dateTimePickerInterval";
            this.dateTimePickerInterval.Size = new System.Drawing.Size(192, 20);
            this.dateTimePickerInterval.TabIndex = 3;
            this.dateTimePickerInterval.Value = new System.DateTime(2021, 12, 19, 0, 30, 0, 0);
            // 
            // buttonGenerate
            // 
            this.buttonGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonGenerate.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.buttonGenerate.Location = new System.Drawing.Point(200, 97);
            this.buttonGenerate.Name = "buttonGenerate";
            this.buttonGenerate.Size = new System.Drawing.Size(192, 42);
            this.buttonGenerate.TabIndex = 4;
            this.buttonGenerate.Text = "Сгенерировать";
            this.buttonGenerate.UseVisualStyleBackColor = true;
            this.buttonGenerate.Click += new System.EventHandler(this.buttonGenerate_Click_1);
            // 
            // TimeGenerator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(420, 167);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "TimeGenerator";
            this.Text = "Генератор времени";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dateTimePickerFirstStart;
        private System.Windows.Forms.DateTimePicker dateTimePickerInterval;
        private System.Windows.Forms.Button buttonGenerate;
    }
}
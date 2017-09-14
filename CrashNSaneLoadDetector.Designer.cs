namespace CrashNSaneLoadDetector
{
	partial class CrashNSaneLoadDetector
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
			this.startButton = new System.Windows.Forms.Button();
			this.imageDisplay = new System.Windows.Forms.PictureBox();
			this.matchDisplayLabel = new System.Windows.Forms.Label();
			this.pausedDisplay = new System.Windows.Forms.Panel();
			this.gameTimeLabel = new System.Windows.Forms.Label();
			this.realTimeLabel = new System.Windows.Forms.Label();
			this.pausedTimeLabel = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.pauseSegmentList = new System.Windows.Forms.ListBox();
			this.label4 = new System.Windows.Forms.Label();
			this.recordCheck = new System.Windows.Forms.CheckBox();
			this.saveDiagnosticCheck = new System.Windows.Forms.CheckBox();
			this.label6 = new System.Windows.Forms.Label();
			this.requiredMatchDisplayLabel = new System.Windows.Forms.Label();
			this.recordCurrentButton = new System.Windows.Forms.Button();
			this.resetButton = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.processListBox = new System.Windows.Forms.ListBox();
			this.label7 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.imageDisplay)).BeginInit();
			this.SuspendLayout();
			// 
			// startButton
			// 
			this.startButton.Location = new System.Drawing.Point(12, 397);
			this.startButton.Name = "startButton";
			this.startButton.Size = new System.Drawing.Size(43, 23);
			this.startButton.TabIndex = 0;
			this.startButton.Text = "Start";
			this.startButton.UseVisualStyleBackColor = true;
			this.startButton.Click += new System.EventHandler(this.startButton_Click);
			// 
			// imageDisplay
			// 
			this.imageDisplay.Location = new System.Drawing.Point(12, 5);
			this.imageDisplay.Name = "imageDisplay";
			this.imageDisplay.Size = new System.Drawing.Size(500, 200);
			this.imageDisplay.TabIndex = 1;
			this.imageDisplay.TabStop = false;
			// 
			// matchDisplayLabel
			// 
			this.matchDisplayLabel.AutoSize = true;
			this.matchDisplayLabel.Location = new System.Drawing.Point(12, 234);
			this.matchDisplayLabel.Name = "matchDisplayLabel";
			this.matchDisplayLabel.Size = new System.Drawing.Size(13, 13);
			this.matchDisplayLabel.TabIndex = 2;
			this.matchDisplayLabel.Text = "0";
			// 
			// pausedDisplay
			// 
			this.pausedDisplay.Location = new System.Drawing.Point(130, 357);
			this.pausedDisplay.Name = "pausedDisplay";
			this.pausedDisplay.Size = new System.Drawing.Size(37, 35);
			this.pausedDisplay.TabIndex = 3;
			// 
			// gameTimeLabel
			// 
			this.gameTimeLabel.AutoSize = true;
			this.gameTimeLabel.Location = new System.Drawing.Point(12, 333);
			this.gameTimeLabel.Name = "gameTimeLabel";
			this.gameTimeLabel.Size = new System.Drawing.Size(43, 13);
			this.gameTimeLabel.TabIndex = 4;
			this.gameTimeLabel.Text = "0:00:00";
			// 
			// realTimeLabel
			// 
			this.realTimeLabel.AutoSize = true;
			this.realTimeLabel.Location = new System.Drawing.Point(12, 290);
			this.realTimeLabel.Name = "realTimeLabel";
			this.realTimeLabel.Size = new System.Drawing.Size(43, 13);
			this.realTimeLabel.TabIndex = 5;
			this.realTimeLabel.Text = "0:00:00";
			// 
			// pausedTimeLabel
			// 
			this.pausedTimeLabel.AutoSize = true;
			this.pausedTimeLabel.Location = new System.Drawing.Point(12, 379);
			this.pausedTimeLabel.Name = "pausedTimeLabel";
			this.pausedTimeLabel.Size = new System.Drawing.Size(43, 13);
			this.pausedTimeLabel.TabIndex = 6;
			this.pausedTimeLabel.Text = "0:00:00";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label5.Location = new System.Drawing.Point(8, 208);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(142, 22);
			this.label5.TabIndex = 7;
			this.label5.Text = "Current Matches";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(8, 268);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(92, 22);
			this.label1.TabIndex = 8;
			this.label1.Text = "Real Time";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(8, 311);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(103, 22);
			this.label2.TabIndex = 9;
			this.label2.Text = "Game Time";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(8, 357);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(116, 22);
			this.label3.TabIndex = 10;
			this.label3.Text = "Paused Time";
			// 
			// pauseSegmentList
			// 
			this.pauseSegmentList.FormattingEnabled = true;
			this.pauseSegmentList.Location = new System.Drawing.Point(265, 299);
			this.pauseSegmentList.Name = "pauseSegmentList";
			this.pauseSegmentList.Size = new System.Drawing.Size(142, 121);
			this.pauseSegmentList.TabIndex = 11;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.Location = new System.Drawing.Point(261, 269);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(146, 22);
			this.label4.TabIndex = 12;
			this.label4.Text = "Pause Segments";
			// 
			// recordCheck
			// 
			this.recordCheck.AutoSize = true;
			this.recordCheck.Location = new System.Drawing.Point(413, 299);
			this.recordCheck.Name = "recordCheck";
			this.recordCheck.Size = new System.Drawing.Size(105, 17);
			this.recordCheck.TabIndex = 13;
			this.recordCheck.Text = "Record Features";
			this.recordCheck.UseVisualStyleBackColor = true;
			this.recordCheck.CheckedChanged += new System.EventHandler(this.recordCheck_CheckedChanged);
			// 
			// saveDiagnosticCheck
			// 
			this.saveDiagnosticCheck.AutoSize = true;
			this.saveDiagnosticCheck.Checked = true;
			this.saveDiagnosticCheck.CheckState = System.Windows.Forms.CheckState.Checked;
			this.saveDiagnosticCheck.Location = new System.Drawing.Point(413, 322);
			this.saveDiagnosticCheck.Name = "saveDiagnosticCheck";
			this.saveDiagnosticCheck.Size = new System.Drawing.Size(109, 17);
			this.saveDiagnosticCheck.TabIndex = 14;
			this.saveDiagnosticCheck.Text = "Save Diagnostics";
			this.saveDiagnosticCheck.UseVisualStyleBackColor = true;
			this.saveDiagnosticCheck.CheckedChanged += new System.EventHandler(this.saveDiagnosticCheck_CheckedChanged);
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label6.Location = new System.Drawing.Point(183, 208);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(249, 22);
			this.label6.TabIndex = 15;
			this.label6.Text = "Required Matches (for Pause)";
			// 
			// requiredMatchDisplayLabel
			// 
			this.requiredMatchDisplayLabel.AutoSize = true;
			this.requiredMatchDisplayLabel.Location = new System.Drawing.Point(184, 234);
			this.requiredMatchDisplayLabel.Name = "requiredMatchDisplayLabel";
			this.requiredMatchDisplayLabel.Size = new System.Drawing.Size(25, 13);
			this.requiredMatchDisplayLabel.TabIndex = 16;
			this.requiredMatchDisplayLabel.Text = "290";
			// 
			// recordCurrentButton
			// 
			this.recordCurrentButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.recordCurrentButton.Location = new System.Drawing.Point(117, 397);
			this.recordCurrentButton.Name = "recordCurrentButton";
			this.recordCurrentButton.Size = new System.Drawing.Size(116, 23);
			this.recordCurrentButton.TabIndex = 17;
			this.recordCurrentButton.Text = "Record Current Feature";
			this.recordCurrentButton.UseVisualStyleBackColor = true;
			this.recordCurrentButton.Click += new System.EventHandler(this.recordCurrentButton_Click);
			// 
			// resetButton
			// 
			this.resetButton.Location = new System.Drawing.Point(61, 397);
			this.resetButton.Name = "resetButton";
			this.resetButton.Size = new System.Drawing.Size(50, 23);
			this.resetButton.TabIndex = 18;
			this.resetButton.Text = "Reset";
			this.resetButton.UseVisualStyleBackColor = true;
			this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
			// 
			// button1
			// 
			this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button1.Location = new System.Drawing.Point(409, 369);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(109, 23);
			this.button1.TabIndex = 19;
			this.button1.Text = "Feature from File";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// processListBox
			// 
			this.processListBox.FormattingEnabled = true;
			this.processListBox.Location = new System.Drawing.Point(537, 299);
			this.processListBox.Name = "processListBox";
			this.processListBox.Size = new System.Drawing.Size(142, 121);
			this.processListBox.TabIndex = 20;
			this.processListBox.SelectedIndexChanged += new System.EventHandler(this.processListBox_SelectedIndexChanged);
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label7.Location = new System.Drawing.Point(533, 268);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(144, 22);
			this.label7.TabIndex = 21;
			this.label7.Text = "Process Capture";
			// 
			// CrashNSaneLoadDetector
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(682, 432);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.processListBox);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.resetButton);
			this.Controls.Add(this.recordCurrentButton);
			this.Controls.Add(this.requiredMatchDisplayLabel);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.saveDiagnosticCheck);
			this.Controls.Add(this.recordCheck);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.pauseSegmentList);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.pausedTimeLabel);
			this.Controls.Add(this.realTimeLabel);
			this.Controls.Add(this.gameTimeLabel);
			this.Controls.Add(this.pausedDisplay);
			this.Controls.Add(this.matchDisplayLabel);
			this.Controls.Add(this.imageDisplay);
			this.Controls.Add(this.startButton);
			this.Name = "CrashNSaneLoadDetector";
			this.Text = "Crash N Sane Trilogy Load Detector";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CrashNSaneLoadDetector_FormClosing);
			((System.ComponentModel.ISupportInitialize)(this.imageDisplay)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button startButton;
		private System.Windows.Forms.PictureBox imageDisplay;
		private System.Windows.Forms.Label matchDisplayLabel;
		private System.Windows.Forms.Panel pausedDisplay;
		private System.Windows.Forms.Label gameTimeLabel;
		private System.Windows.Forms.Label realTimeLabel;
		private System.Windows.Forms.Label pausedTimeLabel;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ListBox pauseSegmentList;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.CheckBox recordCheck;
		private System.Windows.Forms.CheckBox saveDiagnosticCheck;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label requiredMatchDisplayLabel;
		private System.Windows.Forms.Button recordCurrentButton;
		private System.Windows.Forms.Button resetButton;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.ListBox processListBox;
		private System.Windows.Forms.Label label7;
	}
}


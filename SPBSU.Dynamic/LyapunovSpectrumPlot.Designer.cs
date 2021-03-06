﻿namespace SPBSU.Dynamic {
	partial class LyapunovSpectrumPlot {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose ( bool disposing ) {
			if ( disposing && ( components != null ) ) {
				components.Dispose ();
			}
			base.Dispose ( disposing );
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent () {
			this.graphSystemOscillogram1 = new Graph.GraphSystemOscillogram();
			this.SuspendLayout();
			// 
			// graphSystemOscillogram1
			// 
			this.graphSystemOscillogram1.AxisXlabel = "x";
			this.graphSystemOscillogram1.AxisYlabel = "y";
			this.graphSystemOscillogram1.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
			this.graphSystemOscillogram1.GraphHist = false;
			this.graphSystemOscillogram1.IsAxisVisible = true;
			this.graphSystemOscillogram1.Location = new System.Drawing.Point(22, 12);
			this.graphSystemOscillogram1.MoveButtonsExist = true;
			this.graphSystemOscillogram1.Name = "graphSystemOscillogram1";
			this.graphSystemOscillogram1.Scatter = false;
			this.graphSystemOscillogram1.Size = new System.Drawing.Size(934, 369);
			this.graphSystemOscillogram1.TabIndex = 0;
			this.graphSystemOscillogram1.ZoomButtonsExist = true;
			// 
			// LyapunovSpectrumPlot
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(987, 459);
			this.Controls.Add(this.graphSystemOscillogram1);
			this.Name = "LyapunovSpectrumPlot";
			this.Text = "LyapunovSpectrumPlot";
			this.ResumeLayout(false);

		}

		#endregion

		public Graph.GraphSystemOscillogram graphSystemOscillogram1;

	}
}
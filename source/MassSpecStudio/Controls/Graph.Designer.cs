namespace MassSpecStudio.UI.Controls
{
	public partial class Graph
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.ZedGraphControl = new ZedGraph.ZedGraphControl();
			this.SuspendLayout();
			// 
			// ZedGraphControl
			// 
			this.ZedGraphControl.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.ZedGraphControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ZedGraphControl.Location = new System.Drawing.Point(0, 0);
			this.ZedGraphControl.Margin = new System.Windows.Forms.Padding(3, 3, 3, 50);
			this.ZedGraphControl.Name = "ZedGraphControl";
			this.ZedGraphControl.ScrollGrace = 0D;
			this.ZedGraphControl.ScrollMaxX = 0D;
			this.ZedGraphControl.ScrollMaxY = 0D;
			this.ZedGraphControl.ScrollMaxY2 = 0D;
			this.ZedGraphControl.ScrollMinX = 0D;
			this.ZedGraphControl.ScrollMinY = 0D;
			this.ZedGraphControl.ScrollMinY2 = 0D;
			this.ZedGraphControl.Size = new System.Drawing.Size(150, 150);
			this.ZedGraphControl.TabIndex = 1;
			this.ZedGraphControl.Load += new System.EventHandler(this.ZedGraphControl1_Load);
			// 
			// Graph
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.ZedGraphControl);
			this.Name = "Graph";
			this.ResumeLayout(false);

		}

		#endregion

		public ZedGraph.ZedGraphControl ZedGraphControl;
	}
}

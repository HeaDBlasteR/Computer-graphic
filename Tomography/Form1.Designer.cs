namespace Tomography
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            menuStrip1 = new MenuStrip();
            открытьToolStripMenuItem = new ToolStripMenuItem();
            glControl2 = new OpenTK.GLControl.GLControl();
            trackBar1 = new TrackBar();
            checkBox1 = new CheckBox();
            checkBox2 = new CheckBox();
            trackBarMin = new TrackBar();
            trackBarWidth = new TrackBar();
            menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBar1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarMin).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarWidth).BeginInit();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(32, 32);
            menuStrip1.Items.AddRange(new ToolStripItem[] { открытьToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1974, 40);
            menuStrip1.TabIndex = 1;
            menuStrip1.Text = "menuStrip1";
            // 
            // открытьToolStripMenuItem
            // 
            открытьToolStripMenuItem.Name = "открытьToolStripMenuItem";
            открытьToolStripMenuItem.Size = new Size(127, 36);
            открытьToolStripMenuItem.Text = "Открыть";
            открытьToolStripMenuItem.Click += открытьToolStripMenuItem_Click;
            // 
            // glControl2
            // 
            glControl2.API = OpenTK.Windowing.Common.ContextAPI.OpenGL;
            glControl2.APIVersion = new Version(3, 3, 0, 0);
            glControl2.Flags = OpenTK.Windowing.Common.ContextFlags.Default;
            glControl2.IsEventDriven = true;
            glControl2.Location = new Point(0, 203);
            glControl2.Name = "glControl2";
            glControl2.Profile = OpenTK.Windowing.Common.ContextProfile.Core;
            glControl2.SharedContext = null;
            glControl2.Size = new Size(1974, 896);
            glControl2.TabIndex = 2;
            glControl2.Paint += glControl2_Paint;
            // 
            // trackBar1
            // 
            trackBar1.Location = new Point(0, 53);
            trackBar1.Name = "trackBar1";
            trackBar1.Size = new Size(1778, 90);
            trackBar1.TabIndex = 3;
            trackBar1.Scroll += trackBar1_Scroll;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(1784, 53);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(114, 36);
            checkBox1.TabIndex = 4;
            checkBox1.Text = "Quads";
            checkBox1.CheckedChanged += checkBox1_CheckedChanged;
            // 
            // checkBox2
            // 
            checkBox2.AutoSize = true;
            checkBox2.Location = new Point(1784, 107);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new Size(124, 36);
            checkBox2.TabIndex = 5;
            checkBox2.Text = "Texture";
            checkBox2.UseVisualStyleBackColor = true;
            checkBox2.CheckedChanged += checkBox2_CheckedChanged;
            // 
            // trackBarMin
            // 
            trackBarMin.Location = new Point(0, 107);
            trackBarMin.Name = "trackBarMin";
            trackBarMin.Size = new Size(908, 90);
            trackBarMin.TabIndex = 6;
            trackBarMin.Scroll += trackBarMin_Scroll;
            // 
            // trackBarWidth
            // 
            trackBarWidth.Location = new Point(914, 107);
            trackBarWidth.Name = "trackBarWidth";
            trackBarWidth.Size = new Size(864, 90);
            trackBarWidth.TabIndex = 7;
            trackBarWidth.Scroll += trackBarWidth_Scroll;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1974, 1099);
            Controls.Add(trackBarWidth);
            Controls.Add(trackBarMin);
            Controls.Add(checkBox2);
            Controls.Add(checkBox1);
            Controls.Add(trackBar1);
            Controls.Add(glControl2);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "Form1";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackBar1).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarMin).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarWidth).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private MenuStrip menuStrip1;
        private ToolStripMenuItem открытьToolStripMenuItem;
        private OpenTK.GLControl.GLControl glControl2;
        private TrackBar trackBar1;
        private CheckBox checkBox1;
        private CheckBox checkBox2;
        private TrackBar trackBarMin;
        private TrackBar trackBarWidth;
    }
}

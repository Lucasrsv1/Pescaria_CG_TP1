using SharpGL;

namespace Pescaria_CG_TP1 {
	partial class OpenGLForm {
		/// <summary>
		/// Variável de designer necessária.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Limpar os recursos que estão sendo usados.
		/// </summary>
		/// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
		protected override void Dispose (bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Código gerado pelo Windows Form Designer

		/// <summary>
		/// Método necessário para suporte ao Designer - não modifique 
		/// o conteúdo deste método com o editor de código.
		/// </summary>
		private void InitializeComponent () {
			this.openGLControl1 = new SharpGL.OpenGLControl();
			((System.ComponentModel.ISupportInitialize)(this.openGLControl1)).BeginInit();
			this.SuspendLayout();
			// 
			// openGLControl1
			// 
			this.openGLControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.openGLControl1.DrawFPS = false;
			this.openGLControl1.FrameRate = 28;
			this.openGLControl1.Location = new System.Drawing.Point(0, 0);
			this.openGLControl1.Margin = new System.Windows.Forms.Padding(0);
			this.openGLControl1.Name = "openGLControl1";
			this.openGLControl1.OpenGLVersion = SharpGL.Version.OpenGLVersion.OpenGL2_1;
			this.openGLControl1.RenderContextType = SharpGL.RenderContextType.FBO;
			this.openGLControl1.RenderTrigger = SharpGL.RenderTrigger.TimerBased;
			this.openGLControl1.Size = new System.Drawing.Size(800, 600);
			this.openGLControl1.TabIndex = 0;
			this.openGLControl1.OpenGLInitialized += new System.EventHandler(this.openGLControl_OpenGLInitialized);
			this.openGLControl1.OpenGLDraw += new SharpGL.RenderEventHandler(this.openGLControl1_OpenGLDraw);
			this.openGLControl1.Resized += new System.EventHandler(this.openGLControl_Resized);
			this.openGLControl1.Click += new System.EventHandler(this.openGLControl1_Click);
			this.openGLControl1.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.openGLControl1_PreviewKeyDown);
			// 
			// OpenGLForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(799, 598);
			this.Controls.Add(this.openGLControl1);
			this.Name = "OpenGLForm";
			this.Text = "Form1";
			((System.ComponentModel.ISupportInitialize)(this.openGLControl1)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		public OpenGLControl openGLControl1;
	}
}


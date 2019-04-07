using SharpGL;
using SharpGL.Enumerations;
using System;
using System.Drawing;
using System.Windows.Forms;
using Pescaria_CG_TP1.Engine;

namespace Pescaria_CG_TP1 {
	public partial class OpenGLForm : Form {
		public OpenGLForm () {
			InitializeComponent();
		}

		private int glWidth, glHeight;

		private OpenGL gl;
		private GameObject fish1;

		private void openGLControl_OpenGLInitialized (object sender, EventArgs e) {
			// Salva a referência ao objeto do OpenGL
			gl = openGLControl1.OpenGL;

			// Cor de fundo
			gl.ClearColor(0, 0, 0, 0);

			fish1 = new GameObject(gl, new Vector2(100, 100));
			Bitmap textureImage = new Bitmap("./Textures/FISH1_SWIM_RIGHT.png");
			fish1.Animator.AddTexture("FISH1_SWIM_RIGHT", textureImage, 6, 600);

			Bitmap textureImage2 = new Bitmap("./Textures/FISH1_SWIM_LEFT.png");
			fish1.Animator.AddTexture("FISH1_SWIM_LEFT", textureImage2, 6, 600);

			fish1.Animator.CurrentTexture = "FISH1_SWIM_RIGHT";
			fish1.Transform.Translate(new Vector2(300, 0), 1500);
			fish1.Animator.LoadTexture("FISH1_SWIM_LEFT", 1500);
		}

		private void openGLControl_Resized (object sender, EventArgs e) {
			gl.MatrixMode(MatrixMode.Modelview);

			// Carrega a matriz identidade.
			gl.LoadIdentity();

			// Configura os eixos coordenados
			glWidth = openGLControl1.Width;
			glHeight = openGLControl1.Height;
			gl.Ortho(0, glWidth, glHeight, 0, -5, 5);
		}

		private void openGLControl1_OpenGLDraw (object sender, RenderEventArgs e) {
			gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);          // Clear The Screen And The Depth Buffer

			// Carrega a matriz identidade.
			gl.LoadIdentity();

			// Configura os eixos coordenados
			glWidth = openGLControl1.Width;
			glHeight = openGLControl1.Height;
			gl.Ortho(0, glWidth, glHeight, 0, -5, 5);

			// Draw objects
			fish1.OpenGLDraw(glWidth, glHeight);

			gl.Flush();
		}

		private void openGLControl1_PreviewKeyDown (object sender, PreviewKeyDownEventArgs e) {
			if (e.KeyCode == Keys.Escape)
				Application.Exit();
		}
	}
}

using SharpGL;
using SharpGL.Enumerations;
using System;
using System.Windows.Forms;
using Pescaria_CG_TP1.Engine;
using Pescaria_CG_TP1.Scenes;

namespace Pescaria_CG_TP1 {
	public partial class OpenGLForm : Form {
		public OpenGLForm () {
			InitializeComponent();
			SceneManager.Form = this;
		}

		private int glWidth;
		private int glHeight;
		private OpenGL gl;

		private void openGLControl_OpenGLInitialized (object sender, EventArgs e) {
			// Salva a referência ao objeto do OpenGL
			gl = openGLControl1.OpenGL;
			LoadCoordinateSystem();
			SceneManager.ScreenSize.X = glWidth;
			SceneManager.ScreenSize.Y = glHeight;

			SceneManager.RegisterNewScene("GAME", new Game(gl));
			SceneManager.LoadScene("GAME");
		}

		private void LoadCoordinateSystem () {
			// Carrega a matriz identidade.
			gl.LoadIdentity();

			// Configura os eixos coordenados
			glWidth = openGLControl1.Width;
			glHeight = openGLControl1.Height;
			gl.Ortho(0, glWidth, glHeight, 0, -5, 5);
		}

		private void openGLControl_Resized (object sender, EventArgs e) {
			gl.MatrixMode(MatrixMode.Modelview);
			LoadCoordinateSystem();
		}

		private void openGLControl1_OpenGLDraw (object sender, RenderEventArgs e) {
			// Limpa o buffer da tela
			gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
			LoadCoordinateSystem();

			// Desenha os objetos
			SceneManager.OpenGLDraw(glWidth, glHeight);
			gl.Flush();
		}

		private void openGLControl1_Click (object sender, EventArgs e) {
			SceneManager.MouseClick();
		}

		private void openGLControl1_PreviewKeyDown (object sender, PreviewKeyDownEventArgs e) {
			if (e.KeyCode == Keys.Escape)
				Application.Exit();
			else if (e.KeyCode == Keys.P)
				SceneManager.Pause();

			if (SceneManager.Player != null)
				SceneManager.Player.PreviewKeyDown(e);
		}
	}
}

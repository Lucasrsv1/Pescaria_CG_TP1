using SharpGL;
using SharpGL.Enumerations;
using System;
using System.Windows.Forms;
using Pescaria_CG_TP1.Engine;
using Pescaria_CG_TP1.Scenes;
using Pescaria_CG_TP1.Prefabs;

namespace Pescaria_CG_TP1 {
	public partial class OpenGLForm : Form {
		public OpenGLForm () {
			InitializeComponent();
			this.WindowState = FormWindowState.Maximized;
			SceneManager.Form = this;
			SceneManager.IsMute = false;
		}

		private int glWidth;
		private int glHeight;
		private OpenGL gl;

		private void openGLControl_OpenGLInitialized (object sender, EventArgs e) {
			// Save a reference to the OpenGL object
			gl = openGLControl1.OpenGL;
			Animator.gl = gl;
			GameObject.gl = gl;
			SceneManager.gl = gl;

			LoadCoordinateSystem();

			SceneManager.ScreenSize.X = glWidth;
			SceneManager.ScreenSize.Y = glHeight;

			SceneManager.RegisterNewScene("SPLASH_SCREEN", new SplashScreen(gl));
			SceneManager.RegisterNewScene("MENU", new Scenes.Menu(gl));
			SceneManager.RegisterNewScene("STORY_MANAGER", new StoryManager(gl));
			SceneManager.RegisterNewScene("GAME", new Game(gl));
			SceneManager.LoadScene("SPLASH_SCREEN");
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
			if (e.KeyCode == Keys.Escape) {
				if (SceneManager.CurrentScene == "MENU") {
					if (!((MenuHUD) SceneManager.HUD).ShowAbout)
						Application.Exit();
					else
						((MenuHUD) SceneManager.HUD).HideAbout();
				} else {
					SceneManager.LoadScene("MENU");
				}
			} else if (e.KeyCode == Keys.P && SceneManager.CurrentScene == "GAME") {
				SceneManager.Pause();
			} else if (e.KeyCode == Keys.R && !GameHUD.IsNewScore && SceneManager.CurrentScene == "GAME") {
				SceneManager.ReleadLevel();
			} else if ((e.KeyCode == Keys.Return || e.KeyCode == Keys.Space) && SceneManager.CurrentScene == "STORY_MANAGER") {
				StoryManager.NextSpeech();
			}

			if (SceneManager.Player != null)
				SceneManager.Player.PreviewKeyDown(e);
		}
	}
}

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
			SceneManager.Form = this;
		}

		private int glWidth, glHeight;

		private OpenGL gl;

		private void openGLControl_OpenGLInitialized (object sender, EventArgs e) {
			// Salva a referência ao objeto do OpenGL
			gl = openGLControl1.OpenGL;

			// Cor de fundo
			gl.ClearColor(0, 0, 0, 0);

			for (int i = 0; i < 5; i++) {
				GameObject fish = new GameObject(gl, new Vector2(100, 100), new Vector2(0, 150 * i));
				SceneManager.AddObject(fish);

				fish.AddOnClickListener(fish.Destroy);
				if (i == 2)
					fish.InteractiveDuringPause = true;

				fish.Animator.AddTexture("FISH1_SWIM_RIGHT", new Bitmap("./Textures/FISH1_SWIM_RIGHT.png"), 6, 600);
				fish.Animator.AddTexture("FISH1_REST_RIGHT", new Bitmap("./Textures/FISH1_REST_RIGHT.png"), 6, 600);
				fish.Animator.AddTexture("FISH1_SWIM_LEFT", new Bitmap("./Textures/FISH1_SWIM_LEFT.png"), 6, 600);
				fish.Animator.AddTexture("FISH1_REST_LEFT", new Bitmap("./Textures/FISH1_REST_LEFT.png"), 6, 600);

				AnimationClip animationClip = new AnimationClip(AnimationClip.ClipTypes.LOOP);
				animationClip.AddClipPoint(1000, "FISH1_SWIM_RIGHT", new Vector2(200, 0));
				animationClip.AddClipPoint(1000);
				animationClip.AddClipPoint(1000, "FISH1_SWIM_RIGHT", new Vector2(200, 0));
				animationClip.AddClipPoint(1000, "FISH1_REST_RIGHT");
				animationClip.AddClipPoint(1000, "FISH1_SWIM_LEFT", new Vector2(-200, 0));
				animationClip.AddClipPoint(1000);
				animationClip.AddClipPoint(1000, "FISH1_SWIM_LEFT", new Vector2(-200, 0));
				animationClip.AddClipPoint(1000, "FISH1_REST_LEFT");
				fish.Animator.AddAnimationClip("CLIP_1", animationClip);

				fish.Animator.PlayAnimationClip("CLIP_1");
			}
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
		}
	}
}

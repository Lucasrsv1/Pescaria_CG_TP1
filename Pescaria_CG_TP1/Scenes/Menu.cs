///-----------------------------------------------------------------
///   Namespace:		Pescaria_CG_TP1.Scenes
///   Class:			Menu
///   Description:		Creates the menu scene and all its objects.
///   Subject:			Computer Graphics
///   Author:			Lucas Rassilan Vilanova
///-----------------------------------------------------------------

using NAudio.Wave;
using Pescaria_CG_TP1.Engine;
using Pescaria_CG_TP1.Prefabs;
using SharpGL;
using SharpGL.Enumerations;
using System.Drawing;

namespace Pescaria_CG_TP1.Scenes {
	public class Menu : IScene {
		private static bool keepPlaying;
		private static readonly WaveOutEvent OutputDevice = new WaveOutEvent();
		private static readonly AudioFileReader audioFile = new AudioFileReader("./Audio/Menu.mp3");

		public Menu (OpenGL gl) {
			this.gl = gl;
		}

		private OpenGL gl;
		private MenuHUD menuHUD;

		public void InitScene () {
			GameObject background = new GameObject(new Vector2(SceneManager.ScreenSize.X, 400, SceneManager.ScreenSize.X, 0), "", new Vector2(0, 250));
			background.Animator.AddTexture("BACKGROUND", new Bitmap("./Textures/BACKGROUND.png"), 3, 2000);
			background.Animator.CurrentTexture = "BACKGROUND";
			SceneManager.AddObject(background);

			GameObject sun = new GameObject(new Vector2(230, 300), "Sun", new Vector2(10, -25));
			sun.Animator.AddTexture("SUN", new Bitmap("./Textures/SUN.png"));
			sun.Animator.CurrentTexture = "SUN";
			SceneManager.AddObject(sun);

			menuHUD = new MenuHUD(gl);
			menuHUD.Init();
			SceneManager.HUD = menuHUD;

			// Play games' song
			long position;
			try {
				position = OutputDevice.GetPosition();
			} catch {
				position = 0;
			}

			if (position == 0) {
				keepPlaying = true;
				OutputDevice.PlaybackStopped += SongStopped;
				OutputDevice.Init(audioFile);
				OutputDevice.Play();
			}
		}

		public void DisposeScene () {
			keepPlaying = false;
			OutputDevice.Stop();
			OutputDevice.PlaybackStopped -= SongStopped;
			audioFile.Seek(0, System.IO.SeekOrigin.Begin);
		}

		private void SongStopped (object sender, StoppedEventArgs e) {
			if (keepPlaying) {
				audioFile.Seek(0, System.IO.SeekOrigin.Begin);
				OutputDevice.Play();
			}
		}

		public void OpenGLDraw (int glWidth, int glHeight) {
			// Background color
			gl.ClearColor(6 / 255f, 132 / 255f, 152 / 255f, 0f);	// Ocean
			
			// Sky top
			gl.Begin(BeginMode.TriangleFan);
				gl.Color(64 / 255f, 154 / 255f, 246 / 255f);
				gl.Vertex(0, SceneManager.ScreenSize.Y / -2);
				gl.Vertex(SceneManager.ScreenSize.X, SceneManager.ScreenSize.Y / -2);
				gl.Vertex(SceneManager.ScreenSize.X, 250);
				gl.Vertex(0, 250);
			gl.End();
			gl.Color(1f, 1f, 1f);
		}
	}
}

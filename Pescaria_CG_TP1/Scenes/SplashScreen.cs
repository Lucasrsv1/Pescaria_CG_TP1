///-----------------------------------------------------------------
///   Namespace:		Pescaria_CG_TP1.Scenes
///   Class:			SplashScreen
///   Description:		Creates the splash screen scene and animation.
///   Subject:			Computer Graphics
///   Author:			Lucas Rassilan Vilanova
///-----------------------------------------------------------------

using Pescaria_CG_TP1.Engine;
using SharpGL;
using System;
using System.Drawing;
using System.Media;

namespace Pescaria_CG_TP1.Scenes {
	public class SplashScreen : IScene {
		private static readonly SoundPlayer transition = new SoundPlayer("./Audio/Transition.wav");

		public SplashScreen (OpenGL gl) {
			this.gl = gl;
		}

		private OpenGL gl;
		private DateTime start;
		private GameObject background;

		private readonly int animationDuration = 2000;
		private readonly int idleDuration = 3000;

		public void InitScene () {
			background = new GameObject(new Vector2(2560, 1440, 2560, 1440));
			background.Animator.AddTexture("LRV", new Bitmap("./Textures/LRV.png"));
			background.Animator.CurrentTexture = "LRV";
			background.Animator.Color = Color.FromArgb(0, background.Animator.Color);
			SceneManager.AddObject(background);
			start = SceneManager.Now;
			transition.Play();
		}

		public void DisposeScene () {}

		public void OpenGLDraw (int glWidth, int glHeight) {
			double timePassed = SceneManager.Now.Subtract(start).TotalMilliseconds;
			background.Animator.Color = Color.FromArgb((int) Math.Min(255, 255 * timePassed / animationDuration), background.Animator.Color);

			if (timePassed > animationDuration + idleDuration)
				SceneManager.LoadScene("MENU");
		}
	}
}

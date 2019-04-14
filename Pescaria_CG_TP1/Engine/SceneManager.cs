using Pescaria_CG_TP1.Scenes;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Pescaria_CG_TP1.Engine {
	public static class SceneManager {
		public const bool SHOW_COLLIDERS = false;

		private static bool paused = false;
		private static string currentScene = "";
		private static TimeSpan pauseDelay = new TimeSpan();
		private static DateTime pausedTime;
		private static Dictionary<string, IScene> scenes = new Dictionary<string, IScene>();

		public static OpenGLForm Form = null;
		public static Vector2 ScreenSize = Vector2.Zero;

		public static IHUD HUD = null;
		public static PlayerObject Player = null;
		public static List<GameObject> SceneObjects = new List<GameObject>();

		public static bool IsPaused {
			get { return paused; }
		}

		public static void Pause () {
			paused = !paused;
			if (!paused)
				// If it's not paused, the game just continued, therefor sum the pause delay
				pauseDelay += SceneManager.Now.Subtract(pausedTime);
			else
				// Save the moment when the game was paused
				pausedTime = DateTime.Now.Subtract(pauseDelay);
		}

		public static DateTime Now {
			get {
				if (paused)
					// Returns the moment when the game was paused, freezing the game
					return pausedTime;
				else
					// Returns the current moment less the pause delay
					return DateTime.Now.Subtract(pauseDelay);
			}
		}

		public static void RegisterNewScene (string sceneKey, IScene scene) {
			scenes.Add(sceneKey, scene);
		}

		public static void LoadScene (string sceneKey) {
			paused = false;
			pauseDelay = new TimeSpan();
			SceneObjects.Clear();
			currentScene = sceneKey;
			scenes[currentScene].InitScene();
		}

		public static void ReleadLevel () {
			LoadScene(currentScene);
		}

		public static void AddObject (GameObject obj) {
			SceneObjects.Add(obj);
		}

		public static void RemoveObject (GameObject obj) {
			SceneObjects.Remove(obj);
		}

		public static void OpenGLDraw (int glWidth, int glHeight) {
			ScreenSize.X = glWidth;
			ScreenSize.Y = glHeight;

			if (scenes.ContainsKey(currentScene))
				scenes[currentScene].OpenGLDraw(glWidth, glHeight);

			for (int o = 0; o < SceneObjects.Count; o++) {
				if (SceneObjects[o].HasCollisionListeners()) {
					for (int b = 0; b < SceneObjects.Count; b++) {
						if (Vector2.Overlap(SceneObjects[o].Transform, SceneObjects[b].Transform))
							SceneObjects[o].CollisionDetected(SceneObjects[b]);
					}
				}
				SceneObjects[o].OpenGLDraw(glWidth, glHeight);
			}

			HUD.OpenGLDraw(glWidth, glHeight);
		}

		public static void MouseClick () {
			Vector2 mousePos = new Vector2(Form.openGLControl1.PointToClient(Cursor.Position));
			for (int i = SceneObjects.Count - 1; i >= 0; i--) {
				if (!SceneObjects[i].HasClickListeners() || (paused && !SceneObjects[i].InteractiveDuringPause)) continue;

				// Fix cursor position due to the camera moviment
				float posY = -Game.CameraYPosition;

				// If the object was clicked, call its callback
				if (Vector2.Overlap(SceneObjects[i].Transform, mousePos + new Vector2(0, posY))) {
					SceneObjects[i].Clicked();
					break;
				}
			}
		}
	}
}

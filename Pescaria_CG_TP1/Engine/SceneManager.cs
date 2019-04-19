using Pescaria_CG_TP1.Scenes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Pescaria_CG_TP1.Engine {
	public static class SceneManager {
		// Show physical colliders (debug)
		public const bool SHOW_COLLIDERS = false;

		private static bool mute = false;
		private static string currentScene = "";
		private static TimeSpan pauseDelay = new TimeSpan();
		private static DateTime pausedTime;
		private static Dictionary<string, IScene> scenes = new Dictionary<string, IScene>();

		private static string ReverseString (string s) {
			char[] array = s.ToCharArray();
			Array.Reverse(array);
			return new string(array);
		}

		private static string Descrypt (string input, string source, string target) {
			input = ReverseString(input);
			long d = 0;
			for (int i = 0; i < input.Length; i++)
				d += source.IndexOf(input[i]) * (long) Math.Pow(source.Length, i);

			int pos;
			string result = "";
			while (d >= target.Length) {
				pos = (int) (d % target.Length);
				result += target[pos];
				d = (d - pos) / target.Length;
			}
			result += target[(int) d];
			return ReverseString(result);
		}

		public static OpenGLForm Form = null;
		public static Vector2 ScreenSize = Vector2.Zero;

		public static IHUD HUD = null;
		public static GameObject Aim = null;
		public static PlayerObject Player = null;
		public static List<GameObject> SceneObjects = new List<GameObject>();

		public static bool IsPaused { get; private set; } = false;

		public static bool IsMute {
			get {
				return mute;
			}
			set {
				Game.OutputDevice.Volume = value ? 0 : 1;
				mute = value;
			}
		}

		public static void Pause () {
			if (Game.GameEnded) return;
			IsPaused = !IsPaused;
			if (!IsPaused)
				// If it's not paused, the game just continued, therefor sum the pause delay
				pauseDelay += SceneManager.Now.Subtract(pausedTime);
			else
				// Save the moment when the game was paused
				pausedTime = DateTime.Now.Subtract(pauseDelay);
		}

		public static DateTime Now {
			get {
				if (IsPaused)
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
			IsPaused = false;
			pauseDelay = new TimeSpan();
			SceneObjects.Clear();

			// Remove old scene
			if (!string.IsNullOrEmpty(currentScene) && currentScene != sceneKey)
				scenes[currentScene].DisposeScene();

			// Initiate the new scene
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

			try {
				for (int o = 0; o < SceneObjects.Count; o++) {
					if (SceneObjects[o].HasCollisionListeners()) {
						for (int b = 0; b < SceneObjects.Count; b++) {
							if (Vector2.Overlap(SceneObjects[o].Transform, SceneObjects[b].Transform))
								SceneObjects[o].CollisionDetected(SceneObjects[b]);
						}
					}
					SceneObjects[o].OpenGLDraw(glWidth, glHeight);
				}
			} catch { }

			HUD.OpenGLDraw(glWidth, glHeight);
			if (Aim != null)
				Aim.OpenGLDraw(glWidth, glHeight);
		}

		public static Vector2 MousePositionInScene () {
			Vector2 mousePos = new Vector2(Form.openGLControl1.PointToClient(Cursor.Position));

			// Fix cursor position due to the camera moviment
			float posY = -Game.CameraYPosition;
			return mousePos + new Vector2(0, posY);
		}

		public static void MouseClick () {
			for (int i = SceneObjects.Count - 1; i >= 0; i--) {
				if (!SceneObjects[i].HasClickListeners() || (IsPaused && !SceneObjects[i].InteractiveDuringPause)) continue;

				// If the object was clicked, call its callback
				if (SceneObjects[i].Tag == "Bubble") {
					// Bubbles are small, so consider the aim object area insted of the cursor point
					if (!Game.GameEnded && Vector2.Overlap(SceneObjects[i].Transform, Aim.Transform)) {
						SceneObjects[i].Clicked();
						break;
					}
				} else if (Vector2.Overlap(SceneObjects[i].Transform, MousePositionInScene())) {
					SceneObjects[i].Clicked();
					break;
				}
			}
		}

		public static string[] ReadFile (string path) {
			if (!File.Exists(path))
				File.WriteAllLines(path, new string[] { "" });

			List<string> result = new List<string>();
			StreamReader reader = new StreamReader(path);
			while (!reader.EndOfStream) {
				string line = "";
				string[] lineParts = reader.ReadLine().Split('.');
				for (int i = 0; i < lineParts.Length; i++) {
					if (!string.IsNullOrEmpty(lineParts[i]))
						line += Descrypt(lineParts[i], "LRV", "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890.; ");
				}

				result.Add(line);
			}
			reader.Close();
			return result.ToArray();
		}

		public static void WriteFile (string path, string[] lines) {
			string[] linesCopy = new string[lines.Length];
			lines.CopyTo(linesCopy, 0);

			if (!File.Exists(path))
				File.WriteAllLines(path, new string[] { "" });

			StreamWriter writer = new StreamWriter(path);
			for (int i = 0; i < linesCopy.Length; i++) {
				List<string> lineParts = new List<string>();
				while (linesCopy[i].Length > 0) {
					lineParts.Add(linesCopy[i].Substring(0, Math.Min(5, linesCopy[i].Length)));
					linesCopy[i] = linesCopy[i].Substring(Math.Min(5, linesCopy[i].Length));
				}

				string line = "";
				for (int j = 0; j < lineParts.Count; j++)
					line += Descrypt(lineParts[j], "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890.; ", "LRV") + ".";

				writer.WriteLine(line);
			}
			writer.Flush();
			writer.Close();
		}
	}
}

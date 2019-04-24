///-----------------------------------------------------------------
///   Namespace:		Pescaria_CG_TP1.Engine
///   Class:			SceneManager
///   Description:		Load scenes, draw objects and the HUD, check collisions and clicks.
///   Subject:			Computer Graphics
///   Author:			Lucas Rassilan Vilanova
///-----------------------------------------------------------------

using Pescaria_CG_TP1.Scenes;
using SharpGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace Pescaria_CG_TP1.Engine {
	public static class SceneManager {
		// Show physical colliders (debug)
		public const bool SHOW_COLLIDERS = false;

		private static readonly Color loadingBackgroundColor = Color.FromArgb(6, 132, 152);
		private static readonly Vector2 audioSize = new Vector2(40, 40);
		private static readonly GameObject audioIcon = new GameObject(audioSize, "AudioIcon");
		private static readonly Bitmap[] AUDIO_TEXTURES = new Bitmap[2] {
			new Bitmap("./Textures/VOLUME.png"),
			new Bitmap("./Textures/MUTE.png")
		};

		private static int PADDING = 10;
		private static bool texturesRegistered = false;
		private static uint[] AUDIO_TEXTURES_IDS = new uint[AUDIO_TEXTURES.Length];
		private static Texture[] audioTextures = new Texture[AUDIO_TEXTURES.Length];

		private static bool mute = false;
		private static bool canLoadScene = false;
		private static TimeSpan pauseDelay = new TimeSpan();
		private static DateTime pausedTime;
		private static Dictionary<string, IScene> scenes = new Dictionary<string, IScene>();

		public static OpenGL gl;
		public static bool IsLoadingScene = false;
		public static string CurrentScene = "";

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
			IsLoadingScene = true;
			canLoadScene = CurrentScene == sceneKey;

			if (!texturesRegistered) {
				for (int i = 0; i < AUDIO_TEXTURES.Length; i++) {
					AUDIO_TEXTURES_IDS[i] = Animator.RegisterTexture(AUDIO_TEXTURES[i]);
					audioTextures[i] = new Texture(gl, AUDIO_TEXTURES_IDS[i], 1);
				}
				texturesRegistered = true;

				audioIcon.Animator.AddTexture("VOLUME", AUDIO_TEXTURES_IDS[0]);
				audioIcon.Animator.AddTexture("MUTE", AUDIO_TEXTURES_IDS[1]);
				audioIcon.Animator.CurrentTexture = "VOLUME";
				audioIcon.Animator.ZIndex = 4;

				audioIcon.Transform.SetPositionFn(() => {
					audioIcon.Transform.Position.X = ScreenSize.X - audioSize.X - PADDING;
					audioIcon.Transform.Position.Y = ScreenSize.Y - audioSize.Y - PADDING - (CurrentScene == "GAME" ? Game.CameraYPosition : 0);
				});
				audioIcon.AddOnClickListener(() => {
					IsMute = !IsMute;
					audioIcon.Animator.CurrentTexture = !IsMute ? "VOLUME" : "MUTE";
				});
			}

			IsPaused = false;
			pauseDelay = new TimeSpan();
			for (int i = 0; i < SceneObjects.Count; i++)
				SceneObjects[i].Destroy();

			SceneObjects.Clear();

			HUD = null;
			Aim = null;
			Player = null;

			// Remove old scene
			if (!string.IsNullOrEmpty(CurrentScene) && CurrentScene != sceneKey)
				scenes[CurrentScene].DisposeScene();

			// Initiate the new scene
			CurrentScene = sceneKey;

			if (!canLoadScene) {
				// Show the loading screen before starts to load the scene
				new Thread(new ThreadStart(() => {
					Thread.Sleep(100);
					canLoadScene = true;
				})).Start();
			}
		}

		public static void ReleadLevel () {
			LoadScene(CurrentScene);
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

			if (IsLoadingScene) {
				gl.ClearColor(loadingBackgroundColor.R / 255f, loadingBackgroundColor.G / 255f, loadingBackgroundColor.B / 255f, 0f);
				gl.DrawText((int) ((ScreenSize.X - 26 * 7) / 2f), (int) (ScreenSize.Y / 2f) - 26, 1, 1, 1, "Arial", 26, "Carregando...");

				if (canLoadScene) {
					scenes[CurrentScene].InitScene();
					IsLoadingScene = false;
				}
				return;
			}

			if (scenes.ContainsKey(CurrentScene))
				scenes[CurrentScene].OpenGLDraw(glWidth, glHeight);

			try {
				Vector2 mousePosition = MousePositionInScene();
				for (int o = 0; o < SceneObjects.Count; o++) {
					if (SceneObjects[o].HasCollisionListeners()) {
						for (int b = 0; b < SceneObjects.Count; b++) {
							if (Vector2.Overlap(SceneObjects[o].Transform, SceneObjects[b].Transform))
								SceneObjects[o].CollisionDetected(SceneObjects[b]);
						}
					}

					// Check mouse enter and leave events
					if (Vector2.Overlap(SceneObjects[o].Transform, mousePosition))
						SceneObjects[o].Hover();
					else
						SceneObjects[o].MouseOut();

					SceneObjects[o].OpenGLDraw(glWidth, glHeight);
				}
			} catch { }

			if (HUD != null)
				HUD.OpenGLDraw(glWidth, glHeight);

			if (Aim != null)
				Aim.OpenGLDraw(glWidth, glHeight);

			if (audioIcon != null)
				audioIcon.OpenGLDraw(glWidth, glHeight);
		}

		public static Vector2 MousePositionInScene () {
			Vector2 mousePos = new Vector2(Form.openGLControl1.PointToClient(Cursor.Position));

			// Fix cursor position due to the camera moviment
			float posY = CurrentScene == "GAME" ? Game.CameraYPosition : 0;
			return mousePos - new Vector2(0, posY);
		}

		public static void MouseClick () {
			if (IsLoadingScene) return;
			for (int i = SceneObjects.Count - 1; i >= 0; i--) {
				if (!SceneObjects[i].HasClickListeners() || (IsPaused && !SceneObjects[i].InteractiveDuringPause) || SceneObjects[i].IsHidden) continue;

				// If the object was clicked, calls its callback
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

			if (Vector2.Overlap(audioIcon.Transform, MousePositionInScene()))
				audioIcon.Clicked();
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

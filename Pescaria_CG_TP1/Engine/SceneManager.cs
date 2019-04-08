using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Pescaria_CG_TP1.Engine {
	public static class SceneManager {
		private static bool paused = false;
		private static string currentScene = "";
		private static TimeSpan pauseDelay = new TimeSpan();
		private static DateTime pausedTime;
		private static List<GameObject> sceneObjects = new List<GameObject>();
		private static Dictionary<string, IScene> scenes = new Dictionary<string, IScene>();

		public static Vector2 ScreenSize = Vector2.Zero;
		public static OpenGLForm Form = null;
		public static PlayerObject Player = null;

		public static bool IsPaused {
			get { return paused; }
		}

		public static void Pause () {
			paused = !paused;
			if (!paused)
				// Se não está pausado é porque acabou de sair do pause, logo contabiliza o atraso do pause
				pauseDelay += SceneManager.Now.Subtract(pausedTime);
			else
				// Armazena o instante de início do pause
				pausedTime = DateTime.Now.Subtract(pauseDelay);
		}

		public static DateTime Now {
			get {
				if (paused)
					// Retorna o momento do pause, congelando o jogo
					return pausedTime;
				else
					// Retorna o momento atual com a correção do tempo que o software ficou pausado
					return DateTime.Now.Subtract(pauseDelay);
			}
		}

		public static void RegisterNewScene (string sceneKey, IScene scene) {
			scenes.Add(sceneKey, scene);
		}

		public static void LoadScene (string sceneKey) {
			paused = false;
			pauseDelay = new TimeSpan();
			sceneObjects.Clear();
			currentScene = sceneKey;
			scenes[currentScene].InitScene();
		}

		public static void AddObject (GameObject obj) {
			sceneObjects.Add(obj);
		}

		public static void RemoveObject (GameObject obj) {
			sceneObjects.Remove(obj);
		}

		public static void OpenGLDraw (int glWidth, int glHeight) {
			ScreenSize.X = glWidth;
			ScreenSize.Y = glHeight;

			if (scenes.ContainsKey(currentScene))
				scenes[currentScene].OpenGLDraw(glWidth, glHeight);

			for (int o = 0; o < sceneObjects.Count; o++) {
				if (sceneObjects[o].HasCollisionListeners()) {
					for (int b = 0; b < sceneObjects.Count; b++) {
						if (Vector2.Overlap(sceneObjects[o].Transform, sceneObjects[b].Transform))
							sceneObjects[o].CollisionDetected(sceneObjects[b]);
					}
				}
				sceneObjects[o].OpenGLDraw(glWidth, glHeight);
			}
		}

		public static void MouseClick () {
			Vector2 mousePos = new Vector2(Form.openGLControl1.PointToClient(Cursor.Position));
			List<GameObject> objs = new List<GameObject>();

			foreach (GameObject obj in sceneObjects) {
				if (!obj.HasClickListeners() || (paused && !obj.InteractiveDuringPause)) continue;

				// Em caso de ter clicado sobre o objeto, separa ele para chamar o callback
				// Isso é necessário, pois o callback pode destruir o objeto alterando assim a lista sceneObjects, quebrando o loop.
				if (Vector2.Overlap(obj.Transform, mousePos))
					objs.Add(obj);
			}

			// Percorre os objetos clicados e chama seus callbacks
			foreach (GameObject obj in objs)
				obj.Clicked();
		}
	}
}

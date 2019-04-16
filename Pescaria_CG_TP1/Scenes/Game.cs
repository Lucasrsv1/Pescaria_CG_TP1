using System;
using System.Drawing;
using System.Threading;
using Pescaria_CG_TP1.Engine;
using Pescaria_CG_TP1.Prefabs;
using SharpGL;
using SharpGL.Enumerations;

namespace Pescaria_CG_TP1.Scenes {
	public class Game : IScene {
		private static bool aimTextureRegistered = false;
		private static uint AIM_TEXTURE_ID;

		public static bool GameEnded;
		public static float CameraYPosition;

		public Game (OpenGL gl) {
			this.gl = gl;
			this.random = new Random();

			if (!aimTextureRegistered) {
				AIM_TEXTURE_ID = Animator.RegisterTexture(new Bitmap("./Textures/AIM.png"));
				aimTextureRegistered = true;
			}
		}

		private OpenGL gl;
		private Random random;
		private GameHUD gameHUD;

		public void InitScene () {
			GameEnded = false;
			gameHUD = new GameHUD(gl);
			gameHUD.Init();
			SceneManager.HUD = gameHUD;

			// Create scene's background
			CreateBackground();

			// Create player
			PlayerObject player = new PlayerObject(new Vector2(34, 71), "Player", new Vector2(SceneManager.ScreenSize.X / 2f, 5));
			player.Animator.AddTexture("HOOK", new Bitmap("./Textures/HOOK.png"));
			player.Animator.CurrentTexture = "HOOK";
			SceneManager.AddObject(player);
			SceneManager.Player = player;

			// Create fishes
			Fish.RegisterTextures();
			Fish3.RegisterTextures();
			new Thread(new ThreadStart(CreateFishes)).Start();

			// Create bombs
			Bombs.RegisterTextures();
			new Thread(new ThreadStart(CreateBombs)).Start();

			// Create bubbles
			float bubblePos = 500;
			while (bubblePos < 17000) {
				CreateBubbles(new Vector2((float)this.random.NextDouble() * SceneManager.ScreenSize.X, bubblePos, SceneManager.ScreenSize.X, 0));
				bubblePos += (float) this.random.NextDouble() * 500;
			}

			// Create player's aim
			GameObject aim = new GameObject(new Vector2(40, 40), "Aim");
			aim.Animator.AddTexture("AIM", AIM_TEXTURE_ID);
			aim.Animator.CurrentTexture = "AIM";
			aim.Transform.SetPositionFn(() => {
				aim.Transform.Position = SceneManager.MousePositionInScene() - aim.Transform.Size / 2f;
			});
			SceneManager.Aim = aim;
		}

		public void CreateBackground () {
			GameObject background = new GameObject(new Vector2(SceneManager.ScreenSize.X, 400, SceneManager.ScreenSize.X, 0), "", new Vector2(0, -225));
			background.Animator.AddTexture("BACKGROUND", new Bitmap("./Textures/BACKGROUND.png"), 3, 2000);
			background.Animator.CurrentTexture = "BACKGROUND";
			SceneManager.AddObject(background);

			background = new GameObject(new Vector2(SceneManager.ScreenSize.X, 270, SceneManager.ScreenSize.X, 0), "Background Bottom", new Vector2(0, 9330));
			background.Animator.AddTexture("BACKGROUND_BOTTOM", new Bitmap("./Textures/BACKGROUND_BOTTOM.png"));
			background.Animator.CurrentTexture = "BACKGROUND_BOTTOM";
			SceneManager.AddObject(background);

			GameObject sun = new GameObject(new Vector2(230, 300), "Sun", new Vector2(10, -450));
			sun.Animator.AddTexture("SUN", new Bitmap("./Textures/SUN.png"));
			sun.Animator.CurrentTexture = "SUN";
			sun.Transform.SetPositionFn(() => {
				if (CameraYPosition > 450)
					sun.Transform.Position.Y = -CameraYPosition - 25;
			});
			SceneManager.AddObject(sun);
		}

		public void CreateBubbles (Vector2 centerPos) {
			float size;
			int qty = 1 + this.random.Next(4);
			for (int i = 0; i < qty; i++) {
				size = 10 + (float) this.random.NextDouble() * 16;
				Vector2 pos = centerPos + (Vector2.One * ((this.random.NextDouble() + 6) * size * (this.random.NextDouble() > 0.5 ? -1 : 1)));

				GameObject bubble = new GameObject(new Vector2(size, size), "Bubble", pos);
				bubble.Animator.AddTexture("BUBBLE", gameHUD.BUBBLE_TEXTURE_ID);
				bubble.AddOnClickListener(() => {
					SceneManager.Player.BubblesScore++;
					bubble.Destroy();
				});

				AnimationClip animationClip = new AnimationClip(AnimationClip.ClipTypes.ONCE);
				animationClip.AddClipPoint((int) Math.Round(750 * (pos.Y / 100)), "BUBBLE", new Vector2(Transform.DISABLE_AXIS_MOVIMENT, 0));
				animationClip.AddClipPoint(1000, "", null, bubble.Destroy);
				bubble.Animator.AddAnimationClip("CLIP", animationClip);

				bubble.Animator.PlayAnimationClip("CLIP");
				SceneManager.AddObject(bubble);
			}
		}

		public void CreateFishes () {
			float fishPos = 200 + (float) this.random.NextDouble() * 150;
			while (fishPos < 9400 && (SceneManager.Form == null || !SceneManager.Form.IsDisposed)) {
				if (random.NextDouble() <= 0.33)
					Fish3.Instantiate(SceneManager.Player, new Vector2(0, fishPos));
				else
					Fish.Instantiate(SceneManager.Player, new Vector2(0, fishPos));		// Fishes 1 and 2

				fishPos += (float) this.random.NextDouble() * 175;
				Thread.Sleep((int) Math.Round(random.NextDouble() * 450));
			}
		}

		public void CreateBombs () {
			float bombPos = 200 + (float) this.random.NextDouble() * 500;
			while (bombPos < 9400 && (SceneManager.Form == null || !SceneManager.Form.IsDisposed)) {
				Bombs.Instantiate(new Vector2(0, bombPos));
				bombPos += (float) this.random.NextDouble() * 850;
				Thread.Sleep((int) Math.Round(random.NextDouble() * 650));
			}
		}

		public static void CreateClouds () {
			Random random = new Random();
			uint[] cloudsIds = new uint[3];
			for (int i = 1; i <= 3; i++)
				cloudsIds[i - 1] = Animator.RegisterTexture(new Bitmap("./Textures/CLOUD_" + i + ".png"));

			// Create clouds
			float cloudPos = -500;
			while (cloudPos > -7300) {
				GameObject cloud = new GameObject(new Vector2(250, 100), "Cloud", new Vector2(-200 + (float) random.NextDouble() * (SceneManager.ScreenSize.X + 200), cloudPos, SceneManager.ScreenSize.X, 0));
				cloud.Animator.AddTexture("CLOUD", cloudsIds[random.Next(3)]);
				cloud.Animator.CurrentTexture = "CLOUD";
				SceneManager.AddObject(cloud);
				cloudPos -= (float) random.NextDouble() * 300;
			}
		}

		public void OpenGLDraw (int glWidth, int glHeight) {
			// Anda com a cena para cima à medida que o anzol afunda (rola a câmera)
			Game.CameraYPosition = Math.Max(250 - SceneManager.Player.Transform.Position.Y, -9600 + SceneManager.ScreenSize.Y);
			gl.Translate(0, Game.CameraYPosition, 0);
			
			// Background color
			if (SceneManager.Player.Transform.Position.Y < 250 - SceneManager.ScreenSize.Y)
				gl.ClearColor(64 / 255f, 154 / 255f, 246 / 255f, 0f);		// Sky
			else
				gl.ClearColor(6 / 255f, 132 / 255f, 152 / 255f, 0f);		// Ocean

			// Sky top
			gl.Begin(BeginMode.TriangleFan);
				gl.Color(64 / 255f, 154 / 255f, 246 / 255f);
				gl.Vertex(0, -SceneManager.ScreenSize.Y);
				gl.Vertex(SceneManager.ScreenSize.X, -SceneManager.ScreenSize.Y);
				gl.Vertex(SceneManager.ScreenSize.X, 0);
				gl.Vertex(0, 0);
			gl.End();
			gl.Color(1f, 1f, 1f);
		}
	}
}

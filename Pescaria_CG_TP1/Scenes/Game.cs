using System;
using System.Drawing;
using System.Threading;
using Pescaria_CG_TP1.Engine;
using Pescaria_CG_TP1.Prefabs;
using SharpGL;
using SharpGL.Enumerations;

namespace Pescaria_CG_TP1.Scenes {
	public class Game : IScene {
		public static float CameraYPosition;

		public Game (OpenGL gl) {
			this.gl = gl;
			this.random = new Random();
		}

		private OpenGL gl;
		private Random random;
		private GameHUD gameHUD;

		public void InitScene () {
			gameHUD = new GameHUD(gl);
			gameHUD.Init();
			SceneManager.HUD = gameHUD;

			GameObject background = new GameObject(new Vector2(SceneManager.ScreenSize.X, 400, SceneManager.ScreenSize.X, 0), "", new Vector2(0, -225));
			background.Animator.AddTexture("BACKGROUND", new Bitmap("./Textures/BACKGROUND.png"), 3, 2000);
			background.Animator.CurrentTexture = "BACKGROUND";
			SceneManager.AddObject(background);

			background = new GameObject(new Vector2(SceneManager.ScreenSize.X, 270, SceneManager.ScreenSize.X, 0), "Background Bottom", new Vector2(0, 9330));
			background.Animator.AddTexture("BACKGROUND_BOTTOM", new Bitmap("./Textures/BACKGROUND_BOTTOM.png"));
			background.Animator.CurrentTexture = "BACKGROUND_BOTTOM";
			SceneManager.AddObject(background);

			PlayerObject player = new PlayerObject(new Vector2(34, 71), "Player", new Vector2((SceneManager.ScreenSize.X - 34) / 2f, 5));
			player.Animator.AddTexture("HOOK", new Bitmap("./Textures/HOOK.png"));
			player.Animator.CurrentTexture = "HOOK";

			SceneManager.AddObject(player);
			SceneManager.Player = player;

			Fish.RegisterTextures();
			new Thread(new ThreadStart(CreateFishes)).Start();

			GameObject fish2 = new GameObject(new Vector2(80, 80), "Fish", new Vector2(375, 375), 0, new Vector2(-1, 1));
			fish2.Animator.AddTexture("GREEN_FISH_REST", new Bitmap("./Textures/GREEN_FISH_REST.png"), 20, 1000, Texture.Orientations.BOTH, 4);
			fish2.Animator.CurrentTexture = "GREEN_FISH_REST";
			SceneManager.AddObject(fish2);

			float bubblePos = 500;
			while (bubblePos < 17000) {
				CreateBubbles(new Vector2((float)this.random.NextDouble() * SceneManager.ScreenSize.X, bubblePos, SceneManager.ScreenSize.X, 0));
				bubblePos += (float) this.random.NextDouble() * 500;
			}
		}

		public void CreateBubbles (Vector2 centerPos) {
			float size;
			int qty = 1 + this.random.Next(4);
			for (int i = 0; i < qty; i++) {
				size = 8 + (float) this.random.NextDouble() * 12;
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
			Random random = new Random();
			float fishPos = 200 + (float) this.random.NextDouble() * 150;
			while (fishPos < 9400 && (SceneManager.Form == null || !SceneManager.Form.IsDisposed)) {
				Fish.Instantiate(SceneManager.Player, new Vector2(0, fishPos));
				fishPos += (float) this.random.NextDouble() * 200;
				Thread.Sleep((int) Math.Round(random.NextDouble() * 750));
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

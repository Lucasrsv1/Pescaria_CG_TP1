using System;
using System.Drawing;
using Pescaria_CG_TP1.Engine;
using SharpGL;

namespace Pescaria_CG_TP1.Scenes {
	public class Game : IScene {
		public Game (OpenGL gl) {
			this.gl = gl;
			this.random = new Random();
		}

		private OpenGL gl;
		private Random random;

		public void InitScene () {
			// Cor de fundo
			gl.ClearColor(6 / 255f, 132 / 255f, 152 / 255f, 0);

			GameObject background = new GameObject(gl, new Vector2(SceneManager.ScreenSize.X, 400, SceneManager.ScreenSize.X, 0), new Vector2(0, -225));
			background.Animator.AddTexture("BACKGROUND", new Bitmap("./Textures/BACKGROUND.png"), 3, 2000);
			background.Animator.CurrentTexture = "BACKGROUND";
			SceneManager.AddObject(background);

			background = new GameObject(gl, new Vector2(SceneManager.ScreenSize.X, 270, SceneManager.ScreenSize.X, 0), new Vector2(0, 9330));
			background.Animator.AddTexture("BACKGROUND_BOTTOM", new Bitmap("./Textures/BACKGROUND_BOTTOM.png"));
			background.Animator.CurrentTexture = "BACKGROUND_BOTTOM";
			SceneManager.AddObject(background);

			PlayerObject player = new PlayerObject(gl, new Vector2(34, 71), new Vector2((SceneManager.ScreenSize.X - 34) / 2f, 5), "Player");
			player.Animator.AddTexture("HOOK", new Bitmap("./Textures/HOOK.png"));
			player.Animator.CurrentTexture = "HOOK";

			SceneManager.AddObject(player);
			SceneManager.Player = player;

			for (int i = 0; i < 5; i++) {
				GameObject fish = new GameObject(gl, new Vector2(100, 100), new Vector2(-100, 150 * i), "Fish");
				fish.AddOnCollisionListener((collider) => {
					if (collider.Tag == "Player") {
						fish.Animator.StopAnimationClip();
						fish.Animator.CurrentTexture = "FISH1_HOOKED";

						float space = -10 + (float) this.random.NextDouble() * 20;
						fish.Transform.SetPositionFn(() => {
							fish.Transform.Position.X = player.Transform.Position.X - (player.Transform.Size.X * 1.5f) + space;
							fish.Transform.Position.Y = player.Transform.Position.Y + (player.Transform.Size.Y * 0.4f) + space;
						});
					}
				});

				fish.Animator.AddTexture("FISH1_HOOKED", new Bitmap("./Textures/FISH1_HOOKED.png"), 6, 600, Texture.Orientations.VERTICAL);
				fish.Animator.AddTexture("FISH1_SWIM_RIGHT", new Bitmap("./Textures/FISH1_SWIM_RIGHT.png"), 6, 600);
				fish.Animator.AddTexture("FISH1_REST_RIGHT", new Bitmap("./Textures/FISH1_REST_RIGHT.png"), 6, 600);
				fish.Animator.AddTexture("FISH1_SWIM_LEFT", new Bitmap("./Textures/FISH1_SWIM_LEFT.png"), 6, 600);
				fish.Animator.AddTexture("FISH1_REST_LEFT", new Bitmap("./Textures/FISH1_REST_LEFT.png"), 6, 600);

				AnimationClip animationClip = new AnimationClip(AnimationClip.ClipTypes.LOOP);
				animationClip.AddClipPoint(2000, "FISH1_SWIM_RIGHT", new Vector2(500, 0, 400, 0));
				animationClip.AddClipPoint(2000, "FISH1_REST_RIGHT");
				animationClip.AddClipPoint(2000, "FISH1_SWIM_LEFT", new Vector2(-500, 0, 400, 0));
				animationClip.AddClipPoint(2000, "FISH1_REST_LEFT");
				fish.Animator.AddAnimationClip("CLIP", animationClip);

				fish.Animator.PlayAnimationClip("CLIP");
				SceneManager.AddObject(fish);
			}

			float bubblePos = 500;
			while (bubblePos < 18000) {
				CreateBubbles(new Vector2((float)this.random.NextDouble() * SceneManager.ScreenSize.X, bubblePos));
				bubblePos += (float) this.random.NextDouble() * 500;
			}
		}

		public void CreateBubbles (Vector2 centerPos) {
			float size;
			int qty = 1 + this.random.Next(4);
			for (int i = 0; i < qty; i++) {
				size = 8 + (float) this.random.NextDouble() * 10;
				Vector2 pos = centerPos + (Vector2.Identity * ((this.random.NextDouble() + 6) * size * (this.random.NextDouble() > 0.5 ? -1 : 1)));

				GameObject bubble = new GameObject(gl, new Vector2(size, size), pos);
				bubble.Animator.AddTexture("BUBBLE", new Bitmap("./Textures/BUBBLE.png"));

				AnimationClip animationClip = new AnimationClip(AnimationClip.ClipTypes.ONCE);
				animationClip.AddClipPoint((int) Math.Round(750 * (pos.Y / 100)), "BUBBLE", new Vector2(0, -pos.Y));
				animationClip.AddClipPoint(1000, "", null, bubble.Destroy);
				bubble.Animator.AddAnimationClip("CLIP", animationClip);

				bubble.Animator.PlayAnimationClip("CLIP");
				SceneManager.AddObject(bubble);
			}
		}

		public void OpenGLDraw (int glWidth, int glHeight) {
			// Anda com a cena para cima à medida que o anzol afunda (rola a câmera)
			if (SceneManager.Player != null && SceneManager.Player.Transform.Position.Y > 250)
				gl.Translate(0, Math.Max(250 - SceneManager.Player.Transform.Position.Y, -9600 + SceneManager.ScreenSize.Y), 0);
		}
	}
}

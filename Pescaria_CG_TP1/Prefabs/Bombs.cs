using Pescaria_CG_TP1.Engine;
using System;
using System.Drawing;
using System.Media;

namespace Pescaria_CG_TP1.Prefabs {
	public class Bombs {
		private static bool texturesRegistered = false;
		private static readonly SoundPlayer explosionSound = new SoundPlayer ("./Audio/Explosion.wav");
		private static readonly Random random = new Random();

		public static readonly Bitmap[] BOMB_TEXTURES = new Bitmap[4] {
			new Bitmap("./Textures/FLOATING_MINE.png"),
			new Bitmap("./Textures/TNT.png"),
			new Bitmap("./Textures/BOMB.png"),
			new Bitmap("./Textures/TORPEDO.png")
		};

		public static Bitmap[] EXPLOSION_TEXTURES = new Bitmap[9];

		public static uint[] BOMB_TEXTURES_IDS = new uint[BOMB_TEXTURES.Length];
		public static uint[] EXPLOSION_TEXTURES_IDS = new uint[EXPLOSION_TEXTURES.Length];

		public static void RegisterTextures () {
			if (!texturesRegistered)
				texturesRegistered = true;
			else
				return;

			for (int i = 0; i < BOMB_TEXTURES_IDS.Length; i++)
				BOMB_TEXTURES_IDS[i] = Animator.RegisterTexture(BOMB_TEXTURES[i]);

			for (int i = 0; i < EXPLOSION_TEXTURES.Length; i++) {
				EXPLOSION_TEXTURES[i] = new Bitmap("./Textures/EXPLOSION_" + (i + 1) + ".png");
				EXPLOSION_TEXTURES_IDS[i] = Animator.RegisterTexture(EXPLOSION_TEXTURES[i]);
			}
		}

		public static void Instantiate (Vector2 position) {
			GameObject bomb;
			int size = 60 + random.Next(40);
			double bombType = random.NextDouble();
			float fraction = 1f / BOMB_TEXTURES.Length;
			if (bombType < 1 - fraction) {
				bomb = new GameObject(new Vector2(size, size), "Bomb", position);
				bomb.Animator.AddTexture("BOMB", BOMB_TEXTURES_IDS[(int) Math.Floor(bombType / fraction)]);
				bomb.Animator.CurrentTexture = "BOMB";
				bomb.Transform.Spin(3);

				// Select a random animation clip
				bomb.Animator.AddAnimationClip("CLIP", GetAnimationClip(random.Next(4), bomb));
				bomb.Animator.PlayAnimationClip("CLIP");

				SceneManager.AddObject(bomb);
			} else {
				bomb = new GameObject(new Vector2(size, size * 0.65f), "Bomb", position);
				bomb.Animator.AddTexture("BOMB", BOMB_TEXTURES_IDS[BOMB_TEXTURES.Length - 1]);
				bomb.Animator.CurrentTexture = "BOMB";

				// Select a random animation clip
				bomb.Animator.AddAnimationClip("CLIP", GetTorpedoAnimationClip(random.Next(3), bomb));
				bomb.Animator.PlayAnimationClip("CLIP");

				SceneManager.AddObject(bomb);
			}

			bomb.AddOnCollisionListener("EXPLODE", (GameObject collider) => {
				if (collider.Tag == "Player") {
					// Take one player's life and destroy all fishes he hooked
					SceneManager.Player.Lives--;
					for (int i = 0; i < SceneManager.SceneObjects.Count; i++) {
						if (SceneManager.SceneObjects[i].Tag == "Hooked_Fish")
							SceneManager.SceneObjects[i--].Destroy();
					}

					Explode(bomb);
					bomb.Destroy();
				}
			});

			bomb.AddOnClickListener(() => {
				Explode(bomb);
				bomb.Destroy();
			});
		}

		private static void Explode (GameObject bomb) {
			GameObject explosion = new GameObject(Vector2.One * bomb.Transform.Size.X * 2.5, "Explosion", bomb.Transform.Position - bomb.Transform.Size);
			explosion.Animator.AddTexture("EXPLOSION", EXPLOSION_TEXTURES_IDS[random.Next(EXPLOSION_TEXTURES_IDS.Length)], 14, 600, Texture.Orientations.BOTH, 4);
			explosion.Animator.CurrentTexture = "EXPLOSION";

			AnimationClip animationClip = new AnimationClip(AnimationClip.ClipTypes.ONCE);
			animationClip.AddClipPoint(600);
			animationClip.AddClipPoint(1000, "", null, explosion.Destroy);
			explosion.Animator.AddAnimationClip("AUTO_DESTROY", animationClip);
			explosion.Animator.PlayAnimationClip("AUTO_DESTROY");
			SceneManager.AddObject(explosion);
			if (!SceneManager.IsMute)
				explosionSound.Play();
		}

		private static AnimationClip GetAnimationClip (int animationIdx, GameObject bomb) {
			AnimationClip animationClip = new AnimationClip(AnimationClip.ClipTypes.LOOP);
			switch (animationIdx) {
				case 0:
					animationClip.AddClipPoint(6000, "BOMB", new Vector2(400 - bomb.Transform.Size.X, Transform.DISABLE_AXIS_MOVIMENT, 400, 0, bomb.Transform.Size.X, 0));
					animationClip.AddClipPoint(6000, "BOMB", new Vector2(0, Transform.DISABLE_AXIS_MOVIMENT));
					break;
				case 1:
					animationClip.AddClipPoint(6000, "BOMB", new Vector2(0, bomb.Transform.Position.Y + 200, 400, 0, bomb.Transform.Size.X, 0), () => {
						bomb.Transform.Position.X = SceneManager.ScreenSize.X - bomb.Transform.Size.X;
					});
					animationClip.AddClipPoint(3000, "BOMB", new Vector2(150, bomb.Transform.Position.Y, 400, 0, bomb.Transform.Size.X, 0));
					animationClip.AddClipPoint(4000, "BOMB", new Vector2(400 - bomb.Transform.Size.X, Transform.DISABLE_AXIS_MOVIMENT, 400, 0, bomb.Transform.Size.X, 0));
					break;
				case 2:
					animationClip.AddClipPoint(4000, "BOMB", new Vector2(200 - bomb.Transform.Size.X / 2f, bomb.Transform.Position.Y + 200, 400, 0, bomb.Transform.Size.X, 0));
					animationClip.AddClipPoint(3000);
					animationClip.AddClipPoint(4000, "BOMB", new Vector2(400 - bomb.Transform.Size.X, bomb.Transform.Position.Y, 400, 0, bomb.Transform.Size.X, 0));
					animationClip.AddClipPoint(4000, "BOMB", new Vector2(200 - bomb.Transform.Size.X / 2f, bomb.Transform.Position.Y - 200, 400, 0, bomb.Transform.Size.X, 0));
					animationClip.AddClipPoint(3000);
					animationClip.AddClipPoint(4000, "BOMB", new Vector2(0, bomb.Transform.Position.Y, 400, 0, bomb.Transform.Size.X, 0));
					break;
				default:
					animationClip.AddClipPoint(6000, "BOMB", new Vector2(400 - bomb.Transform.Size.X, bomb.Transform.Position.Y + 250, 400, 0, bomb.Transform.Size.X, 0));
					animationClip.AddClipPoint(4000, "BOMB", new Vector2(0, bomb.Transform.Position.Y + 250, 400, 0, bomb.Transform.Size.X, 0));
					animationClip.AddClipPoint(6000, "BOMB", new Vector2(400 - bomb.Transform.Size.X, bomb.Transform.Position.Y, 400, 0, bomb.Transform.Size.X, 0));
					animationClip.AddClipPoint(4000, "BOMB", new Vector2(0, bomb.Transform.Position.Y, 400, 0, bomb.Transform.Size.X, 0));
					break;
			}
			return animationClip;
		}

		private static AnimationClip GetTorpedoAnimationClip (int animationIdx, GameObject bomb) {
			AnimationClip animationClip = new AnimationClip(AnimationClip.ClipTypes.LOOP);
			switch (animationIdx) {
				case 0:
					animationClip.AddClipPoint(4000, "BOMB", new Vector2(405, Transform.DISABLE_AXIS_MOVIMENT, 400, 0), () => {
						bomb.Transform.Scale.X = 1;
					});
					animationClip.AddClipPoint(2000);
					animationClip.AddClipPoint(4000, "BOMB", new Vector2(bomb.Transform.Size.X * -2, Transform.DISABLE_AXIS_MOVIMENT), () => {
						bomb.Transform.Scale.X = -1;
					});
					animationClip.AddClipPoint(2000);
					break;
				case 1:
					animationClip.AddClipPoint(4000, "BOMB", new Vector2(405, bomb.Transform.Position.Y + 250, 400, 0), () => {
						bomb.Transform.Scale.X = 1;
						bomb.Transform.Rotation = -30;
					});
					animationClip.AddClipPoint(2000);
					animationClip.AddClipPoint(4000, "BOMB", new Vector2(bomb.Transform.Size.X * -2, bomb.Transform.Position.Y), () => {
						bomb.Transform.Scale.X = -1;
						bomb.Transform.Rotation = -30;
					});
					animationClip.AddClipPoint(2000);
					break;
				default:
					if (bomb.Transform.Position.Y < 250) return GetTorpedoAnimationClip(1, bomb);
					animationClip.AddClipPoint(4000, "BOMB", new Vector2(405, bomb.Transform.Position.Y - 250, 400, 0), () => {
						bomb.Transform.Scale.X = 1;
						bomb.Transform.Rotation = 30;
					});
					animationClip.AddClipPoint(2000);
					animationClip.AddClipPoint(4000, "BOMB", new Vector2(bomb.Transform.Size.X * -2, bomb.Transform.Position.Y), () => {
						bomb.Transform.Scale.X = -1;
						bomb.Transform.Rotation = 30;
					});
					animationClip.AddClipPoint(2000);
					break;
			}
			return animationClip;
		}
	}
}

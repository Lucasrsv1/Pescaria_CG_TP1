///-----------------------------------------------------------------
///   Namespace:		Pescaria_CG_TP1.Prefabs
///   Class:			Fish3
///   Description:		Creates a third type of fish and handle its movement and behaviour.
///   Subject:			Computer Graphics
///   Author:			Lucas Rassilan Vilanova
///-----------------------------------------------------------------

using Pescaria_CG_TP1.Engine;
using Pescaria_CG_TP1.Scenes;
using System;
using System.Drawing;

namespace Pescaria_CG_TP1.Prefabs {
	public class Fish3 {
		private static Random random = new Random();
		private static bool texturesRegistered = false;

		public static Bitmap[] FISH3_TEXTURES = new Bitmap[4] {
			new Bitmap("./Textures/FISH3_SWIM_RIGHT.png"),
			new Bitmap("./Textures/FISH3_REST_RIGHT.png"),
			new Bitmap("./Textures/FISH3_SWIM_LEFT.png"),
			new Bitmap("./Textures/FISH3_REST_LEFT.png")
		};

		public static uint[] FISH3_TEXTURES_IDS = new uint[FISH3_TEXTURES.Length];

		public static void RegisterTextures () {
			if (!texturesRegistered)
				texturesRegistered = true;
			else
				return;

			for (int i = 0; i < FISH3_TEXTURES_IDS.Length; i++)
				FISH3_TEXTURES_IDS[i] = Animator.RegisterTexture(FISH3_TEXTURES[i]);
		}

		public static void Instantiate (PlayerObject player, Vector2 position) {
			int size = 50 + random.Next(50);
			GameObject fish = new GameObject(new Vector2(size, size), "Fish", position);

			// Handle collision with the hook (player)
			fish.AddOnCollisionListener("HOOKED", (collider) => {
				if (collider.Tag == "Player" && !Game.GameEnded) {
					fish.Tag = "Hooked_Fish";
					fish.RemoveCollisionListener("HOOKED");
					fish.Animator.StopAnimationClip();
					fish.Animator.CurrentTexture = "FISH3_SWIM_RIGHT";
					fish.Transform.Rotation = 90;

					float spaceX = (float) random.NextDouble() * 25;
					float spaceY = (float) random.NextDouble() * -50;
					fish.Transform.SetPositionFn(() => {
						fish.Transform.Position.X = player.Transform.Position.X - (player.Transform.Size.X * 1.5f) + spaceX;
						fish.Transform.Position.Y = player.Transform.Position.Y + player.Transform.Size.Y + spaceY - 20;
					});
				}
			});

			// Register textures (sprites)
			fish.Animator.AddTexture("FISH3_SWIM_RIGHT", FISH3_TEXTURES_IDS[0], 6, 600);
			fish.Animator.AddTexture("FISH3_REST_RIGHT", FISH3_TEXTURES_IDS[1], 6, 600);
			fish.Animator.AddTexture("FISH3_SWIM_LEFT", FISH3_TEXTURES_IDS[2], 6, 600);
			fish.Animator.AddTexture("FISH3_REST_LEFT", FISH3_TEXTURES_IDS[3], 6, 600);

			// Select a random animation clip
			fish.Animator.AddAnimationClip("CLIP", GetAnimationClip(random.Next(6), fish));
			fish.Animator.PlayAnimationClip("CLIP");

			SceneManager.AddObject(fish);
		}

		private static AnimationClip GetAnimationClip (int animationIdx, GameObject fish) {
			AnimationClip animationClip = new AnimationClip(AnimationClip.ClipTypes.LOOP);
			switch (animationIdx) {
				case 0:
					animationClip.AddClipPoint(2000, "FISH3_SWIM_RIGHT", new Vector2(400 - fish.Transform.Size.X, Transform.DISABLE_AXIS_MOVIMENT, 400, 0, fish.Transform.Size.X, 0));
					animationClip.AddClipPoint(2000, "FISH3_REST_RIGHT");
					animationClip.AddClipPoint(2000, "FISH3_SWIM_LEFT", new Vector2(0, Transform.DISABLE_AXIS_MOVIMENT));
					animationClip.AddClipPoint(2000, "FISH3_REST_LEFT");
					break;
				case 1:
					animationClip.AddClipPoint(100, "FISH3_SWIM_RIGHT", new Vector2(0, fish.Transform.Position.Y));
					animationClip.AddClipPoint(4000, "FISH3_SWIM_RIGHT", new Vector2(400 - fish.Transform.Size.X, fish.Transform.Position.Y + 400, 400, 0, fish.Transform.Size.X, 0), () => {
						fish.Transform.Rotation = -20;
					});
					animationClip.AddClipPoint(4000, "FISH3_SWIM_LEFT", new Vector2(0, fish.Transform.Position.Y));
					break;
				case 2:
					// Animation allowed only for fishes under 400px of water
					if (fish.Transform.Position.Y < 400) return GetAnimationClip(3, fish);

					animationClip.AddClipPoint(100, "FISH3_SWIM_RIGHT", new Vector2(0, fish.Transform.Position.Y));
					animationClip.AddClipPoint(3000, "FISH3_SWIM_RIGHT", new Vector2(400 - fish.Transform.Size.X, fish.Transform.Position.Y - 400, 400, 0, fish.Transform.Size.X, 0), () => {
						fish.Transform.Rotation = 22;
					});
					animationClip.AddClipPoint(3000, "FISH3_SWIM_LEFT", new Vector2(0, fish.Transform.Position.Y));
					break;
				case 3:
					animationClip.AddClipPoint(2000, "FISH3_SWIM_LEFT", new Vector2(400 - fish.Transform.Size.X, fish.Transform.Position.Y + 300, 400, 400, fish.Transform.Size.X, fish.Transform.Size.Y), () => {
						fish.Transform.Position.X = SceneManager.ScreenSize.X - fish.Transform.Size.X;
						fish.Transform.Rotation = 90;
					});

					animationClip.AddClipPoint(2000, "FISH3_SWIM_LEFT", new Vector2(0, fish.Transform.Position.Y + 300, 400, 400, fish.Transform.Size.X, fish.Transform.Size.Y), () => {
						fish.Transform.Rotation = 0;
					});

					animationClip.AddClipPoint(2000, "FISH3_SWIM_RIGHT", new Vector2(0, fish.Transform.Position.Y + 75, 400, 400, fish.Transform.Size.X, fish.Transform.Size.Y), () => {
						fish.Transform.Rotation = 90;
					});

					animationClip.AddClipPoint(2000, "FISH3_SWIM_RIGHT", new Vector2(400 - fish.Transform.Size.X, fish.Transform.Position.Y + 75, 400, 400, fish.Transform.Size.X, fish.Transform.Size.Y), () => {
						fish.Transform.Rotation = 0;
					});
					break;
				case 4:
					animationClip.AddClipPoint(2000, "FISH3_SWIM_LEFT", new Vector2(0, fish.Transform.Position.Y, 400, 0, fish.Transform.Size.X, 0));
					animationClip.AddClipPoint(2000, "FISH3_SWIM_RIGHT", new Vector2(100, fish.Transform.Position.Y + 100, 400, 0, fish.Transform.Size.X, 0), () => {
						fish.Transform.Rotation = -30;
					});
					animationClip.AddClipPoint(2000, "FISH3_SWIM_RIGHT", new Vector2(200, fish.Transform.Position.Y, 400, 0, fish.Transform.Size.X, 0), () => {
						fish.Transform.Rotation = 30;
					});
					animationClip.AddClipPoint(2000, "FISH3_SWIM_RIGHT", new Vector2(400 - fish.Transform.Size.X, fish.Transform.Position.Y + 100, 400, 0, fish.Transform.Size.X, 0), () => {
						fish.Transform.Rotation = -30;
					});
					animationClip.AddClipPoint(2000);
					animationClip.AddClipPoint(2000, "FISH3_SWIM_LEFT", new Vector2(200, fish.Transform.Position.Y, 400, 0, fish.Transform.Size.X, 0), () => {
						fish.Transform.Rotation = -30;
					});
					animationClip.AddClipPoint(2000, "FISH3_SWIM_LEFT", new Vector2(100, fish.Transform.Position.Y + 100, 400, 0, fish.Transform.Size.X, 0), () => {
						fish.Transform.Rotation = 30;
					});
					animationClip.AddClipPoint(2000, "FISH3_SWIM_LEFT", new Vector2(0, fish.Transform.Position.Y, 400, 0, fish.Transform.Size.X, 0), () => {
						fish.Transform.Rotation = -30;
					});
					animationClip.AddClipPoint(2000, "FISH3_REST_LEFT");
					break;
				default:
					animationClip.AddClipPoint(100, "FISH3_SWIM_RIGHT", new Vector2(400 - fish.Transform.Size.X, fish.Transform.Position.Y, 400, 0, fish.Transform.Size.X, 0), () => {
						fish.Transform.Position.X = SceneManager.ScreenSize.X - fish.Transform.Size.X;
					});
					animationClip.AddClipPoint(2000, "FISH3_SWIM_LEFT", new Vector2(400 - fish.Transform.Size.X, fish.Transform.Position.Y, 400, 0, fish.Transform.Size.X, 0));
					animationClip.AddClipPoint(2000, "FISH3_SWIM_LEFT", new Vector2(200, fish.Transform.Position.Y + 100, 400, 0, fish.Transform.Size.X, 0), () => {
						fish.Transform.Rotation = 30;
					});
					animationClip.AddClipPoint(2000, "FISH3_SWIM_LEFT", new Vector2(100, fish.Transform.Position.Y, 400, 0, fish.Transform.Size.X, 0), () => {
						fish.Transform.Rotation = -30;
					});
					animationClip.AddClipPoint(2000, "FISH3_SWIM_LEFT", new Vector2(0, fish.Transform.Position.Y + 100, 400, 0, fish.Transform.Size.X, 0), () => {
						fish.Transform.Rotation = 30;
					});
					animationClip.AddClipPoint(2000);
					animationClip.AddClipPoint(2000, "FISH3_SWIM_RIGHT", new Vector2(100, fish.Transform.Position.Y, 400, 0, fish.Transform.Size.X, 0), () => {
						fish.Transform.Rotation = 30;
					});
					animationClip.AddClipPoint(2000, "FISH3_SWIM_RIGHT", new Vector2(200, fish.Transform.Position.Y + 100, 400, 0, fish.Transform.Size.X, 0), () => {
						fish.Transform.Rotation = -30;
					});
					animationClip.AddClipPoint(2000, "FISH3_SWIM_RIGHT", new Vector2(400 - fish.Transform.Size.X, fish.Transform.Position.Y, 400, 0, fish.Transform.Size.X, 0), () => {
						fish.Transform.Rotation = 30;
					});
					animationClip.AddClipPoint(2000, "FISH3_REST_RIGHT");
					break;
			}

			return animationClip;
		}
	}
}

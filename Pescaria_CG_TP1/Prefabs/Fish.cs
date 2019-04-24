///-----------------------------------------------------------------
///   Namespace:		Pescaria_CG_TP1.Prefabs
///   Class:			Fish
///   Description:		Creates a random fish and handle its movement and behaviour.
///   Subject:			Computer Graphics
///   Author:			Lucas Rassilan Vilanova
///-----------------------------------------------------------------

using Pescaria_CG_TP1.Engine;
using Pescaria_CG_TP1.Scenes;
using SharpGL;
using System;
using System.Drawing;

namespace Pescaria_CG_TP1.Prefabs {
	public class Fish {
		private static Random random = new Random();
		private static bool texturesRegistered = false;

		public static string[] FISH_COLORS = new string[] { "BLUE", "GREEN", "OTHER", "PINK", "RED", "YELLOW" };

		public static uint[] FISH_TEXTURES_IDS = new uint[FISH_COLORS.Length * 4];

		public static void RegisterTextures () {
			if (!texturesRegistered)
				texturesRegistered = true;
			else
				return;

			for (int i = 0; i < FISH_COLORS.Length; i++) {
				int idxPos = i * 4;
				FISH_TEXTURES_IDS[idxPos] = Animator.RegisterTexture(new Bitmap("./Textures/" + FISH_COLORS[i] + "_FISH_REST.png"));
				FISH_TEXTURES_IDS[idxPos + 1] = Animator.RegisterTexture(new Bitmap("./Textures/" + FISH_COLORS[i] + "_FISH_SWIM.png"));
				FISH_TEXTURES_IDS[idxPos + 2] = Animator.RegisterTexture(new Bitmap("./Textures/" + FISH_COLORS[i] + "_FISH2_REST.png"));
				FISH_TEXTURES_IDS[idxPos + 3] = Animator.RegisterTexture(new Bitmap("./Textures/" + FISH_COLORS[i] + "_FISH2_SWIM.png"));
			}
		}

		public static void Instantiate (PlayerObject player, Vector2 position) {
			int size = 50 + random.Next(50);
			GameObject fish = new GameObject(new Vector2(size, size), "Fish", position);

			int colorIdx = random.Next(FISH_COLORS.Length);
			bool type2 = random.NextDouble() > 0.5;

			// Handle collision with the hook (player)
			fish.AddOnCollisionListener("HOOKED", (collider) => {
				if (collider.Tag == "Player" && !Game.GameEnded) {
					fish.Tag = "Hooked_Fish";
					fish.RemoveCollisionListener("HOOKED");
					fish.Animator.StopAnimationClip();
					fish.Transform.Rotation = -90;
					fish.Transform.Scale.X = 1;

					fish.Animator.CurrentTexture = FISH_COLORS[colorIdx] + "_FISH" + (type2 ? "2" : "") + "_SWIM";

					float spaceX = (float) random.NextDouble() * 25;
					float spaceY = (float) random.NextDouble() * -50;
					fish.Transform.SetPositionFn(() => {
						fish.Transform.Position.X = player.Transform.Position.X - (player.Transform.Size.X * 1.5f) + spaceX;
						fish.Transform.Position.Y = player.Transform.Position.Y + player.Transform.Size.Y + spaceY - 20;
					});
				}
			});

			// Register textures (sprites)
			string restTexture = FISH_COLORS[colorIdx] + "_FISH" + (type2 ? "2" : "") + "_REST";
			string swimTexture = FISH_COLORS[colorIdx] + "_FISH" + (type2 ? "2" : "") + "_SWIM";
			fish.Animator.AddTexture(restTexture, FISH_TEXTURES_IDS[colorIdx * 4 + (type2 ? 2 : 0)], 20, 1000, Texture.Orientations.BOTH, 4);
			fish.Animator.AddTexture(swimTexture, FISH_TEXTURES_IDS[colorIdx * 4 + (type2 ? 3 : 1)], 12, 600, Texture.Orientations.BOTH, 4);

			// Select a random animation clip
			fish.Animator.AddAnimationClip("CLIP", GetAnimationClip(random.Next(6), fish, restTexture, swimTexture));
			fish.Animator.PlayAnimationClip("CLIP");

			SceneManager.AddObject(fish);
		}

		private static AnimationClip GetAnimationClip (int animationIdx, GameObject fish, string restTexture, string swimTexture) {
			AnimationClip animationClip = new AnimationClip(AnimationClip.ClipTypes.LOOP);
			switch (animationIdx) {
				case 0:
					animationClip.AddClipPoint(2000, swimTexture, new Vector2(400 - fish.Transform.Size.X, Transform.DISABLE_AXIS_MOVIMENT, 400, 0, fish.Transform.Size.X, 0), () => {
						fish.Transform.Scale.X = -1;
					});
					animationClip.AddClipPoint(2000, restTexture);
					animationClip.AddClipPoint(2000, swimTexture, new Vector2(0, Transform.DISABLE_AXIS_MOVIMENT), () => {
						fish.Transform.Scale.X = 1;
					});
					animationClip.AddClipPoint(2000, restTexture);
					break;
				case 1:
					animationClip.AddClipPoint(100, swimTexture, new Vector2(0, fish.Transform.Position.Y), () => {
						fish.Transform.Scale.X = -1;
					});
					animationClip.AddClipPoint(4000, swimTexture, new Vector2(400 - fish.Transform.Size.X, fish.Transform.Position.Y + 400, 400, 0, fish.Transform.Size.X, 0), () => {
						fish.Transform.Rotation = -20;
					});
					animationClip.AddClipPoint(4000, swimTexture, new Vector2(0, fish.Transform.Position.Y), () => {
						fish.Transform.Scale.X = 1;
					});
					break;
				case 2:
					// Animation allowed only for fishes under 400px of water
					if (fish.Transform.Position.Y < 400) return GetAnimationClip(3, fish, restTexture, swimTexture);

					animationClip.AddClipPoint(100, swimTexture, new Vector2(0, fish.Transform.Position.Y), () => {
						fish.Transform.Scale.X = -1;
					});
					animationClip.AddClipPoint(3000, swimTexture, new Vector2(400 - fish.Transform.Size.X, fish.Transform.Position.Y - 400, 400, 0, fish.Transform.Size.X, 0), () => {
						fish.Transform.Rotation = 22;
					});
					animationClip.AddClipPoint(3000, swimTexture, new Vector2(0, fish.Transform.Position.Y), () => {
						fish.Transform.Scale.X = 1;
					});
					break;
				case 3:
					animationClip.AddClipPoint(2000, swimTexture, new Vector2(400 - fish.Transform.Size.X, fish.Transform.Position.Y + 300, 400, 400, fish.Transform.Size.X, fish.Transform.Size.Y), () => {
						fish.Transform.Position.X = SceneManager.ScreenSize.X - fish.Transform.Size.X;
						fish.Transform.Rotation = 90;
						fish.Transform.Scale.X = 1;
					});

					animationClip.AddClipPoint(2000, swimTexture, new Vector2(0, fish.Transform.Position.Y + 300, 400, 400, fish.Transform.Size.X, fish.Transform.Size.Y), () => {
						fish.Transform.Rotation = 0;
					});

					animationClip.AddClipPoint(2000, swimTexture, new Vector2(0, fish.Transform.Position.Y + 75, 400, 400, fish.Transform.Size.X, fish.Transform.Size.Y), () => {
						fish.Transform.Scale.X = -1;
						fish.Transform.Rotation = 90;
					});

					animationClip.AddClipPoint(2000, swimTexture, new Vector2(400 - fish.Transform.Size.X, fish.Transform.Position.Y + 75, 400, 400, fish.Transform.Size.X, fish.Transform.Size.Y), () => {
						fish.Transform.Rotation = 0;
					});
					break;
				case 4:
					animationClip.AddClipPoint(2000, swimTexture, new Vector2(0, fish.Transform.Position.Y, 400, 0, fish.Transform.Size.X, 0), () => {
						fish.Transform.Scale.X = 1;
					});
					animationClip.AddClipPoint(2000, swimTexture, new Vector2(100, fish.Transform.Position.Y + 100, 400, 0, fish.Transform.Size.X, 0), () => {
						fish.Transform.Rotation = -30;
						fish.Transform.Scale.X = -1;
					});
					animationClip.AddClipPoint(2000, swimTexture, new Vector2(200, fish.Transform.Position.Y, 400, 0, fish.Transform.Size.X, 0), () => {
						fish.Transform.Rotation = 30;
					});
					animationClip.AddClipPoint(2000, swimTexture, new Vector2(400 - fish.Transform.Size.X, fish.Transform.Position.Y + 100, 400, 0, fish.Transform.Size.X, 0), () => {
						fish.Transform.Rotation = -30;
					});
					animationClip.AddClipPoint(2000);
					animationClip.AddClipPoint(2000, swimTexture, new Vector2(200, fish.Transform.Position.Y, 400, 0, fish.Transform.Size.X, 0), () => {
						fish.Transform.Rotation = -30;
						fish.Transform.Scale.X = 1;
					});
					animationClip.AddClipPoint(2000, swimTexture, new Vector2(100, fish.Transform.Position.Y + 100, 400, 0, fish.Transform.Size.X, 0), () => {
						fish.Transform.Rotation = 30;
					});
					animationClip.AddClipPoint(2000, swimTexture, new Vector2(0, fish.Transform.Position.Y, 400, 0, fish.Transform.Size.X, 0), () => {
						fish.Transform.Rotation = -30;
					});
					animationClip.AddClipPoint(2000, restTexture);
					break;
				default:
					animationClip.AddClipPoint(100, swimTexture, new Vector2(400 - fish.Transform.Size.X, fish.Transform.Position.Y, 400, 0, fish.Transform.Size.X, 0), () => {
						fish.Transform.Position.X = SceneManager.ScreenSize.X - fish.Transform.Size.X;
						fish.Transform.Scale.X = -1;
					});
					animationClip.AddClipPoint(2000, swimTexture, new Vector2(400 - fish.Transform.Size.X, fish.Transform.Position.Y, 400, 0, fish.Transform.Size.X, 0), () => {
						fish.Transform.Scale.X = 1;
					});
					animationClip.AddClipPoint(2000, swimTexture, new Vector2(200, fish.Transform.Position.Y + 100, 400, 0, fish.Transform.Size.X, 0), () => {
						fish.Transform.Rotation = 30;
					});
					animationClip.AddClipPoint(2000, swimTexture, new Vector2(100, fish.Transform.Position.Y, 400, 0, fish.Transform.Size.X, 0), () => {
						fish.Transform.Rotation = -30;
					});
					animationClip.AddClipPoint(2000, swimTexture, new Vector2(0, fish.Transform.Position.Y + 100, 400, 0, fish.Transform.Size.X, 0), () => {
						fish.Transform.Rotation = 30;
					});
					animationClip.AddClipPoint(2000);
					animationClip.AddClipPoint(2000, swimTexture, new Vector2(100, fish.Transform.Position.Y, 400, 0, fish.Transform.Size.X, 0), () => {
						fish.Transform.Rotation = 30;
						fish.Transform.Scale.X = -1;
					});
					animationClip.AddClipPoint(2000, swimTexture, new Vector2(200, fish.Transform.Position.Y + 100, 400, 0, fish.Transform.Size.X, 0), () => {
						fish.Transform.Rotation = -30;
					});
					animationClip.AddClipPoint(2000, swimTexture, new Vector2(400 - fish.Transform.Size.X, fish.Transform.Position.Y, 400, 0, fish.Transform.Size.X, 0), () => {
						fish.Transform.Rotation = 30;
					});
					animationClip.AddClipPoint(2000, restTexture);
					break;
			}

			return animationClip;
		}
	}
}

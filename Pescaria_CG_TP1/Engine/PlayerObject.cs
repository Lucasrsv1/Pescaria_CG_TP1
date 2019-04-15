using Pescaria_CG_TP1.Scenes;
using SharpGL;
using SharpGL.Enumerations;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Pescaria_CG_TP1.Engine {
	public class PlayerObject : GameObject {
		public PlayerObject (Vector2 size, string tag = "", Vector2 position = null, double rotation = 0, Vector2 scale = null) : base(size, tag, position, rotation, scale) {
			AnimationClip animationClip = new AnimationClip(AnimationClip.ClipTypes.ONCE);
			animationClip.AddClipPoint(5000, AnimationClip.CURRENT_TEXTURE, new Vector2(Transform.DISABLE_AXIS_MOVIMENT, 500, 0, SceneManager.ScreenSize.Y));
			animationClip.AddClipPoint(7000, AnimationClip.CURRENT_TEXTURE, new Vector2(Transform.DISABLE_AXIS_MOVIMENT, 1500));
			animationClip.AddClipPoint(5000, AnimationClip.CURRENT_TEXTURE, new Vector2(Transform.DISABLE_AXIS_MOVIMENT, 2500));
			animationClip.AddClipPoint(26000, AnimationClip.CURRENT_TEXTURE, new Vector2(Transform.DISABLE_AXIS_MOVIMENT, 9500));
			animationClip.AddClipPoint(2000, "", new Vector2(Transform.DISABLE_AXIS_MOVIMENT, 9500));
			animationClip.AddClipPoint(2000);
			animationClip.AddClipPoint(14000, AnimationClip.CURRENT_TEXTURE, new Vector2(Transform.DISABLE_AXIS_MOVIMENT, -20));
			animationClip.AddClipPoint(200, AnimationClip.CURRENT_TEXTURE, new Vector2(Transform.DISABLE_AXIS_MOVIMENT, -100), () => {
				this.Transform.Scale.Y = -1;
			});

			animationClip.AddClipPoint(15000, AnimationClip.INVISIBLE, new Vector2(Transform.DISABLE_AXIS_MOVIMENT, -7000), () => {
				// Start part II
				Game.CreateClouds();
				Random random = new Random();
				for (int i = 0; i < SceneManager.SceneObjects.Count; i++) {
					// Remove objects that are not needed anymore
					if (SceneManager.SceneObjects[i].Tag == "Bubble" || SceneManager.SceneObjects[i].Tag == "Fish" || SceneManager.SceneObjects[i].Tag == "Background Bottom")
						SceneManager.SceneObjects[i--].Destroy();
				}

				for (int i = 0; i < SceneManager.SceneObjects.Count; i++) {
					// Add moviment to the fishes that the player must click on
					GameObject obj = SceneManager.SceneObjects[i];
					if (obj.Tag != "Hooked_Fish") continue;

					obj.Transform.Spin(5);
					obj.Transform.RemovePositionFn();
					obj.Animator.StopAnimationClip();

					AnimationClip fishAnimationClip = new AnimationClip(AnimationClip.ClipTypes.ONCE);
					fishAnimationClip.AddClipPoint((int) (14000 + (random.NextDouble() * 2000)), AnimationClip.CURRENT_TEXTURE, new Vector2((float) random.NextDouble() * (SceneManager.ScreenSize.X - obj.Transform.Size.X), -7000, SceneManager.ScreenSize.X, 0, obj.Transform.Size.X, 0));
					fishAnimationClip.AddClipPoint(4500, AnimationClip.CURRENT_TEXTURE, new Vector2(Transform.DISABLE_AXIS_MOVIMENT, 0));

					obj.Animator.AddAnimationClip("CLIP_2", fishAnimationClip);
					obj.Animator.PlayAnimationClip("CLIP_2");
					obj.AddOnClickListener(() => {
						obj.Destroy();
						this.FishScore += (int) Math.Round(obj.Transform.Size.Y / 10f) * 10;
					});
				}
			});

			animationClip.AddClipPoint(5000, AnimationClip.CURRENT_TEXTURE, new Vector2(Transform.DISABLE_AXIS_MOVIMENT, -250));
			animationClip.AddClipPoint(500, AnimationClip.CURRENT_TEXTURE, null, () => {
				for (int i = 0; i < SceneManager.SceneObjects.Count; i++) {
					// Remove objects that are not needed anymore
					if (SceneManager.SceneObjects[i].Tag == "Hooked_Fish" || SceneManager.SceneObjects[i].Tag == "Aim")
						SceneManager.SceneObjects[i--].Destroy();
				}
			});

			this.Animator.AddAnimationClip("CLIP", animationClip);
			this.Animator.PlayAnimationClip("CLIP");
			this.Lives = 4;
			this.FishScore = 0;
			this.BubblesScore = 0;
		}

		private int bubblesScore;
		private int movimentSpeed = 1;
		private DateTime lastKeyTime = SceneManager.Now;
		private Vector2 speedCounter = Vector2.Zero;

		public int Lives { get; set; }
		public int FishScore { get; set; }
		public int BubblesScore {
			get {
				return this.bubblesScore;
			}
			set {
				if (value >= 10 && this.Lives < 4) {
					this.Lives++;
					this.bubblesScore = 0;
				} else {
					this.bubblesScore = value;
				}
			}
		}

		public void PreviewKeyDown (PreviewKeyDownEventArgs e) {
			if (SceneManager.IsPaused && !this.InteractiveDuringPause) return;

			if (SceneManager.Now.Subtract(lastKeyTime).TotalMilliseconds > 600)
				movimentSpeed = 1;

			if ((e.KeyCode == Keys.Left || e.KeyCode == Keys.A) && this.Transform.Position.X > 0) {
				if (speedCounter.Y > 0) movimentSpeed = 1;
				this.Transform.Position.X -= movimentSpeed;

				if (++speedCounter.X > 15) {
					movimentSpeed = Math.Min(movimentSpeed + 1, 10);
					speedCounter.X = 0;
				}

				speedCounter.Y = 0;
				lastKeyTime = SceneManager.Now;
			} else if ((e.KeyCode == Keys.Right || e.KeyCode == Keys.D) && this.Transform.Position.X < SceneManager.ScreenSize.X - this.Transform.Size.X) {
				if (speedCounter.X > 0) movimentSpeed = 1;
				this.Transform.Position.X += movimentSpeed;

				if (++speedCounter.Y > 15) {
					movimentSpeed = Math.Min(movimentSpeed + 1, 10);
					speedCounter.Y = 0;
				}

				speedCounter.X = 0;
				lastKeyTime = SceneManager.Now;
			}
		}

		public override void OpenGLDraw (int glWidth, int glHeight) {
			Transform.Rotation = (30 * (Transform.Position.X / SceneManager.ScreenSize.X)) / 0.5 - 30;
			base.OpenGLDraw(glWidth, glHeight);
			if (this.Animator.CurrentTexture == "") return;

			// Adjust the rope when the hook is rotated
			float rotationFactor = 0.25f + (0.025f * (30 - (float) Transform.Rotation));
			Vector2 start = new Vector2(this.Transform.Position.X + (this.Transform.Size.X * 0.575f * rotationFactor), this.Transform.Position.Y + (this.Transform.Size.Y * (this.Transform.Scale.Y == -1 ? 0.87f : 0.13f)));
			Vector2 screenXMiddle = new Vector2((SceneManager.ScreenSize.X - 34) / 2f, 0);

			gl.Color(1f, 0.686f, 0.176f);
			gl.PointSize(7);
			gl.Begin(BeginMode.Points);
				gl.Vertex(start.X, start.Y, 1);
			gl.End();

			gl.LineWidth(3.5f);
			gl.Begin(BeginMode.Lines);
				gl.Vertex(start.X, start.Y, 1);
				gl.Vertex(screenXMiddle.X, 0, 1);
			gl.End();

			gl.Color(1f, 1f, 1f);
		}
	}
}

using SharpGL;
using SharpGL.Enumerations;
using System;
using System.Windows.Forms;

namespace Pescaria_CG_TP1.Engine {
	public class PlayerObject : GameObject {
		public PlayerObject (OpenGL gl, Vector2 size, Vector2 position = null, string tag = "") : base(gl, size, position, tag) {
			AnimationClip animationClip = new AnimationClip(AnimationClip.ClipTypes.ONCE);
			animationClip.AddClipPoint(5000, "", new Vector2(0, 500, 0, SceneManager.ScreenSize.Y));
			animationClip.AddClipPoint(8000, "", new Vector2(0, 1000));
			animationClip.AddClipPoint(5000, "", new Vector2(0, 1000));
			animationClip.AddClipPoint(28000, "", new Vector2(0, 7000));
			animationClip.AddClipPoint(3000);
			animationClip.AddClipPoint(35000, "", new Vector2(0, -9500));
			this.Animator.AddAnimationClip("CLIP", animationClip);
			this.Animator.PlayAnimationClip("CLIP");
		}

		private int movimentSpeed = 1;
		private DateTime lastKeyTime = SceneManager.Now;
		private Vector2 speedCounter = Vector2.Zero;

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
			base.OpenGLDraw(glWidth, glHeight);

			Vector2 start = new Vector2(this.Transform.Position.X + (this.Transform.Size.X * 0.575f), this.Transform.Position.Y + (this.Transform.Size.Y * 0.11f));
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

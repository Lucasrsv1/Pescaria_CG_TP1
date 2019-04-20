using System;
using System.Collections.Generic;
using static Pescaria_CG_TP1.Engine.AnimationClip;

namespace Pescaria_CG_TP1.Engine {
	public class Transform {
		public const int DISABLE_AXIS_MOVIMENT = int.MinValue;

		private class Moviment {
			public Moviment (Vector2 newPosition, int duration, Vector2 startPosition) {
				this.ToPosition = newPosition;
				this.Duration = duration;
				this.StartPosition = startPosition;
				this.Initiated = SceneManager.Now;
			}

			public int Duration { get; private set; }
			public Vector2 ToPosition { get; private set; }
			public Vector2 StartPosition { get; private set; }
			public DateTime Initiated { get; private set; }
		}

		public Transform (GameObject gameObject, Vector2 size, Vector2 position = null, double rotation = 0, Vector2 scale = null, string tag = "") {
			this.GameObject = gameObject;
			this.Size = size;
			this.Position = position ?? Vector2.Zero;
			this.Rotation = rotation;
			this.Scale = scale ?? Vector2.One;
			this.Tag = tag;

			this.moviments = new List<Moviment>();
		}

		private float spinVelocity = 0;
		private List<Moviment> moviments;
		private Callback setPositionFn;

		public string Tag { get; set; }
		public double Rotation { get; set; }
		public GameObject GameObject { get; private set; }
		public Vector2 Scale { get; set; }
		public Vector2 Position { get; set; }
		public Vector2 Size { get; set; }

		public Vector2 PhysicalPosition {
			get {
				Vector2 result = this.Position.Clone();
				if (this.Scale.X == -1)
					result.X += this.Size.X;

				if (this.Scale.Y == -1)
					result.Y += this.Size.Y;

				return result;
			}
		}

		public void Translate (Vector2 newPosition, int duration) {
			moviments.Add(new Moviment(newPosition, duration, this.Position));
		}

		public void StopTranslations () {
			this.moviments.Clear();
		}

		public void SetPositionFn (Callback cb) {
			this.setPositionFn = cb;
			this.moviments.Clear();
		}

		public void RemovePositionFn () {
			this.setPositionFn = null;
		}

		public void Spin (float speed) {
			spinVelocity = speed;
		}

		public void OpenGLDraw () {
			if (!SceneManager.IsPaused)
				this.Rotation += this.spinVelocity;

			if (this.setPositionFn != null) {
				this.setPositionFn();
				return;
			}

			for (int f = 0; f < moviments.Count; f++) {
				double timePassed = SceneManager.Now.Subtract(this.moviments[f].Initiated).TotalMilliseconds;
				if (timePassed > this.moviments[f].Duration)
					timePassed = this.moviments[f].Duration;

				Vector2 move = new Vector2(this.moviments[f].ToPosition.X != DISABLE_AXIS_MOVIMENT ? this.moviments[f].ToPosition.X : this.moviments[f].StartPosition.X, this.moviments[f].ToPosition.Y != DISABLE_AXIS_MOVIMENT ? this.moviments[f].ToPosition.Y : this.moviments[f].StartPosition.Y) - this.moviments[f].StartPosition;
				Vector2 toMoveNow = this.moviments[f].StartPosition + (move * (timePassed / this.moviments[f].Duration));

				if (this.moviments[f].ToPosition.X == DISABLE_AXIS_MOVIMENT)
					toMoveNow.X = this.Position.X;

				if (this.moviments[f].ToPosition.Y == DISABLE_AXIS_MOVIMENT)
					toMoveNow.Y = this.Position.Y;

				this.Position = toMoveNow;

				// Remove the moviments that have already been executed
				if (timePassed == this.moviments[f].Duration)
					this.moviments.RemoveAt(f--);
			}
		}

		public override string ToString () {
			return string.Format("{{ X: {0}, Y: {1}, Width: {2}, Height: {3} }}", this.Position.X, this.Position.Y, this.Size.X, this.Size.Y);
		}
	}
}

using System;
using System.Collections.Generic;

namespace Pescaria_CG_TP1.Engine {
	public class Transform {
		private class Force {
			public Force (Vector2 move, int duration) {
				this.ToMove = move;
				this.Duration = duration;
				this.MovimentInitiated = SceneManager.Now;
				this.Moved = Vector2.Zero;
			}

			public int Duration { get; private set; }
			public Vector2 Moved { get; private set; }
			public Vector2 ToMove { get; private set; }
			public DateTime MovimentInitiated { get; private set; }

			public void AddToMoved (Vector2 moved) {
				this.Moved += moved;
			}
		}

		public Transform (Vector2 size, Vector2 position = null) {
			this.Size = size;
			this.Position = position != null ? position : Vector2.Zero;
			this.forces = new List<Force>();
		}

		private List<Force> forces;

		public Vector2 Position { get; set; }
		public Vector2 Size { get; set; }

		public void Translate (Vector2 move, int duration) {
			forces.Add(new Force(move, duration));
		}

		public void OpenGLDraw () {
			for (int f = 0; f < forces.Count; f++) {
				double timePassed = SceneManager.Now.Subtract(this.forces[f].MovimentInitiated).TotalMilliseconds;
				if (timePassed > this.forces[f].Duration)
					timePassed = this.forces[f].Duration;

				Vector2 toMoveNow = (this.forces[f].ToMove * (timePassed / this.forces[f].Duration)) - this.forces[f].Moved;

				this.Position += toMoveNow;
				this.forces[f].AddToMoved(toMoveNow);

				// Remove as forças que já foram totalmente executadas
				if (timePassed == this.forces[f].Duration)
					this.forces.RemoveAt(f--);
			}
		}
	}
}

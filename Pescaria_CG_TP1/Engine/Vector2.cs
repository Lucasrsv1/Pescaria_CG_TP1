using System;
using System.Drawing;

namespace Pescaria_CG_TP1.Engine {
	public class Vector2 {
		public static Vector2 Identity {
			get {
				return new Vector2(1, 1);
			}
		}

		public static Vector2 Zero {
			get {
				return new Vector2();
			}
		}

		public static bool Overlap (Transform transform, Vector2 point) {
			return  point.X > transform.Position.X && point.X < transform.Position.X + transform.Size.X &&
					point.Y > transform.Position.Y && point.Y < transform.Position.Y + transform.Size.Y;
		}

		public static bool Overlap (Transform transform, Transform transform2) {
			return  transform2.Position.X > transform.Position.X && transform2.Position.X < transform.Position.X + transform.Size.X &&
					transform2.Position.Y > transform.Position.Y && transform2.Position.Y < transform.Position.Y + transform.Size.Y;
		}

		public Vector2 () {
			this.refX = 0;
			this.refY = 0;
			this.x = 0;
			this.y = 0;
		}

		public Vector2 (float x, float y) {
			this.refX = 0;
			this.refY = 0;
			this.X = x;
			this.Y = y;
		}

		public Vector2 (Point point) {
			this.x = point.X;
			this.y = point.Y;
		}

		public Vector2 (float x, float y, float refX, float refY) {
			this.refX = refX;
			this.refY = refY;
			this.X = x;
			this.Y = y;
		}

		private float x;
		private float y;
		private float refX;
		private float refY;

		public float X {
			get {
				if (refX == 0)
					return x;
				else
					return x * SceneManager.ScreenSize.X / refX;
			}
			set {
				x = value;
			}
		}

		public float Y {
			get {
				if (refY == 0)
					return y;
				else
					return y * SceneManager.ScreenSize.Y / refY;
			}
			set {
				y = value;
			}
		}

		public float Magnitude () {
			return (float) Math.Sqrt(Math.Pow(this.X, 2) + Math.Pow(this.Y, 2));
		}

		public Vector2 Normalize () {
			return new Vector2(this.X / this.Magnitude(), this.Y / this.Magnitude());
		}

		public static Vector2 operator + (Vector2 a, Vector2 b) {
			return new Vector2(a.x + b.x, a.y + b.y, a.refX, a.refY);
		}

		public static Vector2 operator - (Vector2 a, Vector2 b) {
			return new Vector2(a.x - b.x, a.y - b.y, a.refX, a.refY);
		}

		public static Vector2 operator - (Vector2 a) {
			return new Vector2(-a.x, -a.y, a.refX, a.refY);
		}

		public static Vector2 operator / (Vector2 a, double b) {
			return new Vector2(a.x / (float)b, a.y / (float)b, a.refX, a.refY);
		}

		public static Vector2 operator * (Vector2 a, double b) {
			return new Vector2(a.x * (float) b, a.y * (float) b, a.refX, a.refY);
		}

		public override string ToString () {
			return string.Format("{{ X: {0}, Y: {1} }}", this.X, this.Y);
		}
	}
}

using System;

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

		public Vector2 () {
			this.x = 0;
			this.y = 0;
		}

		public Vector2 (float x, float y) {
			this.x = x;
			this.y = y;
		}

		public float x;
		public float y;

		public float Magnitude () {
			return (float) Math.Sqrt(Math.Pow(this.x, 2) + Math.Pow(this.y, 2));
		}

		public Vector2 Normalize () {
			return new Vector2(this.x / this.Magnitude(), this.y / this.Magnitude());
		}

		public static Vector2 operator + (Vector2 a, Vector2 b) {
			return new Vector2(a.x + b.x, a.y + b.y);
		}

		public static Vector2 operator - (Vector2 a, Vector2 b) {
			return new Vector2(a.x - b.x, a.y - b.y);
		}

		public static Vector2 operator - (Vector2 a) {
			return new Vector2(-a.x, -a.y);
		}

		public static Vector2 operator / (Vector2 a, double b) {
			return new Vector2(a.x / (float)b, a.y / (float)b);
		}

		public static Vector2 operator * (Vector2 a, double b) {
			return new Vector2(a.x * (float) b, a.y * (float) b);
		}

		public override string ToString () {
			return string.Format("{{ X: {0}, Y: {1} }}", this.x, this.y);
		}
	}
}

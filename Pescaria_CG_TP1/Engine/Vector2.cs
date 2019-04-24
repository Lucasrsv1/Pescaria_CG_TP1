///-----------------------------------------------------------------
///   Namespace:		Pescaria_CG_TP1.Engine
///   Class:			Vector2
///   Description:		Represents points and vectors in the coordinate system. Also has proportion and overlap test capabilities.
///   Subject:			Computer Graphics
///   Author:			Lucas Rassilan Vilanova
///-----------------------------------------------------------------

using System;
using System.Drawing;

namespace Pescaria_CG_TP1.Engine {
	public class Vector2 {
		public static Vector2 One {
			get {
				return new Vector2(1, 1);
			}
		}

		public static Vector2 Zero {
			get {
				return new Vector2(0, 0);
			}
		}

		// Check collision between a game objects and a point (usually the cursor)
		public static bool Overlap (Transform transform, Vector2 point) {
			return  point.X >= transform.PhysicalPosition.X && point.X <= transform.PhysicalPosition.X + transform.Size.X &&
					point.Y >= transform.PhysicalPosition.Y && point.Y <= transform.PhysicalPosition.Y + transform.Size.Y;
		}

		// Check collision between two game objects
		public static bool Overlap (Transform transform, Transform transform2) {
			return  (transform2.PhysicalPosition.X >= transform.PhysicalPosition.X && transform2.PhysicalPosition.X <= transform.PhysicalPosition.X + transform.Size.X ||
					transform.PhysicalPosition.X >= transform2.PhysicalPosition.X && transform.PhysicalPosition.X <= transform2.PhysicalPosition.X + transform2.Size.X) &&
					(transform2.PhysicalPosition.Y >= transform.PhysicalPosition.Y && transform2.PhysicalPosition.Y <= transform.PhysicalPosition.Y + transform.Size.Y ||
					transform.PhysicalPosition.Y >= transform2.PhysicalPosition.Y && transform.PhysicalPosition.Y <= transform2.PhysicalPosition.Y + transform2.Size.Y);
		}

		public Vector2 (float x, float y) {
			this.X = x;
			this.Y = y;

			this.refX = 0;
			this.refY = 0;
		}

		public Vector2 (Point point) {
			this.X = point.X;
			this.Y = point.Y;

			this.refX = 0;
			this.refY = 0;
			this.constX = 0;
			this.constY = 0;
		}

		public Vector2 (float x, float y, float refX, float refY, float constX = 0, float constY = 0) {
			this.X = x;
			this.Y = y;

			this.refX = refX;
			this.refY = refY;
			this.constX = constX;
			this.constY = constY;
		}

		// Vector's actual values
		private float x;
		private float y;

		// Reference values to use when calculating the porportional value of the vector
		private float refX;
		private float refY;

		// Constant values to be removed from the proportional calculation of the vector's value (usually the size of a game object)
		private float constX;
		private float constY;

		// Calculate X axis proportional value
		public float X {
			get {
				if (refX == 0)
					return x;
				else
					return (x * (SceneManager.ScreenSize.X - constX)) / (refX - constX);
			}
			set {
				if (refX == 0)
					x = value;
				else
					x = (value * (refX - constX)) / (SceneManager.ScreenSize.X - constX);
			}
		}

		// Calculate Y axis proportional value
		public float Y {
			get {
				if (refY == 0)
					return y;
				else
					return (y * (SceneManager.ScreenSize.Y - constY)) / (refY - constY);
			}
			set {
				if (refY == 0)
					y = value;
				else
					y = (value * (refY - constY)) / (SceneManager.ScreenSize.Y - constY);
			}
		}

		public void ClearReferences () {
			this.refX = 0;
			this.refY = 0;
		}

		public Vector2 WithRef (Vector2 reference, float constX = 0, float constY = 0) {
			return new Vector2(this.X, this.Y, reference.X, reference.Y, constX, constY);
		}

		public Vector2 Clone () {
			return new Vector2(this.x, this.y, this.refX, this.refY, this.constX, this.constY);
		}

		// Mathematical Operations:

		public float Magnitude () {
			return (float) Math.Sqrt(Math.Pow(this.X, 2) + Math.Pow(this.Y, 2));
		}

		public Vector2 Normalize () {
			return new Vector2(this.X / this.Magnitude(), this.Y / this.Magnitude());
		}

		public static Vector2 operator + (Vector2 a, Vector2 b) {
			return new Vector2(a.x + b.X, a.y + b.Y, a.refX, a.refY, a.constX, a.constY);
		}

		public static Vector2 operator - (Vector2 a, Vector2 b) {
			return new Vector2(a.x - b.X, a.y - b.Y, a.refX, a.refY, a.constX, a.constY);
		}

		public static Vector2 operator - (Vector2 a) {
			return new Vector2(-a.x, -a.y, a.refX, a.refY, a.constX, a.constY);
		}

		public static Vector2 operator / (Vector2 a, double b) {
			return new Vector2(a.x / (float)b, a.y / (float)b, a.refX, a.refY, a.constX, a.constY);
		}

		public static Vector2 operator * (Vector2 a, double b) {
			return new Vector2(a.x * (float) b, a.y * (float) b, a.refX, a.refY, a.constX, a.constY);
		}

		public override string ToString () {
			return string.Format("{{ X: {0}, Y: {1} }}", this.X, this.Y);
		}
	}
}

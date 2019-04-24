///-----------------------------------------------------------------
///   Namespace:		Pescaria_CG_TP1.Engine
///   Class:			Texture
///   Description:		Load a sprite and handle its frames in order to create animations.
///   Subject:			Computer Graphics
///   Author:			Lucas Rassilan Vilanova
///-----------------------------------------------------------------

using System;
using SharpGL;

namespace Pescaria_CG_TP1.Engine {
	public class Texture {
		public enum Orientations { HORIZONTAL, VERTICAL, BOTH }
		public enum CoordinatesPosition { TOP_LEFT, TOP_RIGHT, BOTTOM_LEFT, BOTTOM_RIGHT }

		public Texture (OpenGL gl, uint id, int qtyFrames) {
			this.gl = gl;
			this.ID = id;
			this.QtyFrames = qtyFrames;
			this.Duration = 1;
			this.Orientation = Orientations.HORIZONTAL;
		}

		public Texture (OpenGL gl, uint id, int qtyFrames, int duration, Orientations orientation, int xFramesQty = 0) {
			this.gl = gl;
			this.ID = id;
			this.QtyFrames = qtyFrames;
			this.Duration = duration;
			this.Orientation = orientation;
			this.XFramesQty = xFramesQty;
		}

		private OpenGL gl;

		public uint ID;
		public int QtyFrames;
		public int Duration;
		public int XFramesQty;
		public Orientations Orientation;

		public float FrameDuration {
			get {
				return this.Duration / this.QtyFrames;
			}
		}

		public void SetFrameCoordinates (int frame, CoordinatesPosition position) {
			Vector2 coord = Vector2.One;
			Vector2 frameSize;
			if (this.Orientation == Orientations.BOTH)
				frameSize = new Vector2(1f / this.XFramesQty, 1f / (float) Math.Ceiling(this.QtyFrames / (float)this.XFramesQty));
			else
				frameSize = new Vector2(1f / this.QtyFrames, 1f / this.QtyFrames);

			if (this.Orientation == Orientations.HORIZONTAL) {
				coord.X = frameSize.X * frame;
			} else if (this.Orientation == Orientations.VERTICAL) {
				coord.Y = frameSize.Y * frame;
			} else {
				coord.X = frameSize.X * (frame % this.XFramesQty);
				coord.Y = frameSize.Y * (frame / this.XFramesQty);
			}

			switch (position) {
				case CoordinatesPosition.TOP_RIGHT:
					coord.X += this.Orientation == Orientations.VERTICAL ? 1 : frameSize.X;
					break;
				case CoordinatesPosition.BOTTOM_LEFT:
					coord.Y += this.Orientation == Orientations.HORIZONTAL ? 1 : frameSize.Y;
					break;
				case CoordinatesPosition.BOTTOM_RIGHT:
					coord.X += this.Orientation == Orientations.VERTICAL ? 1 : frameSize.X;
					coord.Y += this.Orientation == Orientations.HORIZONTAL ? 1 : frameSize.Y;
					break;
			}

			gl.TexCoord(coord.X, coord.Y);
		}
	}
}

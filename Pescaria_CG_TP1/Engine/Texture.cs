using SharpGL;

namespace Pescaria_CG_TP1.Engine {
	public class Texture {
		public enum Orientations { HORIZONTAL, VERTICAL }
		public enum CoordinatesPosition { TOP_LEFT, TOP_RIGHT, BOTTOM_LEFT, BOTTOM_RIGHT }

		public Texture (OpenGL gl, uint id, int qtyFrames) {
			this.gl = gl;
			this.ID = id;
			this.QtyFrames = qtyFrames;
			this.Duration = 1;
			this.Orientation = Orientations.HORIZONTAL;
		}

		public Texture (OpenGL gl, uint id, int qtyFrames, int duration, Orientations orientation) {
			this.gl = gl;
			this.ID = id;
			this.QtyFrames = qtyFrames;
			this.Duration = duration;
			this.Orientation = orientation;
		}

		private OpenGL gl;

		public uint ID;
		public int QtyFrames;
		public int Duration;
		public Orientations Orientation;

		public float FrameDuration {
			get {
				return this.Duration / this.QtyFrames;
			}
		}

		public void SetFrameCoordinates (int frame, CoordinatesPosition position) {
			float frameSize = 1f / this.QtyFrames;
			Vector2 coord = Vector2.Identity;

			if (this.Orientation == Orientations.HORIZONTAL)
				coord.x = frameSize * frame;
			else
				coord.y = frameSize * frame;

			switch (position) {
				case CoordinatesPosition.TOP_RIGHT:
					coord.x += this.Orientation == Orientations.HORIZONTAL ? frameSize : 1;
					break;
				case CoordinatesPosition.BOTTOM_LEFT:
					coord.y += this.Orientation == Orientations.HORIZONTAL ? 1 : frameSize;
					break;
				case CoordinatesPosition.BOTTOM_RIGHT:
					coord.x += this.Orientation == Orientations.HORIZONTAL ? frameSize : 1;
					coord.y += this.Orientation == Orientations.HORIZONTAL ? 1 : frameSize;
					break;
			}

			gl.TexCoord(coord.x, coord.y);
		}
	}
}

using SharpGL;

namespace Pescaria_CG_TP1.Engine {
	public class GameObject {
		public GameObject (OpenGL gl, Vector2 size) {
			this.gl = gl;
			this.Animator = new Animator(gl);
			this.Transform = new Transform(size);
		}

		public GameObject (OpenGL gl, Vector2 size, Vector2 position) {
			this.gl = gl;
			this.Animator = new Animator(gl);
			this.Transform = new Transform(size, position);
		}

		private OpenGL gl;

		public Animator Animator { get; private set; }
		public Transform Transform { get; private set; }

		public void OpenGLDraw (int glWidth, int glHeight) {
			this.Transform.OpenGLDraw();
			this.Animator.OpenGLDraw(glWidth, glHeight, this.Transform);
		}
	}
}

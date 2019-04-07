using SharpGL;
using System.Collections.Generic;

namespace Pescaria_CG_TP1.Engine {
	public class GameObject {
		public delegate void EventCallback ();

		public GameObject (OpenGL gl, Vector2 size, Vector2 position = null) {
			this.gl = gl;
			this.Animator = new Animator(gl);
			this.Transform = new Transform(size, position);
			this.InteractiveDuringPause = false;
		}

		private OpenGL gl;

		private List<EventCallback> onClickCallbacks = new List<EventCallback>();
		private List<EventCallback> onCollisionCallbacks = new List<EventCallback>();

		public bool InteractiveDuringPause { get; set; }
		public Animator Animator { get; private set; }
		public Transform Transform { get; private set; }

		public void Clicked () {
			foreach (EventCallback cb in this.onClickCallbacks) cb();
		}

		public void AddOnClickListener (EventCallback eventCallback) {
			this.onClickCallbacks.Add(eventCallback);
		}

		public bool HasClickListeners () {
			return this.onClickCallbacks.Count > 0;
		}

		public bool HasCollisionListeners () {
			return this.onCollisionCallbacks.Count > 0;
		}

		public void AddOnCollisionListener (EventCallback eventCallback) {
			this.onCollisionCallbacks.Add(eventCallback);
		}

		public void Destroy () {
			SceneManager.RemoveObject(this);
		}

		public void OpenGLDraw (int glWidth, int glHeight) {
			this.Transform.OpenGLDraw();
			this.Animator.OpenGLDraw(glWidth, glHeight, this.Transform);
		}
	}
}

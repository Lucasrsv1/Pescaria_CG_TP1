using SharpGL;
using System.Collections.Generic;

namespace Pescaria_CG_TP1.Engine {
	public class GameObject {
		public delegate void EventCallback();
		public delegate void CollisionCallback(GameObject collider);

		public GameObject (OpenGL gl, Vector2 size, Vector2 position = null, string tag = "") {
			this.gl = gl;
			this.Animator = new Animator(gl);
			this.Transform = new Transform(size, position);
			this.InteractiveDuringPause = false;
			this.Tag = tag;
		}

		protected OpenGL gl;

		private List<EventCallback> onClickCallbacks = new List<EventCallback>();
		private List<CollisionCallback> onCollisionCallbacks = new List<CollisionCallback>();

		public string Tag { get; set; }
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

		public void CollisionDetected (GameObject collider) {
			foreach (CollisionCallback cb in this.onCollisionCallbacks) cb(collider);
		}

		public void AddOnCollisionListener (CollisionCallback collisionCallback) {
			this.onCollisionCallbacks.Add(collisionCallback);
		}

		public bool HasCollisionListeners () {
			return this.onCollisionCallbacks.Count > 0;
		}

		public void Destroy () {
			SceneManager.RemoveObject(this);
		}

		public virtual void OpenGLDraw (int glWidth, int glHeight) {
			this.Transform.OpenGLDraw();
			this.Animator.OpenGLDraw(glWidth, glHeight, this.Transform);
		}
	}
}

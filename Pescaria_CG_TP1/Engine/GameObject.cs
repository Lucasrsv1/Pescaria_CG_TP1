using SharpGL;
using System.Collections.Generic;

namespace Pescaria_CG_TP1.Engine {
	public class GameObject {
		public static OpenGL gl;
		public delegate void EventCallback();
		public delegate void CollisionCallback(GameObject collider);

		public GameObject (Vector2 size, string tag = "", Vector2 position = null, double rotation = 0, Vector2 scale = null) {
			this.Animator = new Animator();
			this.Transform = new Transform(size, position, rotation, scale, tag);
			this.InteractiveDuringPause = false;
			this.Tag = tag;
		}

		private List<EventCallback> onClickCallbacks = new List<EventCallback>();
		private Dictionary<string, CollisionCallback> onCollisionCallbacks = new Dictionary<string, CollisionCallback>();

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
			List<CollisionCallback> callbacks = new List<CollisionCallback>();
			foreach (KeyValuePair<string, CollisionCallback> pair in this.onCollisionCallbacks)
				callbacks.Add(pair.Value);		// Get the callbacks

			// And call them. It's necessary to call'em out of the last loop because it might remove itself, changing the loop elements.
			for (int i = 0; i < callbacks.Count; i++)
				callbacks[i](collider);
		}

		public void AddOnCollisionListener (string eventKey, CollisionCallback collisionCallback) {
			this.onCollisionCallbacks.Add(eventKey, collisionCallback);
		}

		public void RemoveCollisionListener (string eventKey) {
			this.onCollisionCallbacks.Remove(eventKey);
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

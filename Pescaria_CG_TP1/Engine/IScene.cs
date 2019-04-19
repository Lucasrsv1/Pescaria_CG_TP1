namespace Pescaria_CG_TP1.Engine {
	public interface IScene {
		void InitScene ();
		void DisposeScene ();
		void OpenGLDraw (int glWidth, int glHeight);
	}
}

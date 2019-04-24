///-----------------------------------------------------------------
///   Namespace:		Pescaria_CG_TP1.Engine
///   Interface:		IScene
///   Description:		Base methods for all game scenes.
///   Subject:			Computer Graphics
///   Author:			Lucas Rassilan Vilanova
///-----------------------------------------------------------------

namespace Pescaria_CG_TP1.Engine {
	public interface IScene {
		void InitScene ();
		void DisposeScene ();
		void OpenGLDraw (int glWidth, int glHeight);
	}
}

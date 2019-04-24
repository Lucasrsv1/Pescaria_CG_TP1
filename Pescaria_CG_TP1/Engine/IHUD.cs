///-----------------------------------------------------------------
///   Namespace:		Pescaria_CG_TP1.Engine
///   Interface:		IHUD
///   Description:		Base methods for all HUD systems.
///   Subject:			Computer Graphics
///   Author:			Lucas Rassilan Vilanova
///-----------------------------------------------------------------

namespace Pescaria_CG_TP1.Engine {
	public interface IHUD {
		void Init ();
		void OpenGLDraw (int glWidth, int glHeight);
	}
}

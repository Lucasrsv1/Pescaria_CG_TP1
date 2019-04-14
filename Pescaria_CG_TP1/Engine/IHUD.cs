using System;

namespace Pescaria_CG_TP1.Engine {
	public interface IHUD {
		void Init ();
		void OpenGLDraw (int glWidth, int glHeight);
	}
}

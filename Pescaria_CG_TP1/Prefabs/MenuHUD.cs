using Pescaria_CG_TP1.Engine;
using System.Drawing;
using SharpGL;

namespace Pescaria_CG_TP1.Prefabs {
	public class MenuHUD : IHUD {
		private static readonly int PADDING = 20;
		private static readonly int FONT_SIZE = 26;
		private static readonly string[] BTN_TEXTS = new string[] { "Carreira", "Arcade", "Sobre", "Sair" };

		public MenuHUD (OpenGL gl) {
			this.gl = gl;
		}

		private OpenGL gl;
		private readonly Vector2 buttonSize = new Vector2(300, 100, 1366, 768);
		private readonly Color buttonBackgroundColor = Color.FromArgb(200, 0, 120, 155);
		private readonly Button[] buttons = new Button[BTN_TEXTS.Length];

		public bool ShowAbout = false;

		public void Init () {
			for (int i = 0; i < BTN_TEXTS.Length; i++) {
				// Create a local variable in this scope that is not shared between the buttons
				int idx = i;

				buttons[idx] = new Button(BTN_TEXTS[idx], FONT_SIZE, this.buttonSize, this.buttonBackgroundColor);
				buttons[idx].Transform.SetPositionFn(() => {
					buttons[idx].Transform.Position.X = SceneManager.ScreenSize.X / 2f - this.buttonSize.X / 2f;
					buttons[idx].Transform.Position.Y = PADDING * (3 + idx) + (int) (FONT_SIZE * 1.5) + this.buttonSize.Y / 2f + this.buttonSize.Y * idx;
				});

				switch (idx) {
					case 0:
						buttons[idx].AddOnClickListener(() => {});
						break;
					case 1:
						buttons[idx].AddOnClickListener(() => {
							SceneManager.LoadScene("GAME");
						});
						break;
					case 2:
						buttons[idx].AddOnClickListener(() => {
							ShowAbout = true;
							for (int j = 0; j < BTN_TEXTS.Length; j++)
								buttons[j].IsHidden = true;
						});
						break;
					case 3:
						buttons[idx].AddOnClickListener(System.Windows.Forms.Application.Exit);
						break;
				}

				SceneManager.AddObject(buttons[idx]);
			}
		}

		public void HideAbout () {
			ShowAbout = false;
			for (int j = 0; j < BTN_TEXTS.Length; j++)
				buttons[j].IsHidden = false;
		}

		public void OpenGLDraw (int glWidth, int glHeight) {
			int titlePos = (int) SceneManager.ScreenSize.Y - PADDING - (int) (FONT_SIZE * 1.5);
			gl.DrawText((int) ((SceneManager.ScreenSize.X - FONT_SIZE * 13) / 2f), titlePos, 1, 1, 1, "Arial", FONT_SIZE * 1.5f, "Bob, The Fisher");

			if (ShowAbout) {
				gl.DrawText((int) ((SceneManager.ScreenSize.X - FONT_SIZE * 20) / 2f), (int) (SceneManager.ScreenSize.Y / 2f + (FONT_SIZE + PADDING) * 3), 1, 1, 1, "Arial", FONT_SIZE, "Desenvolvedor: Lucas Rassilan Vilanova");
				gl.DrawText((int) ((SceneManager.ScreenSize.X - FONT_SIZE * 14) / 2f), (int) (SceneManager.ScreenSize.Y / 2f + (FONT_SIZE + PADDING) * 2), 1, 1, 1, "Arial", FONT_SIZE, "Matéria: Computação Gráfica");
				gl.DrawText((int) ((SceneManager.ScreenSize.X - FONT_SIZE * 13) / 2f), (int) (SceneManager.ScreenSize.Y / 2f + (FONT_SIZE + PADDING)), 1, 1, 1, "Arial", FONT_SIZE, "Professor: Flávio Coutinho");

				gl.DrawText((int) ((SceneManager.ScreenSize.X - FONT_SIZE * 10) / 2f), (int) (SceneManager.ScreenSize.Y / 2f - (FONT_SIZE + PADDING)), 1, 1, 1, "Arial", FONT_SIZE, "1º Semestre de 2019");
				gl.DrawText((int) ((SceneManager.ScreenSize.X - FONT_SIZE * 20) / 2f), (int) (SceneManager.ScreenSize.Y / 2f - (FONT_SIZE + PADDING) * 2), 1, 1, 1, "Arial", FONT_SIZE, "Centro Federal de Educação Tecnológica");
				gl.DrawText((int) ((SceneManager.ScreenSize.X - FONT_SIZE * 8) / 2f), (int) (SceneManager.ScreenSize.Y / 2f - (FONT_SIZE + PADDING) * 3), 1, 1, 1, "Arial", FONT_SIZE, "de Minas Gerais");

				gl.DrawText(PADDING, PADDING, 1, 1, 1, "Arial", FONT_SIZE * 0.7f, "Pressione ESC para voltar ao MENU.");
			}
		}
	}
}

///-----------------------------------------------------------------
///   Namespace:		Pescaria_CG_TP1.Prefabs
///   Class:			MenuHUD
///   Description:		Creates the menu scene HUD, with buttons for navigation.
///   Subject:			Computer Graphics
///   Author:			Lucas Rassilan Vilanova
///-----------------------------------------------------------------

using Pescaria_CG_TP1.Engine;
using System.Drawing;
using SharpGL;
using Pescaria_CG_TP1.Scenes;
using System;

namespace Pescaria_CG_TP1.Prefabs {
	public class MenuHUD : IHUD {
		private static readonly int PADDING = 20;
		private static readonly int FONT_SIZE = 26;
		private static readonly string[] BTN_TEXTS = new string[] { "Nova Carreira", "Fases", "Arcade", "Instruções", "Sobre", "Sair" };
		private static readonly string[] BTN_LEVELS_TEXTS = new string[] { "Um Sonho", "Liberdade", "Estado Mínimo", "Capitalismo", "Felicidade", "Honestidade", "Justiça Social", "Socialismo" };

		public MenuHUD (OpenGL gl) {
			this.gl = gl;
		}

		private OpenGL gl;
		private readonly Vector2 buttonSize = new Vector2(350, 85, 1366, 768);
		private readonly Vector2 buttonLevelsSize = new Vector2(350, 100, 1366, 768);
		private readonly Color buttonBackgroundColor = Color.FromArgb(200, 0, 120, 155);
		private readonly Button[] buttons = new Button[BTN_TEXTS.Length];
		private readonly Button[] levelButtons = new Button[BTN_LEVELS_TEXTS.Length];

		public static bool ShowAbout = false;
		public bool ShowInstructions = false;
		public bool ShowLevels = false;
		private Label about;
		private Label instructions;
		private GameObject labelsBackground;

		public void Init () {
			labelsBackground = new GameObject(SceneManager.ScreenSize.WithRef(SceneManager.ScreenSize));
			labelsBackground.Animator.Color = Color.FromArgb(100, 10, 10, 10);
			labelsBackground.Animator.CurrentTexture = Animator.SOLID_TEXTURE;
			labelsBackground.IsHidden = true;
			SceneManager.AddObject(labelsBackground);

			Vector2 labelSize = SceneManager.ScreenSize.Clone();
			labelSize.Y -= PADDING + FONT_SIZE * 1.5f;
			labelSize = labelSize.WithRef(SceneManager.ScreenSize, 0, PADDING - FONT_SIZE * 1.5f);
			about = new Label("Desenvolvedor: Lucas Rassilan Vilanova\nMatéria: Computação Gráfica\nProfessor: Flávio Coutinho\n\n1º Semestre de 2019\nCentro Federal de Educação Tecnológica\nde Minas Gerais", FONT_SIZE, Color.White, labelSize);
			about.Transform.Position.Y = PADDING + FONT_SIZE * 1.5f;
			about.IsHidden = true;
			SceneManager.AddObject(about);

			instructions = new Label("\nInstruções:\nPara jogar utilize as setas direita e esquerda ou as teclas A e D para movimentar o anzol. Use o mouse para clicar nas bombas e explodi-las antes que elas te atinjam. Caso encoste em uma bomba, você perderá uma vida (ração de peixe). Você pode estourar as bolhas do fundo do oceano para ganhar pontos, sendo que ao acumular 10 bolhas você ganha uma nova vida, podendo ter no máximo 4 vidas.\nApós pescar os peixes e trazê-los a superfície, você deverá clicar sobre eles para coleta-los. Ao coletar um peixe você recebe um valor proporcional ao tamanho dele.\n\nModos de jogo:\nNo modo Campanha, para passar de fase você deverá pescar peixes o suficiente para atingir uma quantidade alvo indicada no centro superior da tela.\nJá no Arcade você pesca até suas vidas acabarem, e as 4 melhores pontuações ficam listadas nos recordes.", (int) (FONT_SIZE * 0.7), Color.White, labelSize, Label.Alignment.RIGHT, 25);
			instructions.Transform.Position.Y = PADDING + FONT_SIZE * 1.5f;
			instructions.IsHidden = true;
			SceneManager.AddObject(instructions);

			for (int i = 0; i < BTN_TEXTS.Length; i++) {
				// Create a local variable in this scope that is not shared between the buttons
				int idx = i;

				buttons[idx] = new Button(BTN_TEXTS[idx], FONT_SIZE, this.buttonBackgroundColor, this.buttonSize);
				buttons[idx].Transform.SetPositionFn(() => {
					buttons[idx].Transform.Position.X = SceneManager.ScreenSize.X / 2f - this.buttonSize.X / 2f;
					buttons[idx].Transform.Position.Y = PADDING * (idx + 1) + (int) (FONT_SIZE * 1.5) + this.buttonSize.Y / 2f + this.buttonSize.Y * idx;
				});

				switch (idx) {
					case 0:
						buttons[idx].AddOnClickListener(() => {
							StoryManager.LEVEL = 0;
							SceneManager.LoadScene("STORY_MANAGER");
						});
						break;
					case 1:
						buttons[idx].AddOnClickListener(() => {
							ShowLevels = true;
							for (int j = 0; j < BTN_TEXTS.Length; j++)
								buttons[j].IsHidden = true;

							for (int j = 0; j < BTN_LEVELS_TEXTS.Length; j++)
								levelButtons[j].IsHidden = false;
						});
						break;
					case 2:
						buttons[idx].AddOnClickListener(() => {
							Game.Goal = 0;
							SceneManager.LoadScene("GAME");
						});
						break;
					case 3:
						buttons[idx].AddOnClickListener(() => {
							ShowInstructions = true;
							labelsBackground.IsHidden = instructions.IsHidden = false;
							for (int j = 0; j < BTN_TEXTS.Length; j++)
								buttons[j].IsHidden = true;
						});
						break;
					case 4:
						buttons[idx].AddOnClickListener(() => {
							ShowAbout = true;
							labelsBackground.IsHidden = about.IsHidden = false;
							for (int j = 0; j < BTN_TEXTS.Length; j++)
								buttons[j].IsHidden = true;
						});
						break;
					case 5:
						buttons[idx].AddOnClickListener(System.Windows.Forms.Application.Exit);
						break;
				}

				SceneManager.AddObject(buttons[idx]);
			}

			for (int i = 0; i < BTN_LEVELS_TEXTS.Length; i++) {
				int idx = i;
				levelButtons[idx] = new Button(BTN_LEVELS_TEXTS[idx], FONT_SIZE, this.buttonBackgroundColor, this.buttonLevelsSize);
				levelButtons[idx].Transform.SetPositionFn(() => {
					levelButtons[idx].Transform.Position.X = SceneManager.ScreenSize.X / 2f + (idx % 2 == 0 ? -this.buttonLevelsSize.X - PADDING : PADDING);
					levelButtons[idx].Transform.Position.Y = PADDING * (3 + (idx / 2)) + (int) (FONT_SIZE * 1.5) + this.buttonLevelsSize.Y / 2f + this.buttonLevelsSize.Y * (idx / 2);
				});

				levelButtons[idx].IsHidden = true;
				levelButtons[idx].AddOnClickListener(() => {
					StoryManager.LEVEL = idx;
					SceneManager.LoadScene("STORY_MANAGER");
				});

				SceneManager.AddObject(levelButtons[idx]);
			}

			if (ShowAbout) {
				labelsBackground.IsHidden = about.IsHidden = false;
				for (int j = 0; j < BTN_TEXTS.Length; j++)
					buttons[j].IsHidden = true;
			}
		}

		public void HideAbout () {
			ShowAbout = false;
			labelsBackground.IsHidden = about.IsHidden = true;
			for (int j = 0; j < BTN_TEXTS.Length; j++)
				buttons[j].IsHidden = false;
		}

		public void HideIstructions () {
			ShowInstructions = false;
			labelsBackground.IsHidden = instructions.IsHidden = true;
			for (int j = 0; j < BTN_TEXTS.Length; j++)
				buttons[j].IsHidden = false;
		}

		public void HideLevels () {
			ShowLevels = false;
			for (int j = 0; j < BTN_LEVELS_TEXTS.Length; j++)
				levelButtons[j].IsHidden = true;

			for (int j = 0; j < BTN_TEXTS.Length; j++)
				buttons[j].IsHidden = false;
		}

		public void OpenGLDraw (int glWidth, int glHeight) {
			int titlePos = (int) SceneManager.ScreenSize.Y - PADDING - (int) (FONT_SIZE * 1.5);
			gl.DrawText((int) ((SceneManager.ScreenSize.X - FONT_SIZE * 13) / 2f), titlePos, 1, 1, 1, "Arial", FONT_SIZE * 1.5f, "Bob, The Fisher");
		}
	}
}

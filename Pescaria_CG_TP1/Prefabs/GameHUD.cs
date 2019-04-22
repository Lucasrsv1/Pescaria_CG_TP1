using Pescaria_CG_TP1.Engine;
using Pescaria_CG_TP1.Scenes;
using System.Drawing;
using SharpGL;

namespace Pescaria_CG_TP1.Prefabs {
	public class GameHUD : IHUD {
		private static int PADDING = 5;
		private static int FONT_SIZE = 24;

		public GameHUD (OpenGL gl) {
			this.gl = gl;
		}

		private readonly Vector2 fishSize = new Vector2(60, 60);
		private readonly Vector2 foodSize = new Vector2(40, 70);
		private readonly Vector2 trophySize = new Vector2(48, 50);
		private readonly Vector2 bubbleSize = new Vector2(45, 45);
		private readonly Vector2 highcoreSize = new Vector2(575, 300);
		private readonly Bitmap BUBBLE_TEXTURE = new Bitmap("./Textures/BUBBLE.png");
		private readonly Bitmap FISH_TEXTURE = new Bitmap("./Textures/FISH3_REST_RIGHT.png");
		private readonly Bitmap[] FISH_FOOD_TEXTURE = new Bitmap[4] {
			new Bitmap("./Textures/POT_BLUE.png"),
			new Bitmap("./Textures/POT_PINK.png"),
			new Bitmap("./Textures/POT_YELLOW.png"),
			new Bitmap("./Textures/POT_GREEN.png")
		};

		private readonly Bitmap[] HIGHSCORE_TEXTURES = new Bitmap[5] {
			new Bitmap("./Textures/BROWN.png"),
			new Bitmap("./Textures/TROPHY_GOLD.png"),
			new Bitmap("./Textures/TROPHY_SILVER.png"),
			new Bitmap("./Textures/TROPHY_BRONZE.png"),
			new Bitmap("./Textures/STAR.png")
		};

		private OpenGL gl;
		private Texture fish;
		private Texture bubble;
		private Texture[] food;
		private Texture[] highscore;

		public static bool IsNewScore = false;
		public static int NewScorePosition = -1;
		public static string PlayerName = "";
		public static string[] Highscores = new string[0];
		public uint FISH_TEXTURE_ID;
		public uint BUBBLE_TEXTURE_ID;
		public uint[] FISH_FOOD_TEXTURE_IDS;
		public uint[] HIGHSCORE_TEXTURES_IDS;

		public void Init () {
			this.FISH_TEXTURE_ID = Animator.RegisterTexture(this.FISH_TEXTURE);
			this.fish = new Texture(this.gl, this.FISH_TEXTURE_ID, 6);

			this.BUBBLE_TEXTURE_ID = Animator.RegisterTexture(BUBBLE_TEXTURE);
			this.bubble = new Texture(this.gl, this.BUBBLE_TEXTURE_ID, 1);

			this.food = new Texture[FISH_FOOD_TEXTURE.Length];
			this.FISH_FOOD_TEXTURE_IDS = new uint[FISH_FOOD_TEXTURE.Length];
			for (int i = 0; i < FISH_FOOD_TEXTURE_IDS.Length; i++) {
				this.FISH_FOOD_TEXTURE_IDS[i] = Animator.RegisterTexture(this.FISH_FOOD_TEXTURE[i]);
				this.food[i] = new Texture(this.gl, this.FISH_FOOD_TEXTURE_IDS[i], 1);
			}

			this.highscore = new Texture[HIGHSCORE_TEXTURES.Length];
			this.HIGHSCORE_TEXTURES_IDS = new uint[HIGHSCORE_TEXTURES.Length];
			for (int i = 0; i < HIGHSCORE_TEXTURES_IDS.Length; i++) {
				this.HIGHSCORE_TEXTURES_IDS[i] = Animator.RegisterTexture(this.HIGHSCORE_TEXTURES[i]);
				this.highscore[i] = new Texture(this.gl, this.HIGHSCORE_TEXTURES_IDS[i], 1);
			}
		}

		public void OpenGLDraw (int glWidth, int glHeight) {
			gl.Enable(OpenGL.GL_TEXTURE_2D);
			gl.PushMatrix();
				// Roll with the camera
				gl.Translate(PADDING, -Game.CameraYPosition + PADDING, 0);

				// Show the bubble counter
				gl.BindTexture(OpenGL.GL_TEXTURE_2D, this.BUBBLE_TEXTURE_ID);
				gl.PushMatrix();
					gl.Translate(this.bubbleSize.X / 2f, (this.foodSize.Y - this.bubbleSize.Y) / 2f + this.bubbleSize.Y / 2f, 0);
					Animator.DrawTexture(this.bubbleSize, this.bubble, 0, 4);
					gl.DrawText((int) (this.bubbleSize.X / 2f), (int) SceneManager.ScreenSize.Y - PADDING - FONT_SIZE - (int) (this.bubbleSize.Y / 2), 1, 1, 1, "Arial", FONT_SIZE, SceneManager.Player.BubblesScore.ToString());
				gl.PopMatrix();

				// Show the player's points
				gl.BindTexture(OpenGL.GL_TEXTURE_2D, this.FISH_TEXTURE_ID);
				gl.PushMatrix();
					gl.Translate(SceneManager.ScreenSize.X / 2f - this.fishSize.X / 2f, (this.foodSize.Y - this.fishSize.Y) / 2f + this.fishSize.Y / 2f, 0);
					Animator.DrawTexture(this.fishSize, this.fish, 0, 4);
					gl.DrawText((int) (SceneManager.ScreenSize.X / 2f) + PADDING * 3, (int) SceneManager.ScreenSize.Y - PADDING - (int) ((this.fishSize.Y + FONT_SIZE) / 2), 1, 1, 1, "Arial", FONT_SIZE, SceneManager.Player.FishScore.ToString() + (Game.Goal > 0 ? "/" + Game.Goal : ""));
				gl.PopMatrix();

				// Show the player's lifes
				for (int i = 0; i < this.food.Length; i++) {
					if (SceneManager.Player.Lives <= i) continue;

					gl.BindTexture(OpenGL.GL_TEXTURE_2D, this.FISH_FOOD_TEXTURE_IDS[i]);
					gl.PushMatrix();
						gl.Translate(SceneManager.ScreenSize.X - (PADDING * (i + 2)) - (this.foodSize.X * i) - this.foodSize.X / 2f, this.foodSize.Y / 2f, 0);
						Animator.DrawTexture(this.foodSize, this.food[i], 0, 4);
					gl.PopMatrix();
				}

				if (Game.GameEnded && Game.Goal <= 0) {
					// Show the highscores
					gl.BindTexture(OpenGL.GL_TEXTURE_2D, this.HIGHSCORE_TEXTURES_IDS[0]);
					gl.PushMatrix();
						gl.Translate(SceneManager.ScreenSize.X / 2f, SceneManager.ScreenSize.Y / 2f, 0);
						Animator.DrawTexture(this.highcoreSize, this.highscore[0], 0, 4);
						gl.DrawText((int) ((SceneManager.ScreenSize.X - FONT_SIZE * 6) / 2f), (int) ((SceneManager.ScreenSize.Y + this.highcoreSize.Y) / 2f - FONT_SIZE * 2), 1, 1, 1, "Arial", FONT_SIZE * 1.25f, "Recordes");

						for (int i = 0; i < this.HIGHSCORE_TEXTURES.Length - 1; i++) {
							gl.BindTexture(OpenGL.GL_TEXTURE_2D, this.HIGHSCORE_TEXTURES_IDS[i + 1]);
							gl.PushMatrix();
								gl.Translate(PADDING * 2 + this.trophySize.X / 2f - this.highcoreSize.X / 2f, FONT_SIZE * 3 - this.highcoreSize.Y / 2f + (PADDING * 2 + this.trophySize.Y) * i, 0);
								Animator.DrawTexture(this.trophySize, this.highscore[i + 1], 0, 4);

								if (NewScorePosition != i)
									gl.DrawText((int) ((SceneManager.ScreenSize.X - this.highcoreSize.X) / 2f + this.trophySize.X + PADDING * 6), (int) (SceneManager.ScreenSize.Y / 2f - FONT_SIZE * 4.25 + (PADDING * 2 + this.trophySize.Y) * (this.HIGHSCORE_TEXTURES.Length - 2 - i) - (this.trophySize.Y - FONT_SIZE) / 2f), 1, 1, 1, "Arial", FONT_SIZE, !string.IsNullOrEmpty(Highscores[i]) ? Highscores[i].Replace(";", ": ") : "Vazio");
								else
									gl.DrawText((int) ((SceneManager.ScreenSize.X - this.highcoreSize.X) / 2f + this.trophySize.X + PADDING * 6), (int) (SceneManager.ScreenSize.Y / 2f - FONT_SIZE * 4.25 + (PADDING * 2 + this.trophySize.Y) * (this.HIGHSCORE_TEXTURES.Length - 2 - i) - (this.trophySize.Y - FONT_SIZE) / 2f), 1, 1, 1, "Arial", FONT_SIZE, PlayerName + "_: " + SceneManager.Player.FishScore);
							gl.PopMatrix();
						}

						if (IsNewScore)
							gl.DrawText(PADDING * 4, PADDING * 4, 1, 1, 1, "Arial", FONT_SIZE * 0.75f, "Pressione ENTER para confirmar o nome.");
						else
							gl.DrawText(PADDING * 4, PADDING * 4, 1, 1, 1, "Arial", FONT_SIZE * 0.75f, "Pressione R para reiniciar ou ESC para voltar ao menu.");
					gl.PopMatrix();
				} else if (Game.GameEnded) {
					gl.DrawText((int) ((SceneManager.ScreenSize.X - FONT_SIZE * 6) / 2f), (int) ((SceneManager.ScreenSize.Y - FONT_SIZE * 1.5) / 2f), 1, 1, 1, "Arial", FONT_SIZE * 1.25f, "Você falhou!");
					gl.DrawText(PADDING * 4, PADDING * 4, 1, 1, 1, "Arial", FONT_SIZE * 0.75f, "Pressione R para reiniciar ou ESC para voltar ao menu.");
				} else if (SceneManager.IsPaused) {
					gl.DrawText((int) ((SceneManager.ScreenSize.X - FONT_SIZE * 4) / 2f), (int) ((SceneManager.ScreenSize.Y - FONT_SIZE * 1.5) / 2f), 1, 1, 1, "Arial", FONT_SIZE * 1.25f, "PAUSE");
					gl.DrawText(PADDING * 4, PADDING * 4, 1, 1, 1, "Arial", FONT_SIZE * 0.75f, "Pressione P para continuar.");
				}
			gl.PopMatrix();
			gl.Disable(OpenGL.GL_TEXTURE_2D);
		}
	}
}

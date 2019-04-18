using Pescaria_CG_TP1.Engine;
using Pescaria_CG_TP1.Scenes;
using System.Drawing;
using SharpGL;
using SharpGL.Enumerations;

namespace Pescaria_CG_TP1.Prefabs {
	public class GameHUD : IHUD {
		private static int PADDING = 5;
		private static int FONT_SIZE = 24;

		public GameHUD (OpenGL gl) {
			this.gl = gl;
		}

		private readonly Vector2 fishSize = new Vector2(60, 60);
		private readonly Vector2 bubbleSize = new Vector2(45, 45);
		private readonly Vector2 foodSize = new Vector2(40, 70);
		private readonly Bitmap BUBBLE_TEXTURE = new Bitmap("./Textures/BUBBLE.png");
		private readonly Bitmap FISH_TEXTURE = new Bitmap("./Textures/FISH3_REST_RIGHT.png");
		private readonly Bitmap[] FISH_FOOD_TEXTURE = new Bitmap[4] {
			new Bitmap("./Textures/POT_BLUE.png"),
			new Bitmap("./Textures/POT_PINK.png"),
			new Bitmap("./Textures/POT_YELLOW.png"),
			new Bitmap("./Textures/POT_GREEN.png")
		};

		private OpenGL gl;
		private Texture fish;
		private Texture bubble;
		private Texture[] food;

		public uint FISH_TEXTURE_ID;
		public uint BUBBLE_TEXTURE_ID;
		public uint[] FISH_FOOD_TEXTURE_IDS;

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
					gl.Begin(BeginMode.TriangleFan);
						this.bubble.SetFrameCoordinates(0, Texture.CoordinatesPosition.TOP_LEFT);
						gl.Vertex(this.bubbleSize.X / -2f, this.bubbleSize.Y / -2f, 4f);

						this.bubble.SetFrameCoordinates(0, Texture.CoordinatesPosition.TOP_RIGHT);
						gl.Vertex(this.bubbleSize.X / 2f, this.bubbleSize.Y / -2f, 4f);

						this.bubble.SetFrameCoordinates(0, Texture.CoordinatesPosition.BOTTOM_RIGHT);
						gl.Vertex(this.bubbleSize.X / 2f, this.bubbleSize.Y / 2f, 4f);

						this.bubble.SetFrameCoordinates(0, Texture.CoordinatesPosition.BOTTOM_LEFT);
						gl.Vertex(this.bubbleSize.X / -2f, this.bubbleSize.Y / 2f, 4f);
					gl.End();
					gl.DrawText((int) (this.bubbleSize.X / 2f), (int) SceneManager.ScreenSize.Y - PADDING - FONT_SIZE - (int) (this.bubbleSize.Y / 2), 1, 1, 1, "Arial", FONT_SIZE, SceneManager.Player.BubblesScore.ToString());
				gl.PopMatrix();

				// Show the player's points
				gl.BindTexture(OpenGL.GL_TEXTURE_2D, this.FISH_TEXTURE_ID);
				gl.PushMatrix();
					gl.Translate(SceneManager.ScreenSize.X / 2f - this.fishSize.X / 2f, (this.foodSize.Y - this.fishSize.Y) / 2f + this.fishSize.Y / 2f, 0);
					gl.Begin(BeginMode.TriangleFan);
						this.fish.SetFrameCoordinates(0, Texture.CoordinatesPosition.TOP_LEFT);
						gl.Vertex(this.fishSize.X / -2f, this.fishSize.Y / -2f, 4f);

						this.fish.SetFrameCoordinates(0, Texture.CoordinatesPosition.TOP_RIGHT);
						gl.Vertex(this.fishSize.X / 2f, this.fishSize.Y / -2f, 4f);

						this.fish.SetFrameCoordinates(0, Texture.CoordinatesPosition.BOTTOM_RIGHT);
						gl.Vertex(this.fishSize.X / 2f, this.fishSize.Y / 2f, 4f);

						this.fish.SetFrameCoordinates(0, Texture.CoordinatesPosition.BOTTOM_LEFT);
						gl.Vertex(this.fishSize.X / -2f, this.fishSize.Y / 2f, 4f);
					gl.End();
					gl.DrawText((int) (SceneManager.ScreenSize.X / 2f) + PADDING * 3, (int) SceneManager.ScreenSize.Y - PADDING - (int) ((this.fishSize.Y + FONT_SIZE) / 2), 1, 1, 1, "Arial", FONT_SIZE, SceneManager.Player.FishScore.ToString());
				gl.PopMatrix();

				// Show the player's lifes
				for (int i = 0; i < this.food.Length; i++) {
					if (SceneManager.Player.Lives <= i) continue;

					gl.BindTexture(OpenGL.GL_TEXTURE_2D, this.FISH_FOOD_TEXTURE_IDS[i]);
					gl.PushMatrix();
						gl.Translate(SceneManager.ScreenSize.X - (PADDING * (i + 2)) - (this.foodSize.X * i) - this.foodSize.X / 2f, this.foodSize.Y / 2f, 0);
						gl.Begin(BeginMode.TriangleFan);
							this.food[i].SetFrameCoordinates(0, Texture.CoordinatesPosition.TOP_LEFT);
							gl.Vertex(this.foodSize.X / -2f, this.foodSize.Y / -2f, 4f);

							this.food[i].SetFrameCoordinates(0, Texture.CoordinatesPosition.TOP_RIGHT);
							gl.Vertex(this.foodSize.X / 2f, this.foodSize.Y / -2f, 4f);

							this.food[i].SetFrameCoordinates(0, Texture.CoordinatesPosition.BOTTOM_RIGHT);
							gl.Vertex(this.foodSize.X / 2f, this.foodSize.Y / 2f, 4f);

							this.food[i].SetFrameCoordinates(0, Texture.CoordinatesPosition.BOTTOM_LEFT);
							gl.Vertex(this.foodSize.X / -2f, this.foodSize.Y / 2f, 4f);
						gl.End();
					gl.PopMatrix();
				}
			gl.PopMatrix();

			if (Game.GameEnded) {
				gl.DrawText((int) ((SceneManager.ScreenSize.X - FONT_SIZE * 6.5) / 2f), (int) ((SceneManager.ScreenSize.Y - FONT_SIZE * 1.5) / 2f), 1, 1, 1, "Arial", FONT_SIZE * 1.25f, "Fim de Jogo!");
				gl.DrawText(PADDING * 4, PADDING * 4, 1, 1, 1, "Arial", FONT_SIZE * 0.75f, "Pressione R para reiniciar ou ESC para voltar ao menu.");
			}

			gl.Disable(OpenGL.GL_TEXTURE_2D);
		}
	}
}

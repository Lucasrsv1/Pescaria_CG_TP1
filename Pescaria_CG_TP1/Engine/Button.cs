using SharpGL;
using System;
using System.Drawing;

namespace Pescaria_CG_TP1.Engine {
	public class Button : GameObject {
		private static uint BUTTON_TEXTURE_ID;
		private static Texture button;
		private static bool texturesRegistered = false;
		private static readonly Bitmap BUTTON_TEXTURE = new Bitmap("./Textures/BUTTON.png");

		public Button (string text, int fontSize, Color color, Vector2 size, string tag = "", Vector2 position = null, double rotation = 0, Vector2 scale = null) : base(size, tag, position, rotation, scale) {
			this.Text = text;
			this.FontSize = fontSize;
			this.Color = color;
			originalColor = color;

			if (!texturesRegistered) {
				BUTTON_TEXTURE_ID = Animator.RegisterTexture(BUTTON_TEXTURE);
				button = new Texture(gl, BUTTON_TEXTURE_ID, 1);
				texturesRegistered = true;
			}

			this.Animator.AddTexture("BACKGROUND", BUTTON_TEXTURE_ID);
			this.Animator.CurrentTexture = "BACKGROUND";
			this.AddOnHoverListener(() => {
				this.Color = Color.FromArgb(originalColor.A, Math.Min(255, originalColor.R + 25), Math.Min(255, originalColor.G + 25), Math.Min(255, originalColor.B + 25));
			});
			this.AddOnMouseLeaveListener(() => {
				this.Color = originalColor;
			});
		}

		private Color originalColor;

		public int FontSize { get; set; }
		public string Text { get; set; }
		public Color Color { get; set; }

		public override void OpenGLDraw (int glWidth, int glHeight) {
			// Set button's background color
			this.Animator.Color = this.Color;

			// Draw background
			base.OpenGLDraw(glWidth, glHeight);

			if (!this.IsHidden)
				gl.DrawText((int) (this.Transform.Position.X + this.Transform.Size.X / 2f - (this.FontSize * (this.Text.Length / 1.75)) / 2f), (int) (SceneManager.ScreenSize.Y - this.Transform.Position.Y - (this.Transform.Size.Y + this.FontSize) / 2.2f), 1, 1, 1, "Arial", this.FontSize, this.Text);
		}
	}
}

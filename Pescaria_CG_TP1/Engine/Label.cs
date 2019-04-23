using System;
using System.Collections.Generic;
using System.Drawing;

namespace Pescaria_CG_TP1.Engine {
	public class Label : GameObject {
		public enum Alignment { CENTER, RIGHT };

		public static string[] GetBrokenStrings (string text, int fontSize, float sizeX) {
			List<string> brokenStrings = new List<string>();
			int maxLength = (int) Math.Round(sizeX / fontSize * 1.75);

			if (text.Length <= maxLength) {
				brokenStrings.Add(text);
			} else {
				string part = text.Substring(0, maxLength);
				if (part.Contains(" ")) {
					brokenStrings.Add(part.Substring(0, part.LastIndexOf(" ")));
					text = text.Substring(part.LastIndexOf(" ") + 1);
				} else {
					brokenStrings.Add(part);
					text = text.Substring(maxLength);
				}

				brokenStrings.AddRange(GetBrokenStrings(text, fontSize, sizeX));
			}

			return brokenStrings.ToArray();
		}

		public Label (string text, int fontSize, Color color, Vector2 size, Alignment textAlignment = Alignment.CENTER, int padding = 0, int lineSpace = 10, string tag = "", Vector2 position = null, double rotation = 0, Vector2 scale = null) : base(size, tag, position, rotation, scale) {
			this.Text = text;
			this.FontSize = fontSize;
			this.Color = color;
			this.LineSpace = lineSpace;
			this.Padding = padding;
			this.TextAlignment = textAlignment;
		}

		public int Padding { get; set; }
		public int FontSize { get; set; }
		public int LineSpace { get; set; }
		public string Text { get; set; }
		public Color Color { get; set; }
		public Alignment TextAlignment { get; set; }

		public override void OpenGLDraw (int glWidth, int glHeight) {
			base.OpenGLDraw(glWidth, glHeight);
			if (this.IsHidden) return;

			List<string> lines = new List<string>();
			string[] textParts = this.Text.Split('\n');
			for (int i = 0; i < textParts.Length; i++)
				lines.AddRange(GetBrokenStrings (textParts[i], this.FontSize, this.Transform.Size.X - this.Padding * 2));

			for (int i = 0; i < lines.Count; i++) {
				if (this.TextAlignment == Alignment.CENTER)
					gl.DrawText((int) Math.Max(0, this.Transform.Position.X + this.Transform.Size.X / 2f - (this.FontSize * (lines[i].Length / 1.75)) / 2f) + this.Padding, (int) Math.Max(0, (SceneManager.ScreenSize.Y - this.Transform.Position.Y - (this.Transform.Size.Y - this.FontSize * lines.Count - this.LineSpace * (lines.Count - 1)) / 2f) - (this.FontSize + this.LineSpace) * i), this.Color.R / 255f, this.Color.G / 255f, this.Color.B / 255f, "Arial", this.FontSize, lines[i]);
				else
					gl.DrawText((int) this.Transform.Position.X + this.Padding, (int) Math.Max(0, (SceneManager.ScreenSize.Y - this.Transform.Position.Y - (this.Transform.Size.Y - this.FontSize * lines.Count - this.LineSpace * (lines.Count - 1)) / 2f) - (this.FontSize + this.LineSpace) * i), this.Color.R / 255f, this.Color.G / 255f, this.Color.B / 255f, "Arial", this.FontSize, lines[i]);
			}
		}
	}
}

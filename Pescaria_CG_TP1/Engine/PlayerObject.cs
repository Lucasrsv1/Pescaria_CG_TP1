using Pescaria_CG_TP1.Prefabs;
using Pescaria_CG_TP1.Scenes;
using SharpGL.Enumerations;
using System;
using System.Collections.Generic;
using System.Media;
using System.Linq;
using System.Windows.Forms;

namespace Pescaria_CG_TP1.Engine {
	public class PlayerObject : GameObject {
		private static readonly SoundPlayer lifeSound = new SoundPlayer("./Audio/Powerup.wav");

		public static int memoryLives = 0;
		public static int memoryFishScore = 0;
		public static int memoryBubbleScore = 0;

		public PlayerObject (Vector2 size, string tag = "", Vector2 position = null, double rotation = 0, Vector2 scale = null) : base(size, tag, position, rotation, scale) {
			AnimationClip animationClip = new AnimationClip(AnimationClip.ClipTypes.ONCE);
			animationClip.AddClipPoint(5000, AnimationClip.CURRENT_TEXTURE, new Vector2(Transform.DISABLE_AXIS_MOVIMENT, 500, 0, SceneManager.ScreenSize.Y));
			animationClip.AddClipPoint(7000, AnimationClip.CURRENT_TEXTURE, new Vector2(Transform.DISABLE_AXIS_MOVIMENT, 1500));
			animationClip.AddClipPoint(5000, AnimationClip.CURRENT_TEXTURE, new Vector2(Transform.DISABLE_AXIS_MOVIMENT, 2500));
			animationClip.AddClipPoint(26000, AnimationClip.CURRENT_TEXTURE, new Vector2(Transform.DISABLE_AXIS_MOVIMENT, 9500));
			animationClip.AddClipPoint(2000, "", new Vector2(Transform.DISABLE_AXIS_MOVIMENT, 9500));
			animationClip.AddClipPoint(2000);
			animationClip.AddClipPoint(14000, AnimationClip.CURRENT_TEXTURE, new Vector2(Transform.DISABLE_AXIS_MOVIMENT, -20));
			animationClip.AddClipPoint(200, AnimationClip.CURRENT_TEXTURE, new Vector2(Transform.DISABLE_AXIS_MOVIMENT, -100), () => {
				this.Transform.Scale.Y = -1;
			});

			animationClip.AddClipPoint(12000, AnimationClip.INVISIBLE, new Vector2(Transform.DISABLE_AXIS_MOVIMENT, -7000), () => {
				// Start part II
				Game.CreateClouds();
				Random random = new Random();
				for (int i = 0; i < SceneManager.SceneObjects.Count; i++) {
					// Remove objects that are not needed anymore
					if (SceneManager.SceneObjects[i].Tag == "Bubble" || SceneManager.SceneObjects[i].Tag == "Fish" || SceneManager.SceneObjects[i].Tag == "Background Bottom")
						SceneManager.SceneObjects[i--].Destroy();
				}

				int totalHookedFishes = 0;
				for (int i = 0; i < SceneManager.SceneObjects.Count; i++) {
					// Add moviment to the fishes that the player must click on
					GameObject obj = SceneManager.SceneObjects[i];
					if (obj.Tag != "Hooked_Fish") continue;

					totalHookedFishes++;
					obj.Transform.Spin(5);
					obj.Transform.RemovePositionFn();
					obj.Animator.StopAnimationClip();

					AnimationClip fishAnimationClip = new AnimationClip(AnimationClip.ClipTypes.ONCE);
					fishAnimationClip.AddClipPoint((int) (11000 + (random.NextDouble() * 2000)), AnimationClip.CURRENT_TEXTURE, new Vector2((float) random.NextDouble() * (SceneManager.ScreenSize.X - obj.Transform.Size.X), -7000, SceneManager.ScreenSize.X, 0, obj.Transform.Size.X, 0));
					fishAnimationClip.AddClipPoint(4500, AnimationClip.CURRENT_TEXTURE, new Vector2(Transform.DISABLE_AXIS_MOVIMENT, 0));

					obj.Animator.AddAnimationClip("CLIP_2", fishAnimationClip);
					obj.Animator.PlayAnimationClip("CLIP_2");
					obj.AddOnClickListener(() => {
						obj.Destroy();
						this.FishScore += (int) Math.Round(obj.Transform.Size.Y / 10f) * 10;
						if (--totalHookedFishes == 0)
							this.Animator.SkipAnimationClipPoint();
					});
				}
			});

			animationClip.AddClipPoint(5000, AnimationClip.CURRENT_TEXTURE, new Vector2(Transform.DISABLE_AXIS_MOVIMENT, -250));
			animationClip.AddClipPoint(3000, AnimationClip.CURRENT_TEXTURE, new Vector2(Transform.DISABLE_AXIS_MOVIMENT, 0));
			animationClip.AddClipPoint(500, AnimationClip.CURRENT_TEXTURE, null, () => {
				memoryLives = this.Lives;
				memoryFishScore = this.FishScore;
				memoryBubbleScore = this.BubblesScore;
				SceneManager.ReleadLevel();
			});

			this.Animator.AddAnimationClip("CLIP", animationClip);
			this.Animator.PlayAnimationClip("CLIP");
			this.Lives = memoryLives > 0 ? memoryLives: 4;
			this.FishScore = memoryFishScore > 0 ? memoryFishScore : 0;
			this.BubblesScore = memoryBubbleScore > 0 ? memoryBubbleScore : 0;

			memoryLives = 0;
			memoryFishScore = 0;
			memoryBubbleScore = 0;
		}

		private int lives;
		private int bubblesScore;
		private int movimentSpeed = 1;
		private DateTime lastKeyTime = SceneManager.Now;
		private Vector2 speedCounter = Vector2.Zero;

		public int FishScore { get; set; }

		public int Lives {
			get {
				return this.lives;
			}
			set {
				this.lives = value;
				if (value <= 0) GameOver();
			}
		}

		public int BubblesScore {
			get {
				return this.bubblesScore;
			}
			set {
				if (value >= 10) {
					this.Lives = Math.Min(4, this.Lives + 1);
					this.bubblesScore = 0;
					if (!SceneManager.IsMute)
						lifeSound.Play();
				} else {
					this.bubblesScore = value;
				}
			}
		}

		private void GameOver () {
			if (Game.GameEnded) return;
			this.Animator.StopAnimationClip();
			this.Animator.CurrentTexture = AnimationClip.INVISIBLE;
			this.Transform.StopTranslations();
			Game.GameEnded = true;
			SceneManager.Aim = null;
			for (int i = 0; i < SceneManager.SceneObjects.Count; i++) {
				// Remove objects that are not needed anymore
				if (SceneManager.SceneObjects[i].Tag == "Hooked_Fish")
					SceneManager.SceneObjects[i--].Destroy();
			}

			Dictionary<string, int> scores = new Dictionary<string, int>();
			string[] highscores = SceneManager.ReadFile("./highscores.dat");
			for (int i = 0; i < 4 && i < highscores.Length; i++) {
				string[] parts = highscores[i].Split(';');
				if (parts.Length > 1) {
					int.TryParse(parts[1], out int v);
					scores.Add(parts[0] + "_" + i, v);
				}
			}

			IOrderedEnumerable<KeyValuePair<string, int>> descScores = from ele in scores
																	   orderby ele.Value descending
																	   select ele;

			GameHUD.Highscores = new string[4] { "", "", "", "" };
			for (int i = 0; i < 4; i++) {
				if (descScores.Count() > i)
					GameHUD.Highscores[i] = descScores.ElementAt(i).Key.Substring(0, descScores.ElementAt(i).Key.LastIndexOf("_")) + ";" + descScores.ElementAt(i).Value;
			}

			for (int i = 0; i < 4; i++) {
				if (!GameHUD.IsNewScore && this.FishScore > 0 && (descScores.Count() > i && this.FishScore > descScores.ElementAt(i).Value || descScores.Count() <= i)) {
					GameHUD.IsNewScore = true;
					GameHUD.PlayerName = "";
					GameHUD.NewScorePosition = i;
					for (int j = 3; j > i; j--)
						GameHUD.Highscores[j] = GameHUD.Highscores[j - 1];
				}
			}
		}

		public void PreviewKeyDown (PreviewKeyDownEventArgs e) {
			if (Game.GameEnded && GameHUD.IsNewScore) {
				 if (GameHUD.PlayerName.Length < 20) {
					if (e.KeyValue >= 65 && e.KeyValue <= 90 || e.KeyValue >= 48 && e.KeyValue <= 57)
						GameHUD.PlayerName += (char) (e.KeyValue + (Control.IsKeyLocked(Keys.CapsLock) ? (e.Shift ? 32 : 0) : (e.Shift ? 0 : 32)));
					else if (e.KeyValue == 32)
						GameHUD.PlayerName += " ";
					else if (e.KeyData == Keys.OemPeriod)
						GameHUD.PlayerName += ".";
				}

				if (e.KeyCode == Keys.Return) {
					GameHUD.Highscores[GameHUD.NewScorePosition] = GameHUD.PlayerName + ";" + this.FishScore;
					GameHUD.IsNewScore = false;
					GameHUD.PlayerName = "";
					GameHUD.NewScorePosition = -1;
					SceneManager.WriteFile("./highscores.dat", GameHUD.Highscores);
				} else if (e.KeyCode == Keys.Back) {
					GameHUD.PlayerName = GameHUD.PlayerName.Substring(0, Math.Max(0, GameHUD.PlayerName.Length - 1));
				}
				return;
			}

			if (SceneManager.IsPaused && !this.InteractiveDuringPause) return;

			if (SceneManager.Now.Subtract(lastKeyTime).TotalMilliseconds > 600)
				movimentSpeed = 1;

			if ((e.KeyCode == Keys.Left || e.KeyCode == Keys.A) && this.Transform.Position.X > 0) {
				if (speedCounter.Y > 0) movimentSpeed = 1;
				this.Transform.Position.X -= movimentSpeed;

				if (++speedCounter.X > 10) {
					movimentSpeed = Math.Min(movimentSpeed + 2, 20);
					speedCounter.X = 0;
				}

				speedCounter.Y = 0;
				lastKeyTime = SceneManager.Now;
			} else if ((e.KeyCode == Keys.Right || e.KeyCode == Keys.D) && this.Transform.Position.X < SceneManager.ScreenSize.X - this.Transform.Size.X) {
				if (speedCounter.X > 0) movimentSpeed = 1;
				this.Transform.Position.X += movimentSpeed;

				if (++speedCounter.Y > 10) {
					movimentSpeed = Math.Min(movimentSpeed + 2, 20);
					speedCounter.Y = 0;
				}

				speedCounter.X = 0;
				lastKeyTime = SceneManager.Now;
			}
		}

		public override void OpenGLDraw (int glWidth, int glHeight) {
			Transform.Rotation = (30 * (Transform.Position.X / SceneManager.ScreenSize.X)) / 0.5 - 30;
			base.OpenGLDraw(glWidth, glHeight);
			if (this.Animator.CurrentTexture == "") return;

			// Adjust the rope when the hook is rotated
			float rotationFactor = 0.25f + (0.025f * (30 - (float) Transform.Rotation));
			Vector2 start = new Vector2(this.Transform.Position.X + (this.Transform.Size.X * 0.575f * rotationFactor), this.Transform.Position.Y + (this.Transform.Size.Y * (this.Transform.Scale.Y == -1 ? 0.87f : 0.13f)));
			Vector2 screenXMiddle = new Vector2((SceneManager.ScreenSize.X - 34) / 2f, 0);

			if (this.Animator.CurrentTexture != AnimationClip.INVISIBLE) {
				gl.Color(1f, 0.686f, 0.176f);
				gl.PointSize(7);
				gl.Begin(BeginMode.Points);
				gl.Vertex(start.X, start.Y, 1);
				gl.End();

				gl.LineWidth(3.5f);
				gl.Begin(BeginMode.Lines);
				gl.Vertex(start.X, start.Y, 1);
				gl.Vertex(screenXMiddle.X, 0, 1);
				gl.End();

				gl.Color(1f, 1f, 1f);
			}
		}
	}
}

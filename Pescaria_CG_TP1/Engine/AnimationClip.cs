﻿using System;
using System.Collections.Generic;

namespace Pescaria_CG_TP1.Engine {
	public class AnimationClip {
		public const string CURRENT_TEXTURE = "";
		public const string INVISIBLE = "INVISIBLE_CONST";
		public enum ClipTypes { ONCE, LOOP, FORWARD_AND_BACKWARD }

		public delegate void Callback ();

		public class ClipPoint {
			public ClipPoint (int duration, string textureKey = "", Vector2 move = null, Callback before = null) {
				this.Duration = duration;
				this.TextureKey = textureKey;
				this.Move = move;
				this.BeforeCallback = before;

				this.MoveAdded = false;
				this.CallbackCalled = false;
			}

			public bool MoveAdded;
			public bool CallbackCalled;
			public int Duration;
			public string TextureKey;
			public Vector2 Move;
			public Callback BeforeCallback;
		}

		public AnimationClip (ClipTypes clipType = ClipTypes.ONCE) {
			this.ClipType = clipType;
			this.clipPoints = new List<ClipPoint>();
			this.started = null;
			this.backward = false;
		}

		private bool backward;
		private DateTime? started;
		private List<ClipPoint> clipPoints;

		public ClipTypes ClipType { get; set; }

		public void AddClipPoint (int duration, string textureKey = "", Vector2 move = null, Callback before = null) {
			this.clipPoints.Add(new ClipPoint(duration, textureKey, move, before));
		}

		public void OpenGLDraw (Animator animator, Transform transform) {
			ClipPoint clip = null;
			if (this.started == null) {
				this.started = SceneManager.Now;
				clip = this.clipPoints[!this.backward ? 0 : this.clipPoints.Count - 1];
			} else {
				double timePassed = SceneManager.Now.Subtract((DateTime)this.started).TotalMilliseconds;
				if (!this.backward) {
					// Encontra o ponto de clipe atual com a animação normal
					for (int c = 0; c < this.clipPoints.Count; c++) {
						if (timePassed < this.clipPoints[c].Duration) {
							clip = this.clipPoints[c];
							break;
						}

						timePassed -= this.clipPoints[c].Duration;
					}
				} else {
					// Encontra o ponto de clipe atual com a animação tocando ao contrário
					for (int c = this.clipPoints.Count - 1; c >= 0; c--) {
						if (timePassed < this.clipPoints[c].Duration) {
							clip = this.clipPoints[c];
							break;
						}

						timePassed -= this.clipPoints[c].Duration;
					}
				}
				
				if (clip == null) {
					// O clipe chegou ao fim
					for (int c = 0; c < this.clipPoints.Count; c++) {
						// Restaura o clip no início
						this.clipPoints[c].MoveAdded = false;
						this.clipPoints[c].CallbackCalled = false;
					}

					switch (this.ClipType) {
						case ClipTypes.ONCE:
							this.started = null;
							animator.StopAnimationClip();
							break;
						case ClipTypes.LOOP:
							this.started = SceneManager.Now;
							clip = this.clipPoints[0];
							break;
						case ClipTypes.FORWARD_AND_BACKWARD:
							this.started = SceneManager.Now;
							clip = this.clipPoints[this.clipPoints.Count - 1];
							this.backward = !this.backward;
							break;
					}
				}
			}

			if (clip != null) {
				if (clip.BeforeCallback != null && !clip.CallbackCalled) {
					clip.BeforeCallback();
					clip.CallbackCalled = true;
				}

				if (!string.IsNullOrEmpty(clip.TextureKey) && animator.CurrentTexture != clip.TextureKey)
					animator.CurrentTexture = clip.TextureKey != INVISIBLE ? clip.TextureKey : "";

				if (clip.Move != null && !clip.MoveAdded) {
					transform.Translate(clip.Move * (!this.backward ? 1 : -1), clip.Duration);
					clip.MoveAdded = true;
				}
			}
		}
	}
}

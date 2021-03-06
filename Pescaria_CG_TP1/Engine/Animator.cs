﻿///-----------------------------------------------------------------
///   Namespace:		Pescaria_CG_TP1.Engine
///   Class:			Animator
///   Description:		Draws an object and handle animations, using textures and animation clips.
///   Subject:			Computer Graphics
///   Author:			Lucas Rassilan Vilanova
///-----------------------------------------------------------------

using SharpGL;
using SharpGL.Enumerations;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace Pescaria_CG_TP1.Engine {
	public class Animator {
		public const string SOLID_TEXTURE = "SOLID_TEXTURE";
		private struct TextureToLoad {
			public TextureToLoad (string textureKey, int timeout) {
				this.Timeout = timeout;
				this.TextureKey = textureKey;
				this.Started = SceneManager.Now;
			}

			public int Timeout;
			public string TextureKey;
			public DateTime Started;
		}

		public static OpenGL gl;
		public static uint RegisterTexture (Bitmap textureImage) {
			uint[] textureID = new uint[1];

			// Initiate texture
			gl.Enable(OpenGL.GL_TEXTURE_2D);

			// Get texture ID and save it in the array
			gl.GenTextures(1, textureID);

			gl.BindTexture(OpenGL.GL_TEXTURE_2D, textureID[0]);

			// Enable transparency
			gl.Enable(OpenGL.GL_BLEND);
			gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);

			// Tells OpenGL where are the image pixels/data
			IntPtr pixels = textureImage.LockBits(new Rectangle(0, 0, textureImage.Width, textureImage.Height), ImageLockMode.ReadOnly, textureImage.PixelFormat).Scan0;
			gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, (int) OpenGL.GL_RGBA, textureImage.Width, textureImage.Height, 0, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, pixels);

			gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);
			gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);

			return textureID[0];
		}

		public static void DrawTexture (Vector2 size, Texture tex = null, int frame = 0, float zIndex = 0) {
			gl.Begin(BeginMode.TriangleFan);
				if (tex != null)
					tex.SetFrameCoordinates(frame, Texture.CoordinatesPosition.TOP_LEFT);
				gl.Vertex(size.X / -2f, size.Y / -2f, zIndex);

				if (tex != null)
					tex.SetFrameCoordinates(frame, Texture.CoordinatesPosition.TOP_RIGHT);
				gl.Vertex(size.X / 2f, size.Y / -2f, zIndex);

				if (tex != null)
					tex.SetFrameCoordinates(frame, Texture.CoordinatesPosition.BOTTOM_RIGHT);
				gl.Vertex(size.X / 2f, size.Y / 2f, zIndex);

				if (tex != null)
					tex.SetFrameCoordinates(frame, Texture.CoordinatesPosition.BOTTOM_LEFT);
				gl.Vertex(size.X / -2f, size.Y / 2f, zIndex);
			gl.End();
		}

		public Animator () {
			this.Textures = new Dictionary<string, Texture>();
			this.AnimationClips = new Dictionary<string, AnimationClip>();

			this.ZIndex = 0;
			this.Color = Color.FromArgb(255, 255, 255, 255);
			this.currentFrame = -1;
			this.texturesToLoad = new List<TextureToLoad>();
		}

		private int currentFrame;
		private string currentTexture;
		private DateTime frameInitiated;
		private List<TextureToLoad> texturesToLoad;

		public string CurrentTexture {
			get {
				return currentTexture;
			}
			set {
				currentTexture = value;
				currentFrame = 0;
				frameInitiated = SceneManager.Now;
			}
		}

		public float ZIndex { get; set; }
		public Color Color { get; set; }
		public string CurrentAnimationClip { get; private set; }

		public Dictionary<string, Texture> Textures { get; private set; }
		public Dictionary<string, AnimationClip> AnimationClips { get; private set; }

		public void AddTexture (string key, Bitmap textureImage, int qtyFrames = 1, int duration = 1, Texture.Orientations orientation = Texture.Orientations.HORIZONTAL, int xFramesQty = 0) {
			this.Textures.Add(key, new Texture(gl, RegisterTexture(textureImage), qtyFrames, duration, orientation, xFramesQty));
		}

		public void AddTexture (string key, uint textureId, int qtyFrames = 1, int duration = 1, Texture.Orientations orientation = Texture.Orientations.HORIZONTAL, int xFramesQty = 0) {
			this.Textures.Add(key, new Texture(gl, textureId, qtyFrames, duration, orientation, xFramesQty));
		}

		public void LoadTexture (string textureKey, int timeout) {
			this.texturesToLoad.Add(new TextureToLoad(textureKey, timeout));
		}

		public void AddAnimationClip (string clipKey, AnimationClip animationClip) {
			this.AnimationClips.Add(clipKey, animationClip);
		}

		public void PlayAnimationClip (string clipKey) {
			this.CurrentAnimationClip = clipKey;
		}

		public void StopAnimationClip () {
			this.CurrentAnimationClip = "";
		}

		public void SkipAnimationClipPoint () {
			if (this.IsPlayingClip())
				this.AnimationClips[this.CurrentAnimationClip].SkipClipPoint();
		}

		public bool IsPlayingClip () {
			return !string.IsNullOrEmpty(this.CurrentAnimationClip) && this.AnimationClips.ContainsKey(this.CurrentAnimationClip);
		}

		public void OpenGLDraw (int glWidth, int glHeight, Transform transform) {
			if (!string.IsNullOrEmpty(this.CurrentAnimationClip) && this.AnimationClips.ContainsKey(this.CurrentAnimationClip)) {
				// Plays the current clip animation
				this.AnimationClips[this.CurrentAnimationClip].OpenGLDraw(this, transform);
			}

			if (!string.IsNullOrEmpty(this.CurrentTexture) && (this.Textures.ContainsKey(this.CurrentTexture) || this.CurrentTexture == SOLID_TEXTURE)) {
				Texture texture = null;
				if (this.CurrentTexture != SOLID_TEXTURE) {
					// Plays the current texture animation
					texture = this.Textures[this.CurrentTexture];
					if (this.currentFrame < 0 || this.currentFrame > texture.QtyFrames) {
						this.currentFrame = 0;
						this.frameInitiated = SceneManager.Now;
					} else if (SceneManager.Now.Subtract(this.frameInitiated).TotalMilliseconds >= texture.FrameDuration) {
						this.currentFrame++;
						if (this.currentFrame > texture.QtyFrames)
							this.currentFrame = 0;

						this.frameInitiated = SceneManager.Now;
					}

					gl.Enable(OpenGL.GL_TEXTURE_2D);
					gl.BindTexture(OpenGL.GL_TEXTURE_2D, texture.ID);
				}

				gl.PushMatrix();
					gl.Translate(transform.Position.X + (transform.Size.X / 2f) + (transform.Scale.X == -1 ? transform.Size.X : 0), transform.Position.Y + (transform.Size.Y / 2f) + (transform.Scale.Y == -1 ? transform.Size.Y : 0), 1);
					gl.Rotate(-transform.Rotation, 0, 0, 1);
					gl.Scale(transform.Scale.X, transform.Scale.Y, 1);

					gl.Color(this.Color.R / 255f, this.Color.G / 255f, this.Color.B / 255f, transform.GameObject.IsHidden ? 0 : this.Color.A / 255f);
					DrawTexture(transform.Size, texture, this.currentFrame);
					gl.Color(1f, 1f, 1f);
				gl.PopMatrix();

				if (this.CurrentTexture != SOLID_TEXTURE)
					gl.Disable(OpenGL.GL_TEXTURE_2D);
			}

			if (SceneManager.SHOW_COLLIDERS) {
				gl.LineWidth(3);
				gl.Begin(BeginMode.LineLoop);
					gl.Vertex(transform.PhysicalPosition.X, transform.PhysicalPosition.Y, 0f);
					gl.Vertex(transform.PhysicalPosition.X + transform.Size.X, transform.PhysicalPosition.Y, 0f);
					gl.Vertex(transform.PhysicalPosition.X + transform.Size.X, transform.PhysicalPosition.Y + transform.Size.Y, 0f);
					gl.Vertex(transform.PhysicalPosition.X, transform.PhysicalPosition.Y + transform.Size.Y, 0f);
				gl.End();
			}

			// Load textures that have been setted to be loaded after a timeout
			for (int l = 0; l < texturesToLoad.Count; l++) {
				if (SceneManager.Now.Subtract(texturesToLoad[l].Started).TotalMilliseconds >= texturesToLoad[l].Timeout) {
					this.CurrentTexture = texturesToLoad[l].TextureKey;
					texturesToLoad.Remove(texturesToLoad[l]);
				}
			}
		}
	}
}

﻿using SharpGL;
using SharpGL.Enumerations;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace Pescaria_CG_TP1.Engine {
	public class Animator {
		private struct TextureToLoad {
			public TextureToLoad (string textureKey, int timeout) {
				this.Timeout = timeout;
				this.TextureKey = textureKey;
				this.Start = DateTime.Now;
			}

			public int Timeout;
			public string TextureKey;
			public DateTime Start;
		}

		public Animator (OpenGL gl) {
			this.gl = gl;
			this.Textures = new Dictionary<string, Texture>();

			this.currentFrame = -1;
			this.texturesToLoad = new List<TextureToLoad>();
		}

		private int currentFrame;
		private string currentTexture;
		private OpenGL gl;
		private DateTime frameInitiated;
		private List<TextureToLoad> texturesToLoad;

		public string CurrentTexture {
			get {
				return currentTexture;
			}
			set {
				currentTexture = value;
				currentFrame = 0;
				frameInitiated = DateTime.Now;
			}
		}

		public Dictionary<string, Texture> Textures { get; private set; }

		public void AddTexture (string key, Bitmap textureImage, int qtyFrames = 1, int duration = 1, Texture.Orientations orientation = Texture.Orientations.HORIZONTAL) {
			uint[] textureID = new uint[1];

			// Inicializa a textura
			gl.Enable(OpenGL.GL_TEXTURE_2D);

			// Obtém a ID da textura e salva no vetor
			gl.GenTextures(1, textureID);

			gl.BindTexture(OpenGL.GL_TEXTURE_2D, textureID[0]);

			// Ativa transparências
			gl.Enable(OpenGL.GL_BLEND);
			gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);

			// Informa ao OpenGL onde estão os dados/pixels da imagem
			IntPtr pixels = textureImage.LockBits(new Rectangle(0, 0, textureImage.Width, textureImage.Height), ImageLockMode.ReadOnly, textureImage.PixelFormat).Scan0;
			gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, (int) OpenGL.GL_RGBA, textureImage.Width, textureImage.Height, 0, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, pixels);

			gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);
			gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);

			// Registra a textura
			this.Textures.Add(key, new Texture(gl, textureID[0], qtyFrames, duration, orientation));
		}

		public void LoadTexture (string textureKey, int timeout) {
			this.texturesToLoad.Add(new TextureToLoad(textureKey, timeout));
		}

		public void OpenGLDraw (int glWidth, int glHeight, Transform transform) {
			if (string.IsNullOrEmpty(this.CurrentTexture)) return;

			Texture texture = this.Textures[this.CurrentTexture];
			if (this.currentFrame < 0 || this.currentFrame > texture.QtyFrames) {
				this.currentFrame = 0;
				this.frameInitiated = DateTime.Now;
			} else if (DateTime.Now.Subtract(this.frameInitiated).TotalMilliseconds >= texture.FrameDuration) {
				this.currentFrame++;
				if (this.currentFrame > texture.QtyFrames)
					this.currentFrame = 0;

				this.frameInitiated = DateTime.Now;
			}

			gl.Enable(OpenGL.GL_TEXTURE_2D);
			gl.BindTexture(OpenGL.GL_TEXTURE_2D, texture.ID);
			gl.Begin(BeginMode.TriangleFan);
				texture.SetFrameCoordinates(this.currentFrame, Texture.CoordinatesPosition.TOP_LEFT);
				gl.Vertex(transform.Position.x, transform.Position.y, 0f);

				texture.SetFrameCoordinates(this.currentFrame, Texture.CoordinatesPosition.TOP_RIGHT);
				gl.Vertex(transform.Position.x + transform.Size.x, transform.Position.y, 0f);

				texture.SetFrameCoordinates(this.currentFrame, Texture.CoordinatesPosition.BOTTOM_RIGHT);
				gl.Vertex(transform.Position.x + transform.Size.x, transform.Position.y + transform.Size.y, 0f);

				texture.SetFrameCoordinates(this.currentFrame, Texture.CoordinatesPosition.BOTTOM_LEFT);
				gl.Vertex(transform.Position.x, transform.Position.y + transform.Size.y, 0f);
			gl.End();
			gl.Disable(OpenGL.GL_TEXTURE_2D);

			// Carrega as texturas que foram configuradas para serem carregadas após um tempo (timeout)
			for (int l = 0; l < texturesToLoad.Count; l++) {
				if (DateTime.Now.Subtract(texturesToLoad[l].Start).TotalMilliseconds >= texturesToLoad[l].Timeout) {
					this.CurrentTexture = texturesToLoad[l].TextureKey;
					texturesToLoad.Remove(texturesToLoad[l]);
				}
			}
		}
	}
}
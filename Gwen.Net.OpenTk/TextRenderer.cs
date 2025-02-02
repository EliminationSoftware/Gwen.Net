﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using Gwen.Net.OpenTk.Renderers;

namespace Gwen.Net.OpenTk
{
    public sealed class TextRenderer : IDisposable
    {
        private readonly Bitmap bitmap;
        private readonly Graphics graphics;
        private bool disposed;

        public TextRenderer(int width, int height, OpenTKRendererBase renderer)
        {
            if (width <= 0)
            {
                throw new ArgumentOutOfRangeException("width");
            }

            if (height <= 0)
            {
                throw new ArgumentOutOfRangeException("height");
            }

            bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            graphics = Graphics.FromImage(bitmap);
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            graphics.Clear(System.Drawing.Color.Transparent);
            Texture = new Texture(renderer) {Width = width, Height = height};
        }

        public Texture Texture { get; }

        public void Dispose()
        {
            Dispose(manual: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Draws the specified string to the backing store.
        /// </summary>
        /// <param name="text">The <see cref="System.String" /> to draw.</param>
        /// <param name="font">The <see cref="System.Drawing.Font" /> that will be used.</param>
        /// <param name="brush">The <see cref="System.Drawing.Brush" /> that will be used.</param>
        /// <param name="point">
        ///     The location of the text on the backing store, in 2d pixel coordinates.
        ///     The origin (0, 0) lies at the top-left corner of the backing store.
        /// </param>
        public void DrawString(string text, System.Drawing.Font font, Brush brush, Point point, StringFormat format)
        {
            graphics.DrawString(
                text,
                font,
                brush,
                new System.Drawing.Point(point.X, point.Y),
                format); // render text on the bitmap

            OpenTKRendererBase.LoadTextureInternal(Texture, bitmap); // copy bitmap to gl texture
        }

        private void Dispose(bool manual)
        {
            if (!disposed)
            {
                if (manual)
                {
                    bitmap.Dispose();
                    graphics.Dispose();
                    Texture.Dispose();
                }

                disposed = true;
            }
        }

#if DEBUG
        ~TextRenderer()
        {
            throw new InvalidOperationException(String.Format("[Warning] Resource leaked: {0}", typeof(TextRenderer)));
        }
#endif
    }
}
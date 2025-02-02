﻿using Gwen.Net.Renderer;

namespace Gwen.Net.Skin.Texturing
{
    public struct SubRect
    {
        public float[] uv;
    }

    /// <summary>
    ///     3x3 texture grid.
    /// </summary>
    public struct Bordered
    {
        private Texture m_Texture;

        private readonly SubRect[] m_Rects;

        private Margin m_Margin;

        private float m_Width;
        private float m_Height;

        public Bordered(Texture texture, float x, float y, float w, float h, Margin inMargin,
            float drawMarginScale = 1.0f)
            : this()
        {
            m_Rects = new SubRect[9];

            for (var i = 0; i < m_Rects.Length; i++)
            {
                m_Rects[i].uv = new float[4];
            }

            Init(texture, x, y, w, h, inMargin, drawMarginScale);
        }

        private void DrawRect(RendererBase render, int i, int x, int y, int w, int h)
        {
            render.DrawTexturedRect(
                m_Texture,
                new Rectangle(x, y, w, h),
                m_Rects[i].uv[0],
                m_Rects[i].uv[1],
                m_Rects[i].uv[2],
                m_Rects[i].uv[3]);
        }

        private void SetRect(int num, float x, float y, float w, float h)
        {
            float texw = m_Texture.Width;
            float texh = m_Texture.Height;

            //x -= 1.0f;
            //y -= 1.0f;

            m_Rects[num].uv[0] = x / texw;
            m_Rects[num].uv[1] = y / texh;

            m_Rects[num].uv[2] = (x + w) / texw;
            m_Rects[num].uv[3] = (y + h) / texh;

            //	rects[num].uv[0] += 1.0f / m_Texture->width;
            //	rects[num].uv[1] += 1.0f / m_Texture->width;
        }

        private void Init(Texture texture, float x, float y, float w, float h, Margin inMargin,
            float drawMarginScale = 1.0f)
        {
            m_Texture = texture;

            m_Margin = inMargin;

            SetRect(num: 0, x, y, m_Margin.Left, m_Margin.Top);
            SetRect(num: 1, x + m_Margin.Left, y, w - m_Margin.Left - m_Margin.Right, m_Margin.Top);
            SetRect(num: 2, x + w - m_Margin.Right, y, m_Margin.Right, m_Margin.Top);

            SetRect(num: 3, x, y + m_Margin.Top, m_Margin.Left, h - m_Margin.Top - m_Margin.Bottom);

            SetRect(
                num: 4,
                x + m_Margin.Left,
                y + m_Margin.Top,
                w - m_Margin.Left - m_Margin.Right,
                h - m_Margin.Top - m_Margin.Bottom);

            SetRect(
                num: 5,
                x + w - m_Margin.Right,
                y + m_Margin.Top,
                m_Margin.Right,
                h - m_Margin.Top - m_Margin.Bottom - 1);

            SetRect(num: 6, x, y + h - m_Margin.Bottom, m_Margin.Left, m_Margin.Bottom);

            SetRect(
                num: 7,
                x + m_Margin.Left,
                y + h - m_Margin.Bottom,
                w - m_Margin.Left - m_Margin.Right,
                m_Margin.Bottom);

            SetRect(num: 8, x + w - m_Margin.Right, y + h - m_Margin.Bottom, m_Margin.Right, m_Margin.Bottom);

            m_Margin.Left = (int) (m_Margin.Left * drawMarginScale);
            m_Margin.Right = (int) (m_Margin.Right * drawMarginScale);
            m_Margin.Top = (int) (m_Margin.Top * drawMarginScale);
            m_Margin.Bottom = (int) (m_Margin.Bottom * drawMarginScale);

            m_Width = w - x;
            m_Height = h - y;
        }

        // can't have this as default param
        public void Draw(RendererBase render, Rectangle r)
        {
            Draw(render, r, Color.White);
        }

        public void Draw(RendererBase render, Rectangle r, Color col)
        {
            if (m_Texture == null)
            {
                return;
            }

            render.DrawColor = col;

            if (r.Width < m_Width && r.Height < m_Height)
            {
                render.DrawTexturedRect(
                    m_Texture,
                    r,
                    m_Rects[0].uv[0],
                    m_Rects[0].uv[1],
                    m_Rects[8].uv[2],
                    m_Rects[8].uv[3]);

                return;
            }

            DrawRect(render, i: 0, r.X, r.Y, m_Margin.Left, m_Margin.Top);
            DrawRect(render, i: 1, r.X + m_Margin.Left, r.Y, r.Width - m_Margin.Left - m_Margin.Right, m_Margin.Top);
            DrawRect(render, i: 2, r.X + r.Width - m_Margin.Right, r.Y, m_Margin.Right, m_Margin.Top);

            DrawRect(render, i: 3, r.X, r.Y + m_Margin.Top, m_Margin.Left, r.Height - m_Margin.Top - m_Margin.Bottom);

            DrawRect(
                render,
                i: 4,
                r.X + m_Margin.Left,
                r.Y + m_Margin.Top,
                r.Width - m_Margin.Left - m_Margin.Right,
                r.Height - m_Margin.Top - m_Margin.Bottom);

            DrawRect(
                render,
                i: 5,
                r.X + r.Width - m_Margin.Right,
                r.Y + m_Margin.Top,
                m_Margin.Right,
                r.Height - m_Margin.Top - m_Margin.Bottom);

            DrawRect(render, i: 6, r.X, r.Y + r.Height - m_Margin.Bottom, m_Margin.Left, m_Margin.Bottom);

            DrawRect(
                render,
                i: 7,
                r.X + m_Margin.Left,
                r.Y + r.Height - m_Margin.Bottom,
                r.Width - m_Margin.Left - m_Margin.Right,
                m_Margin.Bottom);

            DrawRect(
                render,
                i: 8,
                r.X + r.Width - m_Margin.Right,
                r.Y + r.Height - m_Margin.Bottom,
                m_Margin.Right,
                m_Margin.Bottom);
        }
    }
}

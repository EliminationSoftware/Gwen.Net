﻿using System;

namespace Gwen.Net.Control.Internal
{
    public class ScrollArea : InnerContentControl
    {
        private bool m_CanScrollH;
        private bool m_CanScrollV;

        public ScrollArea(ControlBase parent)
            : base(parent)
        {
            m_CanScrollV = true;
            m_CanScrollH = true;
        }

        public Size ViewableContentSize { get; private set; }

        public Size ContentSize => new(m_InnerPanel.ActualWidth, m_InnerPanel.ActualHeight);

        public Point ScrollPosition
        {
            get => m_InnerPanel.ActualPosition;
            set => SetScrollPosition(value.X, value.Y);
        }

        public int VerticalScroll
        {
            get => m_InnerPanel.ActualTop;
            set => m_InnerPanel.SetPosition(Content.ActualLeft, value);
        }

        public int HorizontalScroll
        {
            get => m_InnerPanel.ActualLeft;
            set => m_InnerPanel.SetPosition(value, m_InnerPanel.ActualTop);
        }

        public virtual void EnableScroll(bool horizontal, bool vertical)
        {
            m_CanScrollV = vertical;
            m_CanScrollH = horizontal;
        }

        public void SetScrollPosition(int horizontal, int vertical)
        {
            m_InnerPanel.SetPosition(horizontal, vertical);
        }

        protected override Size Measure(Size availableSize)
        {
            if (m_InnerPanel == null)
            {
                return Size.Zero;
            }

            Size size = m_InnerPanel.DoMeasure(
                new Size(
                    m_CanScrollH ? Util.Infinity : availableSize.Width,
                    m_CanScrollV ? Util.Infinity : availableSize.Height));

            // Let the parent determine the size if scrolling is enabled
            size.Width = m_CanScrollH ? 0 : Math.Min(size.Width, availableSize.Width);
            size.Height = m_CanScrollV ? 0 : Math.Min(size.Height, availableSize.Height);

            return size;
        }

        protected override Size Arrange(Size finalSize)
        {
            if (m_InnerPanel == null)
            {
                return finalSize;
            }

            int scrollAreaWidth = Math.Max(finalSize.Width, m_InnerPanel.MeasuredSize.Width);
            int scrollAreaHeight = Math.Max(finalSize.Height, m_InnerPanel.MeasuredSize.Height);

            m_InnerPanel.DoArrange(new Rectangle(x: 0, y: 0, scrollAreaWidth, scrollAreaHeight));

            ViewableContentSize = new Size(finalSize.Width, finalSize.Height);

            return finalSize;
        }
    }
}
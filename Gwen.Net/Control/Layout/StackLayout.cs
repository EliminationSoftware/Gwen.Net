﻿using Gwen.Net.Xml;

namespace Gwen.Net.Control.Layout
{
    /// <summary>
    ///     Arrange child controls into a row or a column.
    /// </summary>
    [XmlControl]
    public class StackLayout : ControlBase
    {
        private bool m_Horizontal;

        public StackLayout(ControlBase parent)
            : base(parent) {}

        /// <summary>
        ///     If set, arrange child controls into a row instead of a column.
        /// </summary>
        [XmlProperty] public bool Horizontal
        {
            get => m_Horizontal;
            set
            {
                if (m_Horizontal == value)
                {
                    return;
                }

                m_Horizontal = value;
                Invalidate();
            }
        }

        protected override Size Measure(Size availableSize)
        {
            availableSize -= Padding;

            var width = 0;
            var height = 0;

            if (m_Horizontal)
            {
                foreach (ControlBase child in Children)
                {
                    if (child.IsCollapsed)
                    {
                        continue;
                    }

                    Size size = child.DoMeasure(availableSize);
                    availableSize.Width -= size.Width;

                    if (size.Height > height)
                    {
                        height = size.Height;
                    }

                    width += size.Width;
                }
            }
            else
            {
                foreach (ControlBase child in Children)
                {
                    if (child.IsCollapsed)
                    {
                        continue;
                    }

                    Size size = child.DoMeasure(availableSize);
                    availableSize.Height -= size.Height;

                    if (size.Width > width)
                    {
                        width = size.Width;
                    }

                    height += size.Height;
                }
            }

            return new Size(width, height) + Padding;
        }

        protected override Size Arrange(Size finalSize)
        {
            finalSize -= Padding;

            if (m_Horizontal)
            {
                int height = finalSize.Height;
                int x = Padding.Left;

                foreach (ControlBase child in Children)
                {
                    if (child.IsCollapsed)
                    {
                        continue;
                    }

                    child.DoArrange(new Rectangle(x, Padding.Top, child.MeasuredSize.Width, height));
                    x += child.MeasuredSize.Width;
                }

                x += Padding.Right;

                return new Size(x, finalSize.Height + Padding.Top + Padding.Bottom);
            }

            int width = finalSize.Width;
            int y = Padding.Top;

            foreach (ControlBase child in Children)
            {
                if (child.IsCollapsed)
                {
                    continue;
                }

                child.DoArrange(new Rectangle(Padding.Left, y, width, child.MeasuredSize.Height));
                y += child.MeasuredSize.Height;
            }

            y += Padding.Bottom;

            return new Size(finalSize.Width + Padding.Left + Padding.Right, y);
        }
    }
}

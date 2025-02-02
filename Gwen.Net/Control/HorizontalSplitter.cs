﻿using System;
using Gwen.Net.Control.Internal;
using Gwen.Net.Xml;

namespace Gwen.Net.Control
{
    [XmlControl]
    public class HorizontalSplitter : ControlBase
    {
        private readonly ControlBase[] m_Sections;
        private readonly SplitterBar m_VSplitter;

        private float m_VVal; // 0-1
        private int m_ZoomedSection; // 0-1

        /// <summary>
        ///     Initializes a new instance of the <see cref="CrossSplitter" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public HorizontalSplitter(ControlBase parent)
            : base(parent)
        {
            m_Sections = new ControlBase[2];

            m_VSplitter = new SplitterBar(this);
            m_VSplitter.Dragged += OnVerticalMoved;
            m_VSplitter.Cursor = Cursor.SizeNS;

            m_VVal = 0.5f;

            SetPanel(index: 0, panel: null);
            SetPanel(index: 1, panel: null);

            SplitterSize = 5;
            SplittersVisible = false;

            m_ZoomedSection = -1;
        }

        /// <summary>
        ///     Splitter position (0 - 1)
        /// </summary>
        [XmlProperty] public float Value
        {
            get => m_VVal;
            set => SetVValue(value);
        }

        /// <summary>
        ///     Indicates whether any of the panels is zoomed.
        /// </summary>
        public bool IsZoomed => m_ZoomedSection != -1;

        /// <summary>
        ///     Gets or sets a value indicating whether splitters should be visible.
        /// </summary>
        [XmlProperty] public bool SplittersVisible
        {
            get => m_VSplitter.ShouldDrawBackground;
            set => m_VSplitter.ShouldDrawBackground = value;
        }

        /// <summary>
        ///     Gets or sets the size of the splitter.
        /// </summary>
        [XmlProperty] public int SplitterSize { get; set; }

        /// <summary>
        ///     Invoked when one of the panels has been zoomed (maximized).
        /// </summary>
        [XmlEvent] public event GwenEventHandler<EventArgs> PanelZoomed;

        /// <summary>
        ///     Invoked when one of the panels has been unzoomed (restored).
        /// </summary>
        [XmlEvent] public event GwenEventHandler<EventArgs> PanelUnZoomed;

        /// <summary>
        ///     Invoked when the zoomed panel has been changed.
        /// </summary>
        [XmlEvent] public event GwenEventHandler<EventArgs> ZoomChanged;

        /// <summary>
        ///     Centers the panels so that they take even amount of space.
        /// </summary>
        public void CenterPanels()
        {
            m_VVal = 0.5f;
            Invalidate();
        }

        public void SetVValue(float value)
        {
            if (value <= 1f || value >= 0)
            {
                m_VVal = value;
            }

            Invalidate();
        }

        protected void OnVerticalMoved(ControlBase control, EventArgs args)
        {
            m_VVal = CalculateValueVertical();
            Invalidate();
        }

        private float CalculateValueVertical()
        {
            return m_VSplitter.ActualTop / (float) (ActualHeight - m_VSplitter.ActualHeight);
        }

        protected override Size Measure(Size availableSize)
        {
            Size size = Size.Zero;

            m_VSplitter.DoMeasure(new Size(availableSize.Width, SplitterSize));
            size.Height += m_VSplitter.Height;

            var v = (int) ((availableSize.Height - SplitterSize) * m_VVal);

            if (m_ZoomedSection == -1)
            {
                if (m_Sections[0] != null)
                {
                    m_Sections[0].DoMeasure(new Size(availableSize.Width, v));
                    size.Height += m_Sections[0].MeasuredSize.Height;
                    size.Width = Math.Max(size.Width, m_Sections[0].MeasuredSize.Width);
                }

                if (m_Sections[1] != null)
                {
                    m_Sections[1].DoMeasure(new Size(availableSize.Width, availableSize.Height - SplitterSize - v));
                    size.Height += m_Sections[1].MeasuredSize.Height;
                    size.Width = Math.Max(size.Width, m_Sections[1].MeasuredSize.Width);
                }
            }
            else
            {
                m_Sections[m_ZoomedSection].DoMeasure(availableSize);
                size = m_Sections[m_ZoomedSection].MeasuredSize;
            }

            return size;
        }

        protected override Size Arrange(Size finalSize)
        {
            var v = (int) ((finalSize.Height - SplitterSize) * m_VVal);

            m_VSplitter.DoArrange(
                new Rectangle(x: 0, v, m_VSplitter.MeasuredSize.Width, m_VSplitter.MeasuredSize.Height));

            if (m_ZoomedSection == -1)
            {
                if (m_Sections[0] != null)
                {
                    m_Sections[0].DoArrange(new Rectangle(x: 0, y: 0, finalSize.Width, v));
                }

                if (m_Sections[1] != null)
                {
                    m_Sections[1].DoArrange(
                        new Rectangle(x: 0, v + SplitterSize, finalSize.Width, finalSize.Height - SplitterSize - v));
                }
            }
            else
            {
                m_Sections[m_ZoomedSection].DoArrange(new Rectangle(x: 0, y: 0, finalSize.Width, finalSize.Height));
            }

            return finalSize;
        }

        /// <summary>
        ///     Assigns a control to the specific inner section.
        /// </summary>
        /// <param name="index">Section index (0-3).</param>
        /// <param name="panel">Control to assign.</param>
        public void SetPanel(int index, ControlBase panel)
        {
            m_Sections[index] = panel;

            if (panel != null)
            {
                panel.Parent = this;
            }

            Invalidate();
        }

        /// <summary>
        ///     Gets the specific inner section.
        /// </summary>
        /// <param name="index">Section index (0-3).</param>
        /// <returns>Specified section.</returns>
        public ControlBase GetPanel(int index)
        {
            return m_Sections[index];
        }

        protected override void OnChildAdded(ControlBase child)
        {
            if (!(child is SplitterBar))
            {
                if (m_Sections[0] == null)
                {
                    SetPanel(index: 0, child);
                }
                else if (m_Sections[1] == null)
                {
                    SetPanel(index: 1, child);
                }
                else
                {
                    throw new Exception("Too many panels added.");
                }
            }

            base.OnChildAdded(child);
        }

        /// <summary>
        ///     Internal handler for the zoom changed event.
        /// </summary>
        protected void OnZoomChanged()
        {
            if (ZoomChanged != null)
            {
                ZoomChanged.Invoke(this, EventArgs.Empty);
            }

            if (m_ZoomedSection == -1)
            {
                if (PanelUnZoomed != null)
                {
                    PanelUnZoomed.Invoke(this, EventArgs.Empty);
                }
            }
            else
            {
                if (PanelZoomed != null)
                {
                    PanelZoomed.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        ///     Maximizes the specified panel so it fills the entire control.
        /// </summary>
        /// <param name="section">Panel index (0-3).</param>
        public void Zoom(int section)
        {
            UnZoom();

            if (m_Sections[section] != null)
            {
                for (var i = 0; i < 2; i++)
                {
                    if (i != section && m_Sections[i] != null)
                    {
                        m_Sections[i].IsHidden = true;
                    }
                }

                m_ZoomedSection = section;

                Invalidate();
            }

            OnZoomChanged();
        }

        /// <summary>
        ///     Restores the control so all panels are visible.
        /// </summary>
        public void UnZoom()
        {
            m_ZoomedSection = -1;

            for (var i = 0; i < 2; i++)
            {
                if (m_Sections[i] != null)
                {
                    m_Sections[i].IsHidden = false;
                }
            }

            Invalidate();
            OnZoomChanged();
        }
    }
}

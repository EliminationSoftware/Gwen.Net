﻿using System;
using Gwen.Net.Control.Internal;
using Gwen.Net.DragDrop;
using Gwen.Net.Renderer;
using Gwen.Net.Skin;

namespace Gwen.Net.Control
{
    /// <summary>
    ///     Base for dockable containers.
    /// </summary>
    public class DockBase : ControlBase
    {
        private DockBase m_Bottom;

        // Only CHILD dockpanels have a tabcontrol.
        private DockedTabControl m_DockedTabControl;

        private bool m_DrawHover;
        private bool m_DropFar;
        private Rectangle m_HoverRect;
        private DockBase m_Left;
        private DockBase m_Right;
        private Resizer m_Sizer;
        private DockBase m_Top;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DockBase" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public DockBase(ControlBase parent)
            : base(parent)
        {
            Padding = Padding.One;
            MinimumSize = new Size(width: 30, height: 30);
            MouseInputEnabled = true;
        }

        // todo: dock events?

        /// <summary>
        ///     Control docked on the left side.
        /// </summary>
        public DockBase LeftDock => GetChildDock(Dock.Left);

        /// <summary>
        ///     Control docked on the right side.
        /// </summary>
        public DockBase RightDock => GetChildDock(Dock.Right);

        /// <summary>
        ///     Control docked on the top side.
        /// </summary>
        public DockBase TopDock => GetChildDock(Dock.Top);

        /// <summary>
        ///     Control docked on the bottom side.
        /// </summary>
        public DockBase BottomDock => GetChildDock(Dock.Bottom);

        public TabControl TabControl => m_DockedTabControl;

        /// <summary>
        ///     Indicates whether the control contains any docked children.
        /// </summary>
        public virtual bool IsEmpty
        {
            get
            {
                if (m_DockedTabControl != null && m_DockedTabControl.TabCount > 0)
                {
                    return false;
                }

                if (m_Left != null && !m_Left.IsEmpty)
                {
                    return false;
                }

                if (m_Right != null && !m_Right.IsEmpty)
                {
                    return false;
                }

                if (m_Top != null && !m_Top.IsEmpty)
                {
                    return false;
                }

                if (m_Bottom != null && !m_Bottom.IsEmpty)
                {
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        ///     Handler for Space keyboard event.
        /// </summary>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>
        ///     True if handled.
        /// </returns>
        protected override bool OnKeySpace(bool down)
        {
            // No action on space (default button action is to press)
            return false;
        }

        /// <summary>
        ///     Initializes an inner docked control for the specified position.
        /// </summary>
        /// <param name="pos">Dock position.</param>
        protected virtual void SetupChildDock(Dock pos)
        {
            if (m_DockedTabControl == null)
            {
                m_DockedTabControl = new DockedTabControl(this);
                m_DockedTabControl.TabRemoved += OnTabRemoved;
                m_DockedTabControl.TabStripPosition = Dock.Bottom;
                m_DockedTabControl.TitleBarVisible = true;
            }

            Dock = pos;

            Dock sizeDir;

            if (pos == Dock.Right)
            {
                sizeDir = Dock.Left;
            }
            else if (pos == Dock.Left)
            {
                sizeDir = Dock.Right;
            }
            else if (pos == Dock.Top)
            {
                sizeDir = Dock.Bottom;
            }
            else if (pos == Dock.Bottom)
            {
                sizeDir = Dock.Top;
            }
            else
            {
                throw new ArgumentException("Invalid dock", "pos");
            }

            if (m_Sizer != null)
            {
                m_Sizer.Dispose();
            }

            m_Sizer = new Resizer(this);
            m_Sizer.Dock = sizeDir;
            m_Sizer.ResizeDir = sizeDir;

            if (sizeDir == Dock.Left || sizeDir == Dock.Right)
            {
                m_Sizer.Width = 2;
            }
            else
            {
                m_Sizer.Height = 2;
            }
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Render(SkinBase skin) {}

        /// <summary>
        ///     Gets an inner docked control for the specified position.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        protected virtual DockBase GetChildDock(Dock pos)
        {
            // todo: verify
            DockBase dock = null;

            switch (pos)
            {
                case Dock.Left:
                    if (m_Left == null)
                    {
                        m_Left = new DockBase(this);
                        m_Left.Width = 200;
                        m_Left.SetupChildDock(pos);
                    }

                    dock = m_Left;

                    break;

                case Dock.Right:
                    if (m_Right == null)
                    {
                        m_Right = new DockBase(this);
                        m_Right.Width = 200;
                        m_Right.SetupChildDock(pos);
                    }

                    dock = m_Right;

                    break;

                case Dock.Top:
                    if (m_Top == null)
                    {
                        m_Top = new DockBase(this);
                        m_Top.Height = 200;
                        m_Top.SetupChildDock(pos);
                    }

                    dock = m_Top;

                    break;

                case Dock.Bottom:
                    if (m_Bottom == null)
                    {
                        m_Bottom = new DockBase(this);
                        m_Bottom.Height = 200;
                        m_Bottom.SetupChildDock(pos);
                    }

                    dock = m_Bottom;

                    break;
            }

            if (dock != null)
            {
                dock.IsCollapsed = false;
            }

            return dock;
        }

        /// <summary>
        ///     Calculates dock direction from dragdrop coordinates.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <returns>Dock direction.</returns>
        protected virtual Dock GetDroppedTabDirection(int x, int y)
        {
            int w = ActualWidth;
            int h = ActualHeight;
            float top = y / (float) h;
            float left = x / (float) w;
            float right = (w - x) / (float) w;
            float bottom = (h - y) / (float) h;
            float minimum = Math.Min(Math.Min(Math.Min(top, left), right), bottom);

            m_DropFar = minimum < 0.2f;

            if (minimum > 0.3f)
            {
                return Dock.Fill;
            }

            if (top == minimum && (null == m_Top || m_Top.IsCollapsed))
            {
                return Dock.Top;
            }

            if (left == minimum && (null == m_Left || m_Left.IsCollapsed))
            {
                return Dock.Left;
            }

            if (right == minimum && (null == m_Right || m_Right.IsCollapsed))
            {
                return Dock.Right;
            }

            if (bottom == minimum && (null == m_Bottom || m_Bottom.IsCollapsed))
            {
                return Dock.Bottom;
            }

            return Dock.Fill;
        }

        public override bool DragAndDrop_CanAcceptPackage(Package p)
        {
            // A TAB button dropped 
            if (p.Name == "TabButtonMove")
            {
                return true;
            }

            // a TAB window dropped
            if (p.Name == "TabWindowMove")
            {
                return true;
            }

            return false;
        }

        public override bool DragAndDrop_HandleDrop(Package p, int x, int y)
        {
            Point pos = CanvasPosToLocal(new Point(x, y));
            Dock dir = GetDroppedTabDirection(pos.X, pos.Y);

            Invalidate();

            DockedTabControl addTo = m_DockedTabControl;

            if (dir == Dock.Fill && addTo == null)
            {
                return false;
            }

            if (dir != Dock.Fill)
            {
                DockBase dock = GetChildDock(dir);
                addTo = dock.m_DockedTabControl;

                if (!m_DropFar)
                {
                    dock.BringToFront();
                }
                else
                {
                    dock.SendToBack();
                }
            }

            if (p.Name == "TabButtonMove")
            {
                var tabButton = DragAndDrop.SourceControl as TabButton;

                if (null == tabButton)
                {
                    return false;
                }

                addTo.AddPage(tabButton);
            }

            if (p.Name == "TabWindowMove")
            {
                var tabControl = DragAndDrop.SourceControl as DockedTabControl;

                if (null == tabControl)
                {
                    return false;
                }

                if (tabControl == addTo)
                {
                    return false;
                }

                tabControl.MoveTabsTo(addTo);
            }

            return true;
        }

        protected virtual void OnTabRemoved(ControlBase control, EventArgs args)
        {
            DoRedundancyCheck();
            DoConsolidateCheck();
        }

        protected virtual void DoRedundancyCheck()
        {
            if (!IsEmpty)
            {
                return;
            }

            var pDockParent = Parent as DockBase;

            if (null == pDockParent)
            {
                return;
            }

            pDockParent.OnRedundantChildDock(this);
        }

        protected virtual void DoConsolidateCheck()
        {
            if (IsEmpty)
            {
                return;
            }

            if (null == m_DockedTabControl)
            {
                return;
            }

            if (m_DockedTabControl.TabCount > 0)
            {
                return;
            }

            if (m_Bottom != null && !m_Bottom.IsEmpty)
            {
                m_Bottom.m_DockedTabControl.MoveTabsTo(m_DockedTabControl);

                return;
            }

            if (m_Top != null && !m_Top.IsEmpty)
            {
                m_Top.m_DockedTabControl.MoveTabsTo(m_DockedTabControl);

                return;
            }

            if (m_Left != null && !m_Left.IsEmpty)
            {
                m_Left.m_DockedTabControl.MoveTabsTo(m_DockedTabControl);

                return;
            }

            if (m_Right != null && !m_Right.IsEmpty)
            {
                m_Right.m_DockedTabControl.MoveTabsTo(m_DockedTabControl);
            }
        }

        protected virtual void OnRedundantChildDock(DockBase dock)
        {
            dock.IsCollapsed = true;
            DoRedundancyCheck();
            DoConsolidateCheck();
        }

        public override void DragAndDrop_HoverEnter(Package p, int x, int y)
        {
            m_DrawHover = true;
        }

        public override void DragAndDrop_HoverLeave(Package p)
        {
            m_DrawHover = false;
        }

        public override void DragAndDrop_Hover(Package p, int x, int y)
        {
            Point pos = CanvasPosToLocal(new Point(x, y));
            Dock dir = GetDroppedTabDirection(pos.X, pos.Y);

            if (dir == Dock.Fill)
            {
                if (null == m_DockedTabControl)
                {
                    m_HoverRect = Rectangle.Empty;

                    return;
                }

                m_HoverRect = InnerBounds;

                return;
            }

            m_HoverRect = RenderBounds;

            int helpBarWidth;

            if (dir == Dock.Left)
            {
                helpBarWidth = (int) (m_HoverRect.Width * 0.25f);
                m_HoverRect.Width = helpBarWidth;
            }

            if (dir == Dock.Right)
            {
                helpBarWidth = (int) (m_HoverRect.Width * 0.25f);
                m_HoverRect.X = m_HoverRect.Width - helpBarWidth;
                m_HoverRect.Width = helpBarWidth;
            }

            if (dir == Dock.Top)
            {
                helpBarWidth = (int) (m_HoverRect.Height * 0.25f);
                m_HoverRect.Height = helpBarWidth;
            }

            if (dir == Dock.Bottom)
            {
                helpBarWidth = (int) (m_HoverRect.Height * 0.25f);
                m_HoverRect.Y = m_HoverRect.Height - helpBarWidth;
                m_HoverRect.Height = helpBarWidth;
            }

            if ((dir == Dock.Top || dir == Dock.Bottom) && !m_DropFar)
            {
                if (m_Left != null && !m_Left.IsCollapsed)
                {
                    m_HoverRect.X += m_Left.ActualWidth;
                    m_HoverRect.Width -= m_Left.ActualWidth;
                }

                if (m_Right != null && !m_Right.IsCollapsed)
                {
                    m_HoverRect.Width -= m_Right.ActualWidth;
                }
            }

            if ((dir == Dock.Left || dir == Dock.Right) && !m_DropFar)
            {
                if (m_Top != null && !m_Top.IsCollapsed)
                {
                    m_HoverRect.Y += m_Top.ActualHeight;
                    m_HoverRect.Height -= m_Top.ActualHeight;
                }

                if (m_Bottom != null && !m_Bottom.IsCollapsed)
                {
                    m_HoverRect.Height -= m_Bottom.ActualHeight;
                }
            }
        }

        /// <summary>
        ///     Renders over the actual control (overlays).
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void RenderOver(SkinBase skin)
        {
            if (!m_DrawHover)
            {
                return;
            }

            RendererBase render = skin.Renderer;
            render.DrawColor = new Color(a: 20, r: 255, g: 200, b: 255);
            render.DrawFilledRect(RenderBounds);

            if (m_HoverRect.Width == 0)
            {
                return;
            }

            render.DrawColor = new Color(a: 100, r: 255, g: 200, b: 255);
            render.DrawFilledRect(m_HoverRect);

            render.DrawColor = new Color(a: 200, r: 255, g: 200, b: 255);
            render.DrawLinedRect(m_HoverRect);
        }
    }
}

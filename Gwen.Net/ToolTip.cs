﻿using Gwen.Net.Control;
using Gwen.Net.Input;
using Gwen.Net.Renderer;
using Gwen.Net.Skin;

namespace Gwen.Net
{
    /// <summary>
    ///     Tooltip handling.
    /// </summary>
    public static class ToolTip
    {
        private static ControlBase g_ToolTip;

        /// <summary>
        ///     Enables tooltip display for the specified control.
        /// </summary>
        /// <param name="control">Target control.</param>
        public static void Enable(ControlBase control)
        {
            if (null == control.ToolTip)
            {
                return;
            }

            ControlBase toolTip = control.ToolTip;
            g_ToolTip = control;
            toolTip.DoMeasure(Size.Infinity);
            toolTip.DoArrange(new Rectangle(Point.Zero, toolTip.MeasuredSize));
        }

        /// <summary>
        ///     Disables tooltip display for the specified control.
        /// </summary>
        /// <param name="control">Target control.</param>
        public static void Disable(ControlBase control)
        {
            if (g_ToolTip == control)
            {
                g_ToolTip = null;
            }
        }

        /// <summary>
        ///     Disables tooltip display for the specified control.
        /// </summary>
        /// <param name="control">Target control.</param>
        public static void ControlDeleted(ControlBase control)
        {
            Disable(control);
        }

        /// <summary>
        ///     Renders the currently visible tooltip.
        /// </summary>
        /// <param name="skin"></param>
        public static void RenderToolTip(SkinBase skin)
        {
            if (null == g_ToolTip)
            {
                return;
            }

            RendererBase render = skin.Renderer;

            Point oldRenderOffset = render.RenderOffset;
            Point mousePos = InputHandler.MousePosition;
            Rectangle bounds = g_ToolTip.ToolTip.Bounds;

            Rectangle offset = Util.FloatRect(
                mousePos.X - (bounds.Width / 2),
                mousePos.Y - bounds.Height - 10,
                bounds.Width,
                bounds.Height);

            offset = Util.ClampRectToRect(offset, g_ToolTip.GetCanvas().Bounds);

            //Calculate offset on screen bounds
            render.AddRenderOffset(offset);
            render.EndClip();

            skin.DrawToolTip(g_ToolTip.ToolTip);
            g_ToolTip.ToolTip.DoRender(skin);

            render.RenderOffset = oldRenderOffset;
        }
    }
}
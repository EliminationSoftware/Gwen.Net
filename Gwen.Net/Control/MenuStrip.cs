﻿using System;
using Gwen.Net.Skin;
using Gwen.Net.Xml;

namespace Gwen.Net.Control
{
    /// <summary>
    ///     Menu strip.
    /// </summary>
    [XmlControl(CustomHandler = "XmlElementHandler")]
    public class MenuStrip : Menu
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MenuStrip" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public MenuStrip(ControlBase parent)
            : base(parent)
        {
            Collapse(collapsed: false, measure: false);

            Padding = new Padding(left: 5, top: 0, right: 0, bottom: 0);
            IconMarginDisabled = true;
            EnableScroll(horizontal: true, vertical: false);

            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Top;

            m_Layout.Horizontal = true;
            m_Layout.HorizontalAlignment = HorizontalAlignment.Left;
            m_Layout.VerticalAlignment = VerticalAlignment.Stretch;
        }

        /// <summary>
        ///     Determines whether the menu should open on mouse hover.
        /// </summary>
        protected override bool ShouldHoverOpenMenu => IsMenuOpen();

        /// <summary>
        ///     Closes the current menu.
        /// </summary>
        public override void Close() {}

        /// <summary>
        ///     Renders under the actual control (shadows etc).
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void RenderUnder(SkinBase skin) {}

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Render(SkinBase skin)
        {
            skin.DrawMenuStrip(this);
        }

        /// <summary>
        ///     Add item handler.
        /// </summary>
        /// <param name="item">Item added.</param>
        protected override void OnAddItem(MenuItem item)
        {
            item.TextPadding = new Padding(left: 5, top: 0, right: 5, bottom: 0);
            item.Padding = new Padding(left: 4, top: 4, right: 4, bottom: 4);
            item.HoverEnter += OnHoverItem;
        }

        internal static ControlBase XmlElementHandler(Parser parser, Type type, ControlBase parent)
        {
            MenuStrip element = new(parent);
            parser.ParseAttributes(element);

            if (parser.MoveToContent())
            {
                foreach (string elementName in parser.NextElement())
                {
                    if (elementName == "MenuItem")
                    {
                        element.AddItem(parser.ParseElement<MenuItem>(element));
                    }
                }
            }

            return element;
        }
    }
}
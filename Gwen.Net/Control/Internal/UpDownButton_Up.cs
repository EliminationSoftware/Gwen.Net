﻿using Gwen.Net.Skin;

namespace Gwen.Net.Control.Internal
{
    /// <summary>
    ///     Numeric up arrow.
    /// </summary>
    public class UpDownButton_Up : ButtonBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="UpDownButton_Up" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public UpDownButton_Up(ControlBase parent)
            : base(parent)
        {
            Width = BaseUnit / 2;
        }

        protected override void AdaptToScaleChange()
        {
            Width = BaseUnit / 2;
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Render(SkinBase skin)
        {
            skin.DrawNumericUpDownButton(this, IsDepressed, up: true);
        }
    }
}
﻿using Gwen.Net.Skin;

namespace Gwen.Net.Control.Internal
{
    /// <summary>
    ///     Numeric down arrow.
    /// </summary>
    internal class UpDownButton_Down : ButtonBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="UpDownButton_Down" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public UpDownButton_Down(ControlBase parent)
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
            skin.DrawNumericUpDownButton(this, IsDepressed, up: false);
        }
    }
}
﻿using Gwen.Net.Skin;

namespace Gwen.Net.Control.Internal
{
    /// <summary>
    ///     ComboBox arrow.
    /// </summary>
    public class DownArrow : ControlBase
    {
        private readonly ComboBox m_ComboBox;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DownArrow" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public DownArrow(ComboBox parent)
            : base(parent)
        {
            MouseInputEnabled = false;

            Size = new Size(BaseUnit);

            m_ComboBox = parent;
        }

        protected override void AdaptToScaleChange()
        {
            Size = new Size(BaseUnit);
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Render(SkinBase skin)
        {
            skin.DrawComboBoxArrow(
                this,
                m_ComboBox.IsHovered,
                m_ComboBox.IsDepressed,
                m_ComboBox.IsOpen,
                m_ComboBox.IsDisabled);
        }
    }
}
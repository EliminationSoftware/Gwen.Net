﻿using Gwen.Net.Skin;

namespace Gwen.Net.Control.Internal
{
    /// <summary>
    ///     Color square.
    /// </summary>
    public class ColorDisplay : ControlBase
    {
        private Color m_Color;
        //private bool m_DrawCheckers;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ColorDisplay" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public ColorDisplay(ControlBase parent) : base(parent)
        {
            Size = new Size(BaseUnit * 2);
            m_Color = new Color(a: 255, r: 255, g: 0, b: 0);
            //m_DrawCheckers = true;
        }

        /// <summary>
        ///     Current color.
        /// </summary>
        public Color Color
        {
            get => m_Color;
            set => m_Color = value;
        }

        //public bool DrawCheckers { get { return m_DrawCheckers; } set { m_DrawCheckers = value; } }
        public int R
        {
            get => m_Color.R;
            set => m_Color = new Color(m_Color.A, value, m_Color.G, m_Color.B);
        }

        public int G
        {
            get => m_Color.G;
            set => m_Color = new Color(m_Color.A, m_Color.R, value, m_Color.B);
        }

        public int B
        {
            get => m_Color.B;
            set => m_Color = new Color(m_Color.A, m_Color.R, m_Color.G, value);
        }

        public int A
        {
            get => m_Color.A;
            set => m_Color = new Color(value, m_Color.R, m_Color.G, m_Color.B);
        }

        protected override void AdaptToScaleChange()
        {
            int baseSize = BaseUnit * 2;

            int width = Util.IsIgnore(Size.Width) ? Util.Ignore : baseSize;
            int height = Util.IsIgnore(Size.Height) ? Util.Ignore : baseSize;

            Size = new Size(width, height);
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Render(SkinBase skin)
        {
            skin.DrawColorDisplay(this, m_Color);
        }
    }
}
﻿using System;
using Gwen.Net.Control.Internal;
using Gwen.Net.Skin;
using Gwen.Net.Xml;

namespace Gwen.Net.Control
{
    /// <summary>
    ///     CheckBox control.
    /// </summary>
    [XmlControl]
    public class CheckBox : ButtonBase
    {
        private bool m_Checked;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CheckBox" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public CheckBox(ControlBase parent)
            : base(parent)
        {
            Size = new Size(BaseUnit);
            IsToggle = true;
        }

        /// <summary>
        ///     Indicates whether the checkbox is checked.
        /// </summary>
        [XmlProperty] public bool IsChecked
        {
            get => m_Checked;
            set
            {
                if (m_Checked == value)
                {
                    return;
                }

                m_Checked = value;
                OnCheckChanged();
            }
        }

        /// <summary>
        ///     Determines whether unchecking is allowed.
        /// </summary>
        protected virtual bool AllowUncheck => true;

        protected override void AdaptToScaleChange()
        {
            Size = new Size(BaseUnit);
        }

        /// <summary>
        ///     Toggles the checkbox.
        /// </summary>
        public override void Toggle()
        {
            base.Toggle();
            IsChecked = !IsChecked;
        }

        /// <summary>
        ///     Invoked when the checkbox has been checked.
        /// </summary>
        [XmlEvent] public event GwenEventHandler<EventArgs> Checked;

        /// <summary>
        ///     Invoked when the checkbox has been unchecked.
        /// </summary>
        [XmlEvent] public event GwenEventHandler<EventArgs> UnChecked;

        /// <summary>
        ///     Invoked when the checkbox state has been changed.
        /// </summary>
        [XmlEvent] public event GwenEventHandler<EventArgs> CheckChanged;

        /// <summary>
        ///     Handler for CheckChanged event.
        /// </summary>
        protected virtual void OnCheckChanged()
        {
            if (IsChecked)
            {
                if (Checked != null)
                {
                    Checked.Invoke(this, EventArgs.Empty);
                }
            }
            else
            {
                if (UnChecked != null)
                {
                    UnChecked.Invoke(this, EventArgs.Empty);
                }
            }

            if (CheckChanged != null)
            {
                CheckChanged.Invoke(this, EventArgs.Empty);
            }
        }

        protected override Size Measure(Size availableSize)
        {
            return new(width: 15, height: 15);
        }

        protected override Size Arrange(Size finalSize)
        {
            return MeasuredSize;
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Render(SkinBase skin)
        {
            base.Render(skin);
            skin.DrawCheckBox(this, m_Checked, IsDepressed);
        }

        /// <summary>
        ///     Internal OnPressed implementation.
        /// </summary>
        protected override void OnClicked(int x, int y)
        {
            if (IsDisabled)
            {
                return;
            }

            if (IsChecked && !AllowUncheck)
            {
                return;
            }

            base.OnClicked(x, y);
        }
    }
}
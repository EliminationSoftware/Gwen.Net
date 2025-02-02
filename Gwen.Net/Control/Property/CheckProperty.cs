﻿namespace Gwen.Net.Control.Property
{
    /// <summary>
    ///     Checkable property.
    /// </summary>
    public class CheckProperty : PropertyBase
    {
        protected readonly CheckBox m_CheckBox;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CheckProperty" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public CheckProperty(ControlBase parent)
            : base(parent)
        {
            m_CheckBox = new CheckBox(this);
            m_CheckBox.Dock = Dock.Left;
            m_CheckBox.ShouldDrawBackground = false;
            m_CheckBox.CheckChanged += OnValueChanged;
            m_CheckBox.IsTabable = true;
            m_CheckBox.KeyboardInputEnabled = true;
        }

        /// <summary>
        ///     Property value.
        /// </summary>
        public override string Value
        {
            get => m_CheckBox.IsChecked ? "1" : "0";
            set => base.Value = value;
        }

        /// <summary>
        ///     Indicates whether the property value is being edited.
        /// </summary>
        public override bool IsEditing => m_CheckBox.HasFocus;

        /// <summary>
        ///     Indicates whether the control is hovered by mouse pointer.
        /// </summary>
        public override bool IsHovered => base.IsHovered || m_CheckBox.IsHovered;

        /// <summary>
        ///     Sets the property value.
        /// </summary>
        /// <param name="value">Value to set.</param>
        /// <param name="fireEvents">Determines whether to fire "value changed" event.</param>
        public override void SetValue(string value, bool fireEvents = false)
        {
            if (value == "1" || value.ToLower() == "true" || value.ToLower() == "yes")
            {
                m_CheckBox.IsChecked = true;
            }
            else
            {
                m_CheckBox.IsChecked = false;
            }
        }
    }
}
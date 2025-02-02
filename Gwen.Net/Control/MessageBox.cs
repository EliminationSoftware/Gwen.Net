﻿using System;
using Gwen.Net.Control.Layout;
using Gwen.Net.RichText;

namespace Gwen.Net.Control
{
    public enum MessageBoxButtons
    {
        AbortRetryIgnore,
        OK,
        OKCancel,
        RetryCancel,
        YesNo,
        YesNoCancel
    }

    public enum MessageBoxResult
    {
        Abort,
        Retry,
        Ignore,
        Ok,
        Cancel,
        Yes,
        No
    }

    public class MessageBoxResultEventArgs : EventArgs
    {
        public MessageBoxResult Result { get; set; }
    }

    /// <summary>
    ///     Simple message box.
    /// </summary>
    public class MessageBox : Window
    {
        private readonly RichLabel m_Text;

        /// <summary>
        ///     Invoked when the message box has been dismissed.
        /// </summary>
        public GwenEventHandler<MessageBoxResultEventArgs> Dismissed;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MessageBox" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        /// <param name="text">Message to display.</param>
        /// <param name="caption">Window caption.</param>
        /// <param name="buttons">Message box buttons.</param>
        public MessageBox(ControlBase parent, string text, string caption = "",
            MessageBoxButtons buttons = MessageBoxButtons.OK)
            : base(parent)
        {
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;

            Canvas canvas = GetCanvas();
            MaximumSize = new Size((int)(canvas.ActualWidth * 0.8f), canvas.ActualHeight);

            StartPosition = StartPosition.CenterParent;
            Title = caption;
            DeleteOnClose = true;

            DockLayout layout = new(this);

            m_Text = new RichLabel(layout);
            m_Text.Dock = Dock.Fill;
            m_Text.Margin = Margin.Ten;
            m_Text.Document = new Document(text);

            HorizontalLayout buttonsLayout = new(layout);
            buttonsLayout.Dock = Dock.Bottom;
            buttonsLayout.HorizontalAlignment = HorizontalAlignment.Center;

            switch (buttons)
            {
                case MessageBoxButtons.AbortRetryIgnore:
                    CreateButton(buttonsLayout, "Abort", MessageBoxResult.Abort);
                    CreateButton(buttonsLayout, "Retry", MessageBoxResult.Retry);
                    CreateButton(buttonsLayout, "Ignore", MessageBoxResult.Ignore);

                    break;
                case MessageBoxButtons.OK:
                    CreateButton(buttonsLayout, "Ok", MessageBoxResult.Ok);

                    break;
                case MessageBoxButtons.OKCancel:
                    CreateButton(buttonsLayout, "Ok", MessageBoxResult.Ok);
                    CreateButton(buttonsLayout, "Cancel", MessageBoxResult.Cancel);

                    break;
                case MessageBoxButtons.RetryCancel:
                    CreateButton(buttonsLayout, "Retry", MessageBoxResult.Retry);
                    CreateButton(buttonsLayout, "Cancel", MessageBoxResult.Cancel);

                    break;
                case MessageBoxButtons.YesNo:
                    CreateButton(buttonsLayout, "Yes", MessageBoxResult.Yes);
                    CreateButton(buttonsLayout, "No", MessageBoxResult.No);

                    break;
                case MessageBoxButtons.YesNoCancel:
                    CreateButton(buttonsLayout, "Yes", MessageBoxResult.Yes);
                    CreateButton(buttonsLayout, "No", MessageBoxResult.No);
                    CreateButton(buttonsLayout, "Cancel", MessageBoxResult.Cancel);

                    break;
            }
        }

        /// <summary>
        ///     Show message box.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        /// <param name="text">Message to display.</param>
        /// <param name="caption">Window caption.</param>
        /// <param name="buttons">Message box buttons.</param>
        /// <returns>Message box.</returns>
        public static MessageBox Show(ControlBase parent, string text, string caption = "",
            MessageBoxButtons buttons = MessageBoxButtons.OK)
        {
            MessageBox messageBox = new(parent, text, caption, buttons);

            return messageBox;
        }

        private void CreateButton(ControlBase parent, string text, MessageBoxResult result)
        {
            Button button = new(parent);
            button.Width = 70;
            button.Margin = Margin.Five;
            button.Text = text;
            button.UserData = result;
            button.Clicked += CloseButtonPressed;
            button.Clicked += DismissedHandler;
        }

        private void DismissedHandler(ControlBase control, EventArgs args)
        {
            if (Dismissed != null)
            {
                Dismissed.Invoke(this, new MessageBoxResultEventArgs {Result = (MessageBoxResult)control.UserData});
            }
        }
    }
}
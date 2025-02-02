﻿using Gwen.Net.Control;

namespace Gwen.Net.Input
{
    /// <summary>
    ///     Keyboard state.
    /// </summary>
    public class KeyData
    {
        public readonly bool[] KeyState;
        public readonly float[] NextRepeat;
        public bool LeftMouseDown;
        public bool RightMouseDown;
        public ControlBase Target;

        public KeyData()
        {
            KeyState = new bool[(int)GwenMappedKey.Count];
            NextRepeat = new float[(int)GwenMappedKey.Count];
            // everything is initialized to 0 by default
        }
    }
}
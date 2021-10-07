﻿using System;
using Gwen.Net.Control;
using OpenToolkit.Mathematics;

namespace Gwen.Net.OpenTk
{
    public interface IGwenGui : IDisposable
    {
        ControlBase Root { get; }

        void Load();

        void Resize(Vector2i newSize);

        void Render();
    }
}
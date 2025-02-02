﻿using System;
using System.Collections.Generic;
using System.Linq;
using Gwen.Net.Control;

namespace Gwen.Net.Anim
{
    public class Animation
    {
        //private static List<Animation> g_AnimationsListed = new List<Animation>(); // unused
        private static readonly Dictionary<ControlBase, List<Animation>> m_Animations = new();
        protected ControlBase m_Control;

        public virtual bool Finished => throw new InvalidOperationException("Pure virtual function call");

        protected virtual void Think() {}

        public static void Add(ControlBase control, Animation animation)
        {
            animation.m_Control = control;

            if (!m_Animations.ContainsKey(control))
            {
                m_Animations[control] = new List<Animation>();
            }

            m_Animations[control].Add(animation);
        }

        public static void Cancel(ControlBase control)
        {
            if (m_Animations.ContainsKey(control))
            {
                m_Animations[control].Clear();
                m_Animations.Remove(control);
            }
        }

        internal static void GlobalThink()
        {
            foreach (KeyValuePair<ControlBase, List<Animation>> pair in m_Animations)
            {
                IEnumerable<Animation>
                    valCopy = pair.Value.TakeWhile(
                        x => true); // list copy so foreach won't break when we remove elements

                foreach (Animation animation in valCopy)
                {
                    animation.Think();

                    if (animation.Finished)
                    {
                        pair.Value.Remove(animation);
                    }
                }
            }
        }
    }
}
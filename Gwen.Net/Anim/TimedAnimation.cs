﻿using System;
using Gwen.Net.Platform;

namespace Gwen.Net.Anim
{
    // Timed animation. Provides a useful base for animations.
    public class TimedAnimation : Animation
    {
        private readonly float m_Ease;
        private readonly float m_End;
        private readonly float m_Start;
        private bool m_Finished;
        private bool m_Started;

        public TimedAnimation(float length, float delay = 0.0f, float ease = 1.0f)
        {
            m_Start = GwenPlatform.GetTimeInSeconds() + delay;
            m_End = m_Start + length;
            m_Ease = ease;
            m_Started = false;
            m_Finished = false;
        }

        public override bool Finished => m_Finished;

        protected override void Think()
        {
            //base.Think();

            if (m_Finished)
            {
                return;
            }

            float current = GwenPlatform.GetTimeInSeconds();
            float secondsIn = current - m_Start;

            if (secondsIn < 0.0)
            {
                return;
            }

            if (!m_Started)
            {
                m_Started = true;
                OnStart();
            }

            float delta = secondsIn / (m_End - m_Start);

            if (delta < 0.0f)
            {
                delta = 0.0f;
            }

            if (delta > 1.0f)
            {
                delta = 1.0f;
            }

            Run((float)Math.Pow(delta, m_Ease));

            if (delta == 1.0f)
            {
                m_Finished = true;
                OnFinish();
            }
        }

        // These are the magic functions you should be overriding

        protected virtual void OnStart() {}

        protected virtual void Run(float delta) {}

        protected virtual void OnFinish() {}
    }
}
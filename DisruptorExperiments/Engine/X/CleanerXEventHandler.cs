﻿using Disruptor;

namespace DisruptorExperiments.Engine.X
{
    public class CleanerXEventHandler : IEventHandler<XEvent>
    {
        public void OnEvent(XEvent data, long sequence, bool endOfBatch)
        {
            data.Reset();
        }
    }
}
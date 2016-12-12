﻿using System;
using System.Threading.Tasks;
using Disruptor;
using Disruptor.Dsl;

namespace DisruptorExperiments.Engine.X.Engines.V1_SyncBasedConflaction
{
    public class XEngine
    {
        private readonly Disruptor<XEvent> _disrutpor;
        private readonly RingBuffer<XEvent> _ringBuffer;

        public XEngine()
        {
            _disrutpor = new Disruptor<XEvent>(() => new XEvent(1), 32768, TaskScheduler.Default);
            _disrutpor.HandleEventsWith(new BusinessXEventHandler())
                .Then(new MetricPublisherXEventHandler())
                .Then(new CleanerXEventHandler())
                ;

            _ringBuffer = _disrutpor.RingBuffer;
        }

        public AcquireScope<XEvent> AcquireEvent()
        {
            var sequence = _ringBuffer.Next();
            var data = _ringBuffer[sequence];
            data.OnAcquired();
            return new AcquireScope<XEvent>(_ringBuffer, sequence, data);
        }

        public void Start()
        {
            _disrutpor.Start();
        }

        public void Stop()
        {
            _disrutpor.Shutdown();
        }
    }
}
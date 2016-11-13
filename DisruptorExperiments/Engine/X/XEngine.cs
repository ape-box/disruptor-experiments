﻿using System;
using System.Threading.Tasks;
using Disruptor;
using Disruptor.Dsl;

namespace DisruptorExperiments.Engine.X
{
    public class XEngine : Interfaces.V1.IXEngine, Interfaces.V2.IXEngine
    {
        private readonly Disruptor<XEvent> _disrutpor;
        private readonly RingBuffer<XEvent> _ringBuffer;

        public XEngine()
        {
            _disrutpor = new Disruptor<XEvent>(() => new XEvent(), 32768, TaskScheduler.Default);
            //_disrutpor.HandleEventsWith(new BusinessXEventHandler())
            //    .Then(new StatePublisherXEventHandler())
            //    .Then(new DumperXEventHandler())
            //    .Then(new MetricPublisherXEventHandler())
            //    .Then(new CleanerXEventHandler())
            //    ;

            _disrutpor.HandleEventsWith(new CleanerXEventHandler());

            _ringBuffer = _disrutpor.RingBuffer;
        }

        public AcquireScope<XEvent> AcquireEvent()
        {
            var sequence = _ringBuffer.Next();
            _ringBuffer[sequence].OnAcquired();
            return new AcquireScope<XEvent>(_ringBuffer, sequence);
        }

        public void Start()
        {
            _disrutpor.Start();
        }

        public void Stop()
        {
            _disrutpor.Shutdown();
        }

        public void EnqueueMarketData(int securityId, long bidPrice, long askPrice)
        {
            throw new NotSupportedException();
        }

        public void EnqueueExecution(int securityId, long price, long quantity)
        {
            var sequence = _ringBuffer.Next();
            try
            {
                var evt = _ringBuffer[sequence];
                evt.OnAcquired();
                evt.SetExecution(securityId, price, quantity);
            }
            finally
            {
                _ringBuffer.Publish(sequence);
            }
        }

        public void EnqueueTradingSignal1(int securityId, long value1, long value2, long value3, long value4)
        {
            var sequence = _ringBuffer.Next();
            try
            {
                var evt = _ringBuffer[sequence];
                evt.OnAcquired();
                evt.SetTradingSignal1(securityId, value1, value2, value3, value4);
            }
            finally
            {
                _ringBuffer.Publish(sequence);
            }
        }
    }
}
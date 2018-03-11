﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using DisruptorExperiments.MarketData;

namespace DisruptorExperiments.Engine.X.Engines.V1_SyncBasedConflation
{
    public class XEvent
    {
        public XEvent(int eventHandlerCount)
        {
            HandlerMetrics = new HandlerMetricInfo[eventHandlerCount];
        }

        public readonly HandlerMetricInfo[] HandlerMetrics;
        public long AcquireTimestamp;

        public XEventType EventType;
        public EventInfo EventData;

        public void OnAcquired()
        {
            AcquireTimestamp = Stopwatch.GetTimestamp();
        }

        public void Reset()
        {
            EventType = XEventType.None;
            EventData = default(EventInfo);
        }

        public void SetMarketData(int securityId, MarketDataConflater marketDataConflater, MarketDataUpdate update)
        {
            EventType = XEventType.MarketData;
            EventData.MarketData.Conflater = marketDataConflater;
            EventData.MarketData.SecurityId = securityId;
            EventData.MarketData.BidOrZero = update.Bid.GetValueOrDefault();
            EventData.MarketData.AskOrZero = update.Ask.GetValueOrDefault();
            EventData.MarketData.LastOrZero = update.Last.GetValueOrDefault();
            EventData.MarketData.UpdateCount = update.UpdateCount;
        }

        public void SetExecution(int securityId, long price, long quantity)
        {
            EventType = XEventType.Execution;
            EventData.Execution.SecurityId = securityId;
            EventData.Execution.Price = price;
            EventData.Execution.Quantity = quantity;
        }

        public void SetTradingSignal1(int securityId, long value1, long value2, long value3, long value4)
        {
            EventType = XEventType.TradingSignal1;
            EventData.TradingSignal1.SecurityId = securityId;
            EventData.TradingSignal1.Value1 = value1;
            EventData.TradingSignal1.Value2 = value2;
            EventData.TradingSignal1.Value3 = value3;
            EventData.TradingSignal1.Value4 = value4;
        }

        public struct HandlerMetricInfo
        {
            public long BeginTimestamp;
            public long EndTimestamp;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct EventInfo
        {
            [FieldOffset(0)]
            public MarketDataInfo MarketData;

            [FieldOffset(0)]
            public ExecutionInfo Execution;

            [FieldOffset(0)]
            public TradingSignal1Info TradingSignal1;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct MarketDataInfo
        {
            [FieldOffset(0)]
            public MarketDataConflater Conflater;
            [FieldOffset(8)]
            public int SecurityId;
            [FieldOffset(12)]
            public long BidOrZero;
            [FieldOffset(20)]
            public long AskOrZero;
            [FieldOffset(28)]
            public long LastOrZero;
            [FieldOffset(36)]
            public int UpdateCount;

            internal void MergeWith(MarketDataUpdate update)
            {
                if (update.Bid != null)
                    BidOrZero = update.Bid.GetValueOrDefault();

                if (update.Ask != null)
                    AskOrZero = update.Ask.GetValueOrDefault();

                if (update.Last != null)
                    LastOrZero = update.Last.GetValueOrDefault();

                UpdateCount += update.UpdateCount;
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct ExecutionInfo
        {
            [FieldOffset(0)]
            private object _ref;
            [FieldOffset(8)]
            public int SecurityId;
            [FieldOffset(12)]
            public long Price;
            [FieldOffset(20)]
            public long Quantity;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct TradingSignal1Info
        {
            [FieldOffset(0)]
            private object _ref;
            [FieldOffset(8)]
            public int SecurityId;
            [FieldOffset(12)]
            public long Value1;
            [FieldOffset(20)]
            public long Value2;
            [FieldOffset(28)]
            public long Value3;
            [FieldOffset(36)]
            public long Value4;
        }
        
    }
}
using System;
using System.Collections.Generic;
using GameFramework.Generic;

namespace GameFramework.EventPoolService
{
    public partial class EventPool
    {
        private readonly MultiDictionary<int, EventHandler> eventHandlers;
        private readonly Queue<EventArgs> events;

        public bool IsEnableStrictCheck { get; set; }

        public EventPool(int capacity = 4)
        {
            eventHandlers = new MultiDictionary<int, EventHandler>(capacity);
            events = new Queue<EventArgs>(capacity);
        }

        public int EventCount
        {
            get { return eventHandlers.KeyCount; }
        }

        public int HandlerCount(int id)
        {
            return eventHandlers.ValueCount(id);
        }

        public bool ContainsEvent(int id)
        {
            return eventHandlers.ContainsKey(id);
        }

        public bool Contains(int id, EventHandler handler)
        {
            if (IsEnableStrictCheck && handler == null)
            {
                GameLogger.LogError($"Event {id} handler is invalid");
            }

            return eventHandlers.Contains(id, handler);
        }

        public void Register(int id, EventHandler handler)
        {
            if (IsEnableStrictCheck && Contains(id, handler))
            {
                GameLogger.LogError($"Event {id} is already subscribed");
                return;
            }

            eventHandlers.Add(id, handler);
        }

        public void Unregister(int id, EventHandler handler)
        {
            if (IsEnableStrictCheck && handler == null)
            {
                GameLogger.LogError($"Event {id} handler is invalid");
                return;
            }

            if (!eventHandlers.Remove(id, handler) && IsEnableStrictCheck)
            {
                GameLogger.LogError($"Event {id} not exist specified handler");
            }
        }

        public void Send(int id, Data data)
        {
            EventArgs args = new EventArgs()
            {
                id = id,
                data = data
            };
            HandleEvent(args);
        }

        public void SendAsync(int id, Data data)
        {
            lock (events)
            {
                EventArgs args = new EventArgs()
                {
                    id = id,
                    data = data
                };
                events.Enqueue(args);
            }
        }

        public void Clear()
        {
            lock (events)
            {
                events.Clear();
                eventHandlers.Clear();
            }
        }

        public void Update()
        {
            lock (events)
            {
                while (events.Count != 0)
                {
                    HandleEvent(events.Dequeue());
                }
            }
        }

        private void HandleEvent(EventArgs args)
        {
            if (eventHandlers.TryGetValue(args.id, out LinkedListRange<EventHandler> range))
            {
                foreach (EventHandler handler in range)
                {
                    try
                    {
                        handler(new EventBody(args.id, args.data, this, handler));
                    }
                    catch (Exception ex)
                    {
                        GameLogger.LogError($"PersistentStorage is thrown exception : {ex} {this}");
                    }
                }
            }

            ReferencePool.Release(args.data);
        }
    }
}
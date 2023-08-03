using System;
using System.Collections.Generic;

namespace GameFramework
{
    public delegate void EventHandler(EventBody body);

    public class EventPool
    {
        private MultiDictionary<int, EventHandler> eventHandlers;
        private Queue<EventArgs> events;

        public bool EnableStrictCheck { get; set; }

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
            if (EnableStrictCheck && handler == null)
            {
                GameLogger.LogError($"Event {id} handler is invalid");
            }

            return eventHandlers.Contains(id, handler);
        }

        public void Register(int id, EventHandler handler)
        {
            if (EnableStrictCheck && Contains(id, handler))
            {
                GameLogger.LogError($"Event {id} is already subscribed");
                return;
            }

            eventHandlers.Add(id, handler);
        }

        public void Unregister(int id, EventHandler handler)
        {
            if (EnableStrictCheck && handler == null)
            {
                GameLogger.LogError($"Event {id} handler is invalid");
                return;
            }

            if (!eventHandlers.Remove(id, handler) && EnableStrictCheck)
            {
                GameLogger.LogError($"Event {id} not exist specified handler");
            }
        }

        public void Send(int id, GameData gameData)
        {
            EventArgs args = new EventArgs()
            {
                id = id,
                GameData = gameData
            };
            HandleEvent(args);
        }

        public void SendAsync(int id, GameData gameData)
        {
            lock (events)
            {
                EventArgs args = new EventArgs()
                {
                    id = id,
                    GameData = gameData
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
                        handler(new EventBody(args.id, args.GameData, this, handler));
                    }
                    catch (Exception ex)
                    {
                        GameLogger.LogError($"PersistentStorage is thrown exception : {ex} {this}");
                    }
                }
            }

            ReferencePool.Instance.Release(args.GameData);
        }

        private struct EventArgs
        {
            public int id;
            public GameData GameData;
        }
    }
}
using GameFramework.Generic;

namespace GameFramework.EventPoolService
{
    public class EventManager : Service
    {
        private readonly EventPool eventPool = new EventPool();

        public int EventCount
        {
            get { return eventPool.EventCount; }
        }

        public int HandlerCount(int id)
        {
            return eventPool.HandlerCount(id);
        }

        public bool ContainsEvent(int id)
        {
            return eventPool.ContainsEvent(id);
        }

        private void Update()
        {
            eventPool.Update();
        }

        public bool Contains(int id, EventHandler handler)
        {
            return eventPool.Contains(id, handler);
        }

        public void Register(int id, EventHandler handler)
        {
            eventPool.Register(id, handler);
        }

        public void Unregister(int id, EventHandler handler)
        {
            eventPool.Unregister(id, handler);
        }

        public void Send(int id, Data data = null)
        {
            eventPool.Send(id, data);
        }

        public void SendAsync(int id, Data data = null)
        {
            eventPool.SendAsync(id, data);
        }

        public void Clear()
        {
            eventPool.Clear();
        }
    }
}
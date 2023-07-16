namespace GameFramework
{
    public class EventManager : PersistentSingleton<EventManager>
    {
        private EventPool eventPool = new EventPool();

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

        public void Send(int id, GameData gameData = null)
        {
            eventPool.Send(id, gameData);
        }

        public void SendAsync(int id, GameData gameData = null)
        {
            eventPool.SendAsync(id, gameData);
        }

        public void Clear()
        {
            eventPool.Clear();
        }

        private void Update()
        {
            eventPool.Update();
        }
    }
}
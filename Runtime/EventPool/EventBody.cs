namespace GameFramework
{
    public struct EventBody
    {
        private int id;
        private GameData gameData;
        private EventPool pool;
        private EventHandler handler;

        public EventBody(int id, GameData gameData, EventPool pool, EventHandler handler)
        {
            this.id = id;
            this.gameData = gameData;
            this.pool = pool;
            this.handler = handler;
        }

        public int Id
        {
            get { return id; }
        }

        public T GetData<T>() where T : GameData
        {
            return gameData as T;
        }

        public GameData GetData()
        {
            return gameData;
        }

        public void Unregister()
        {
            pool.Unregister(id, handler);
        }
    }
}
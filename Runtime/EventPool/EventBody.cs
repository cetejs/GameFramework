using GameFramework.Generic;

namespace GameFramework.EventPoolService
{
    public struct EventBody
    {
        private readonly int id;
        private readonly Data data;
        private readonly EventPool pool;
        private readonly EventHandler handler;

        public EventBody(int id, Data data, EventPool pool, EventHandler handler)
        {
            this.id = id;
            this.data = data;
            this.pool = pool;
            this.handler = handler;
        }

        public int Id
        {
            get { return id; }
        }

        public T GetData<T>() where T : Data
        {
            return data as T;
        }

        public Data GetData()
        {
            return data;
        }

        public void Unregister()
        {
            pool.Unregister(id, handler);
        }
    }
}
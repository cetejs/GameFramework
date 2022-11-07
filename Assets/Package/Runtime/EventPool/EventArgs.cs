using GameFramework.Generic;

namespace GameFramework.EventPoolService
{
    public partial class EventPool
    {
        private struct EventArgs
        {
            public int id;
            public Data data;
        }
    }
}
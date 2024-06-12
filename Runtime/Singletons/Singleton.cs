namespace GameFramework
{
    public abstract class Singleton<T> : ISingleton where T : Singleton<T>, new()
    {
        private static object syncObject = new object();

        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncObject)
                    {
                        if (instance == null)
                        {
                            instance = SingletonCreator.CreateSingleton<T>();
                        }
                    }
                }

                return instance;
            }
        }

        public static void Dispose()
        {
            if (instance != null)
            {
                SingletonCreator.DisposeSingleton(instance);
            }
        }

        void ISingleton.OnDispose()
        {
            instance = null;
        }
    }
}
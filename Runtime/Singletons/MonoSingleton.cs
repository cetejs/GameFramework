namespace GameFramework
{
    public abstract class MonoSingleton<T> : GameBehaviour, ISingleton where T : MonoSingleton<T>
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = SingletonCreator.CreateMonoSingleton<T>();
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

        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
                SingletonCreator.AddMonoSingleton(instance);
            }

            if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            if (instance == this)
            {
                SingletonCreator.RemoveMonoSingleton(instance);
                instance = null;
            }
        }

        void ISingleton.OnDispose()
        {
            if (instance != null)
            {
                Destroy(instance.gameObject);
                instance = null;
            }
        }
    }
}
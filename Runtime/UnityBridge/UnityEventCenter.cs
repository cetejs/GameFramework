using System;
using UnityEngine;

namespace GameFramework
{
    public class UnityEventCenter : GameBehaviour
    {
        private static UnityEventCenter instance;

        public event Action OnUpdateEvent;
        public event Action OnFixedUpdateEvent;
        public event Action OnLateUpdateEvent;
        public event Action OnGUIEvent;
        public event Action OnDrawGizmosEvent;
        public event Action<bool> OnApplicationFocusEvent;
        public event Action<bool> OnApplicationPauseEvent;
        public event Action OnApplicationQuitEvent;

        public static bool IsApplicationQuit { get; private set; }

        [RuntimeInitializeOnLoadMethod]
        private static void Initialize()
        {
            instance = new GameObject(nameof(UnityEventCenter)).AddComponent<UnityEventCenter>();
            DontDestroyOnLoad(instance.gameObject);
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }

            IsApplicationQuit = false;
        }

        private void OnDestroy()
        {
            if (instance != this)
            {
                instance = null;
            }
        }

        private void Update()
        {
            OnUpdateEvent?.Invoke();
        }

        private void FixedUpdate()
        {
            OnFixedUpdateEvent?.Invoke();
        }

        private void LateUpdate()
        {
            OnLateUpdateEvent?.Invoke();
        }

        private void OnGUI()
        {
            OnGUIEvent?.Invoke();
        }

        private void OnDrawGizmos()
        {
            OnDrawGizmosEvent?.Invoke();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            OnApplicationFocusEvent?.Invoke(hasFocus);
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            OnApplicationPauseEvent?.Invoke(pauseStatus);
        }

        private void OnApplicationQuit()
        {
            IsApplicationQuit = true;
            OnApplicationQuitEvent?.Invoke();
        }
    }
}
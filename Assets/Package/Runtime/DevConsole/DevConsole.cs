using GameFramework.Generic;
using GameFramework.InputService;
using UnityEngine;

namespace GameFramework.DevConsoleService
{
    internal class DevConsole : MonoBehaviour
    {
#if UNITY_EDITOR || ENABLE_CONSOLE
        private KeyCode keyCode;
        private static DevConsole instance;

        public static DevConsole Instance
        {
            get
            {
                if (!instance)
                {
                    Initialize();
                }

                return instance;
            }
        }

        [RuntimeInitializeOnLoadMethod]
        private static void Initialize()
        {
            instance = FindObjectOfType<DevConsole>();
            if (!instance)
            {
                instance = Instantiate(Resources.Load<DevConsole>("DevConsole"));
                instance.name = "DevConsole";
            }

            instance.console.SetActive(false);
            DontDestroyOnLoad(instance.gameObject);
        }

        private void Start()
        {
            Global.RequireService<InputManager>();
            keyCode = DevConsoleConfig.Get().consoleKey;
        }

        private void Update()
        {
            if (Input.GetKeyDown(keyCode))
            {
                EnableMainPanel();
            }
        }
#endif

        [SerializeField]
        private GameObject console;
        private float lastTimeScale;
        private CursorLockMode lastLockMode;
        private bool lastVisible;

        public void EnableMainPanel()
        {
            DevConsoleConfig config = DevConsoleConfig.Get();
            bool isEnable = !console.activeSelf;
            console.SetActive(isEnable);
            if (isEnable)
            {
                lastTimeScale = Time.timeScale;
                Time.timeScale = config.consoleTimeScale;
            }
            else
            {
                Time.timeScale = lastTimeScale;
            }

#if UNITY_EDITOR || UNITY_STANDALONE
            if (isEnable)
            {
                lastLockMode = Cursor.lockState;
                lastVisible = Cursor.visible;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = lastLockMode;
                Cursor.visible = lastVisible;
            }
#endif
        }
    }
}
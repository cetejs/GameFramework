using GameFramework.Generic;
using GameFramework.InputService;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramework.DevConsoleService
{
    internal class DevConsole : MonoBehaviour
    {
        [SerializeField]
        private GameObject console;
        [SerializeField]
        private Button showConsoleBtn;
        private KeyCode keyCode;
        private float lastTimeScale;
        private CursorLockMode lastLockMode;
        private bool lastVisible;

        public float TimeScale
        {
            get { return lastTimeScale; }
            set { lastTimeScale = value; }
        }
        
#if UNITY_EDITOR || ENABLE_CONSOLE
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
            
            showConsoleBtn.onClick.AddListener(EnableMainPanel);
        }

        private void Update()
        {
            if (Input.GetKeyDown(keyCode))
            {
                EnableMainPanel();
            }
        }

        private void EnableMainPanel()
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
#endif
    }
}
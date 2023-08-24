using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameFramework
{
    internal class DevConsole : MonoBehaviour
    {
        [SerializeField]
        private GameObject console;
        [SerializeField]
        private Button showConsoleBtn;
        [SerializeField]
        private GameObject defaultSelectedGo;
        private float lastTimeScale;
        private CursorLockMode lastLockMode;
        private bool lastVisible;
        private InputManager input;

        public float TimeScale
        {
            get { return lastTimeScale; }
            set { lastTimeScale = value; }
        }

#if ENABLE_CONSOLE
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
            input = InputManager.Instance;
            showConsoleBtn.onClick.AddListener(EnableMainPanel);
        }

        private void Update()
        {
            if (Input.GetKeyDown(GameSettings.Instance.ConsoleInput))
            {
                EnableMainPanel();
            }
        }

        private void EnableMainPanel()
        {
            bool isEnable = !console.activeSelf;
            console.SetActive(isEnable);
            if (isEnable)
            {
                lastTimeScale = Time.timeScale;
                Time.timeScale = GameSettings.Instance.ConsoleTimeScale;
                input.SetSelectedGameObject(defaultSelectedGo);
            }
            else
            {
                Time.timeScale = lastTimeScale;
                input.SetSelectedGameObject(showConsoleBtn.gameObject);
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
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramework
{
    [RequireComponent(typeof(Toggle))]
    public class UIToggle : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> showObjs = new List<GameObject>();
        [SerializeField]
        private List<GameObject> hideObjs = new List<GameObject>();
        private Toggle toggle;

        private void Awake()
        {
            toggle = GetComponent<Toggle>();
            toggle.onValueChanged.AddListener(OnValueChanged);
            OnValueChanged(toggle.isOn);
        }

        private void OnValueChanged(bool isOn)
        {
            foreach (GameObject obj in showObjs)
            {
                obj.SetActive(isOn);
            }

            foreach (GameObject obj in hideObjs)
            {
                obj.SetActive(!isOn);
            }
        }
    }
}
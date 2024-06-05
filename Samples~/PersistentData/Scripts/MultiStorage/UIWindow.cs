using UnityEngine;

namespace GameFramework.MultiStorage
{
    public abstract class UIWindow : MonoBehaviour
    {
        public virtual void Show(object arg)
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
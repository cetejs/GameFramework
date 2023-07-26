using UnityEngine;

namespace GameFramework
{
    public abstract class PersistentSave<T> : GameBehaviour where T : new()
    {
        [SerializeField]
        protected string persistentKey = "DefaultData";

        public T SaveData { get; private set; }

        protected virtual void OnEnable()
        {
            SaveData = PersistentManager.Instance.GetData<T>(persistentKey) ?? new T();
        }

        protected virtual void OnDisable()
        {
            PersistentManager.Instance.SetData(persistentKey, SaveData);
        }
    }
}
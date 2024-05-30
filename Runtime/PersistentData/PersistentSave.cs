using System;
using UnityEngine;

namespace GameFramework
{
    public abstract class PersistentSave<T> : GameBehaviour, ISerializationCallbackReceiver where T : new()
    {
        [SerializeField]
        protected string storageName;
        [SerializeField]
        protected string persistentKey;

        public T SaveData { get; protected set; }

        protected virtual void Reset()
        {
            storageName = PersistentSetting.Instance.DefaultStorageName;
            persistentKey = Guid.NewGuid().ToString();
        }

        protected virtual void OnEnable()
        {
            SaveData = PersistentManager.Instance.GetData<T>(storageName, persistentKey) ?? new T();
        }

        protected virtual void OnDisable()
        {
            PersistentManager.Instance.SetData(storageName, persistentKey, SaveData);
        }

        public virtual void OnBeforeSerialize()
        {
            if (string.IsNullOrEmpty(storageName))
            {
                storageName = PersistentSetting.Instance.DefaultStorageName;
            }

            if (string.IsNullOrEmpty(persistentKey))
            {
                persistentKey = Guid.NewGuid().ToString();
            }
        }

        public virtual void OnAfterDeserialize()
        {
        }
    }
}
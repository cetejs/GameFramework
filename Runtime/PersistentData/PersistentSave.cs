using System;
using UnityEngine;

namespace GameFramework
{
    public abstract class PersistentSave<T> : GameBehaviour where T : new()
    {
        [SerializeField]
        protected string storageName;
        [SerializeField]
        protected string persistentKey;

        public T PersistentData { get; protected set; }

        protected virtual void Reset()
        {
            storageName = PersistentSetting.Instance.DefaultStorageName;
            persistentKey = Guid.NewGuid().ToString();
        }

        protected virtual void OnEnable()
        {
            PersistentData = PersistentManager.Instance.GetData<T>(storageName, persistentKey, PersistentData) ?? new T();
            OnGetPersistentData();

            PersistentManager.Instance.OnStorageLoading += OnStorageLoading;
            PersistentManager.Instance.OnStorageSaving += OnStorageSaving;
        }

        protected virtual void OnDisable()
        {
            if (UnityEventCenter.IsApplicationQuit)
            {
                return;
            }

            PersistentManager.Instance.OnStorageLoading -= OnStorageLoading;
            PersistentManager.Instance.OnStorageSaving -= OnStorageSaving;

            OnSavePersistentData();
            PersistentManager.Instance.SetData(storageName, persistentKey, PersistentData);
        }

        protected virtual void OnGetPersistentData()
        {
        }

        protected virtual void OnSavePersistentData()
        {
        }

        private void OnStorageLoading(string storageName)
        {
            if (this.storageName != storageName)
            {
                return;
            }

            PersistentData = PersistentManager.Instance.GetData<T>(storageName, persistentKey, PersistentData) ?? new T();
            OnGetPersistentData();
        }

        private void OnStorageSaving(string storageName)
        {
            if (this.storageName != storageName)
            {
                return;
            }

            OnSavePersistentData();
            PersistentManager.Instance.SetData(storageName, persistentKey, PersistentData);
        }
    }
}
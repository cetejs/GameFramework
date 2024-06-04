using System;
using UnityEngine;

namespace GameFramework
{
    public class StorageAsyncOperation : CustomYieldInstruction
    {
        private IPersistentStorage storage;
        private Action<StorageAsyncOperation> onCompleted;

        public IPersistentStorage Storage
        {
            get { return storage; }
        }

        public PersistentState State
        {
            get { return storage.State; }
        }

        public bool IsDone
        {
            get { return State == PersistentState.Completed; }
        }

        public override bool keepWaiting
        {
            get { return State == PersistentState.Loading || State == PersistentState.Saving; }
        }

        public event Action<StorageAsyncOperation> OnCompleted
        {
            add
            {
                if (IsDone)
                {
                    value.Invoke(this);
                    return;
                }

                onCompleted += value;
            }
            remove { onCompleted -= value; }
        }

        internal StorageAsyncOperation(IPersistentStorage storage)
        {
            this.storage = storage;
        }

        internal virtual void Completed()
        {
            onCompleted?.Invoke(this);
            onCompleted = null;
        }
    }
}
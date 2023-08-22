using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameFramework
{
    public class AssetAsyncOperation : CustomYieldInstruction
    {
        protected bool isDone;
        protected Object result;
        protected AsyncOperation operation;
        protected AssetAsyncOperation dependency;
        protected Action<AssetAsyncOperation> onCompleted;

        public bool IsDone
        {
            get { return operation != null && operation.isDone || isDone || result != null; }
        }

        public virtual float Progress
        {
            get
            {
                if (operation != null)
                {
                    if (dependency != null)
                    {
                        return (operation.progress + dependency.Progress) / 2f;
                    }

                    return operation.progress;
                }

                return isDone ? 1f : 0f;
            }
        }

        public virtual Object Result
        {
            get { return result; }
        }

        public override bool keepWaiting
        {
            get { return !isDone; }
        }

        public event Action<AssetAsyncOperation> OnCompleted
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

        public T GetResult<T>() where T : Object
        {
            return result as T;
        }

        internal virtual void SetOperation(AsyncOperation op)
        {
            operation = op;
        }

        internal virtual void SetDependency(AssetAsyncOperation dp)
        {
            dependency = dp;
        }

        internal virtual void Completed(Object asset)
        {
            isDone = true;
            result = asset;
            onCompleted?.Invoke(this);
            onCompleted = null;
        }
    }
}
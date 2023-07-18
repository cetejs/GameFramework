using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public class BundleAsyncOperation : AssetAsyncOperation
    {
        private IList<AssetAsyncOperation> dependencies;

        public override float Progress
        {
            get
            {
                if (isDone)
                {
                    return 1f;
                }

                float numOfOps = 1f;
                float total = operation.progress;
                if (dependencies != null)
                {
                    numOfOps += dependencies.Count;
                    foreach (AssetAsyncOperation dp in dependencies)
                    {
                        total += dp.Progress;
                    }
                }

                return total / numOfOps;
            }
        }

        internal void SetDependencies(IList<AssetAsyncOperation> dps)
        {
            dependencies = dps;
            foreach (AssetAsyncOperation dp in dps)
            {
                dp.OnCompleted += _ =>
                {
                    Completed(null);
                };
            }
        }

        internal override void Completed(Object asset)
        {
            if (asset != null)
            {
                result = asset;
            }

            if (!IsDone)
            {
                return;
            }

            if (dependencies != null)
            {
                foreach (AssetAsyncOperation dp in dependencies)
                {
                    if (!dp.IsDone)
                    {
                        return;
                    }
                }
            }

            isDone = true;
            onCompleted?.Invoke(this);
            onCompleted = null;
        }
    }
}
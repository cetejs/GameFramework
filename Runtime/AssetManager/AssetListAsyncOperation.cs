using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public class AssetListAsyncOperation : AssetAsyncOperation
    {
        private IList<AssetAsyncOperation> operations = new List<AssetAsyncOperation>();

        public override float Progress
        {
            get
            {
                if (isDone)
                {
                    return 1f;
                }

                float numOfOps = 1f;
                float total = 1f;
                numOfOps += operations.Count;
                foreach (AssetAsyncOperation op in operations)
                {
                    total += op.Progress;
                }

                return total / numOfOps;
            }
        }

        internal void AddOperation(AssetAsyncOperation op)
        {
            operations.Add(op);
            op.OnCompleted += _ =>
            {
                Completed(null);
            };
        }

        internal override void Completed(Object asset)
        {
            if (asset != null)
            {
                result = asset;
            }

            foreach (AssetAsyncOperation op in operations)
            {
                if (!op.IsDone)
                {
                    return;
                }
            }

            isDone = true;
            onCompleted?.Invoke(this);
            onCompleted = null;
        }
    }
}
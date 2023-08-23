using UnityEngine;

namespace GameFramework
{
    public class SceneAsyncOperation : AssetAsyncOperation
    {
        private bool allowSceneActivation = true;

        public override float Progress
        {
            get
            {
                if (isDone)
                {
                    return 1f;
                }

                if (operation == null)
                {
                    return 0f;
                }

                float numOfOps = 1f;
                float total = operation.progress;
                if (dependency != null)
                {
                    total += dependency.Progress;
                    numOfOps++;
                }

                return total / numOfOps;
            }
        }

        public bool AllowSceneActivation
        {
            get { return allowSceneActivation; }
            set
            {
                allowSceneActivation = value;
                if (operation != null)
                {
                    operation.allowSceneActivation = value;
                }
            }
        }

        internal override void SetOperation(AsyncOperation op)
        {
            base.SetOperation(op);
            op.allowSceneActivation = allowSceneActivation;
        }

        internal override void SetDependency(AssetAsyncOperation dp)
        {
            dependency = dp;
        }
    }
}
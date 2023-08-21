using System;

namespace GameFramework
{
    public class IntegrityAsyncOperation
    {
        internal int Length;
        internal int TotalLength;
        private bool isDone;
        private Action<IntegrityAsyncOperation> onCompleted;

        public IntegrityStatus Status { get; internal set; }

        public bool IsDone
        {
            get { return isDone || Status == IntegrityStatus.NetworkError; }
        }

        public float Progress
        {
            get
            {
                if (isDone)
                {
                    return 1f;
                }

                if (TotalLength > 0)
                {
                    return Length / (float) TotalLength;
                }

                return 0f;
            }
        }

        public event Action<IntegrityAsyncOperation> OnCompleted
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

        internal void Complete()
        {
            isDone = true;
            onCompleted?.Invoke(this);
            onCompleted = null;
        }
    }

    public enum IntegrityStatus
    {
        CompareHash,
        DownloadBundles,
        Success,
        NetworkError
    }
}
using System;
using System.Collections.Generic;

namespace GameFramework
{
    public class CatalogsAsyncOperation
    {
        internal List<BundleCatalog> Catalogs;
        internal List<long> BundleLengths;
        internal int DownloadCount;
        private bool isDone;
        private long totalLength;
        private Action<CatalogsAsyncOperation> onCompleted;

        public UpdateCatalogsStatus Status { get; internal set; }

        public bool IsDone
        {
            get
            {
                return  isDone || Status == UpdateCatalogsStatus.NetworkError;
            }
        }

        public float Progress
        {
            get
            {
                if (isDone)
                {
                    return 1f;
                }

                if (BundleLengths != null && BundleLengths.Count > 0)
                {
                   return DownloadCount / (float)BundleLengths.Count;
                }

                return 0f;
            }
        }

        public float Length
        {
            get
            {
                long result = 0L;
                if (BundleLengths != null)
                {
                    for (int i = 0; i < DownloadCount; i++)
                    {
                        result += BundleLengths[i];
                    }
                }

                return result;
            }
        }

        public float TotalLength
        {
            get
            {
                if (totalLength == 0u)
                {
                    if (BundleLengths != null)
                    {
                        for (int i = 0; i < BundleLengths.Count; i++)
                        {
                            totalLength += BundleLengths[i];
                        }
                    }
                }

                return totalLength;
            }
        }
        
        public event Action<CatalogsAsyncOperation> OnCompleted
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

    public enum UpdateCatalogsStatus
    {
        CollectCatalogs,
        DownloadBundles,
        DeleteRedundancy,
        NetworkError
    }
}
using System;
using System.Collections.Generic;

namespace GameFramework
{
    public class CatalogsAsyncOperation
    {
        internal List<BundleCatalog> Catalogs;
        internal long CurrentDownloadLength;
        private bool isDone;
        private long length;
        private long totalLength;
        private int downloadCount;
        private List<long> bundleLengths;
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

                if (TotalLength > 0)
                {
                    return Length / (float)TotalLength;
                }

                return 0f;
            }
        }

        public long Length
        {
            get
            {
                if (bundleLengths == null)
                {
                    return 0L;
                }

                if (downloadCount >= bundleLengths.Count)
                {
                    return length;
                }

                return length + CurrentDownloadLength;
            }
        }

        public long TotalLength
        {
            get { return totalLength; }
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

        internal void AddLength(long length)
        {
            if (bundleLengths == null)
            {
                bundleLengths = new List<long>();
            }
            
            bundleLengths.Add(length);
            totalLength += length;
        }

        internal void AddDownload()
        {
            length += bundleLengths[downloadCount++];
            CurrentDownloadLength = 0;
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
using UnityEngine;

namespace GameFramework
{
    public class UICell : PoolObject
    {
        private RectTransform rectTransform;

        public int Index { get; internal set; }

        public RectTransform RectTransform
        {
            get
            {
                if (rectTransform == null)
                {
                    rectTransform = transform as RectTransform;
                }

                return rectTransform;
            }
        }

        public T Cast<T>() where T : UICell
        {
            return this as T;
        }
    }
}
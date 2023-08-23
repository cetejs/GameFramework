using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public class DelayedActionManager : PersistentSingleton<DelayedActionManager>
    {
        private SortedList<DelegateInfo> delegateInfos = new SortedList<DelegateInfo>();
        private Queue<DelegateInfo>[] actions = { new Queue<DelegateInfo>(), new Queue<DelegateInfo>() };
        private int collectionIndex;

        public int AddAction(Action action, float delay = 0f)
        {
            return InternalAddAction(action, delay);
        }

        public int AddAction<T>(Action<T> action, T item, float delay = 0f)
        {
            return InternalAddAction(action, delay, item);
        }

        public int AddAction<T1, T2>(Action<T1, T2> action, T1 item1, T2 item2, float delay = 0f)
        {
            return InternalAddAction(action, delay, item1, item2);
        }

        public int AddAction<T1, T2, T3>(Action<T1, T2, T3> action, T1 item1, T2 item2, T3 item3, float delay = 0f)
        {
            return InternalAddAction(action, delay, item1, item2, item3);
        }

        public int AddAction<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 item1, T2 item2, T3 item3, T4 item4, float delay = 0f)
        {
            return InternalAddAction(action, delay, item1, item2, item3, item4);
        }

        public int AddAction<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action, T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, float delay = 0f)
        {
            return InternalAddAction(action, delay, item1, item2, item3, item4, item5);
        }

        public int AddAction<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> action, T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, float delay = 0f)
        {
            return InternalAddAction(action, delay, item1, item2, item3, item4, item5, item6);
        }

        public int AddAction<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> action, T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, float delay = 0f)
        {
            return InternalAddAction(action, delay, item1, item2, item3, item4, item5, item6, item7);
        }

        public int AddAction<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> action, T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, T8 item8, float delay = 0f)
        {
            return InternalAddAction(action, delay, item1, item2, item3, item4, item5, item6, item7, item8);
        }

        public int AddAction<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> action, T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, T8 item8, T9 item9, float delay = 0f)
        {
            return InternalAddAction(action, delay, item1, item2, item3, item4, item5, item6, item7, item8, item9);
        }

        public void RemoveAction(int id)
        {
            LinkedListNode<DelegateInfo> current = delegateInfos.First;
            while (current != null)
            {
                if (current.Value.Id == id)
                {
                    delegateInfos.Remove(current);
                    break;
                }

                current = current.Next;
            }
        }

        public override string ToString()
        {
            string result = "";
            bool first = true;
            foreach (DelegateInfo info in delegateInfos)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    result += "\n";
                }

                result += info;
            }

            return result;
        }

        private int InternalAddAction(Delegate action, float delay = 0f, params object[] param)
        {
            DelegateInfo del = new DelegateInfo(action, Time.unscaledTime + delay, param);
            if (delay > 0f)
            {
                delegateInfos.Add(del);
            }
            else
            {
                actions[collectionIndex].Enqueue(del);
            }

            return del.Id;
        }

        public void LateUpdate()
        {
            int iterationCount = 0;
            while (delegateInfos.Count > 0 && delegateInfos.First.Value.InvocationTime <= Time.unscaledTime)
            {
                actions[collectionIndex].Enqueue(delegateInfos.RemoveFirst());
            }

            do
            {
                int invokeIndex = collectionIndex;
                collectionIndex = (collectionIndex + 1) % 2;
                Queue<DelegateInfo> queue = actions[invokeIndex];
                while (queue.Count > 0)
                {
                    queue.Dequeue().Invoke();
                }

                iterationCount++;
                GameLogger.Assert(iterationCount < 100);
            } while (actions[collectionIndex].Count > 0);
        }
    }
}
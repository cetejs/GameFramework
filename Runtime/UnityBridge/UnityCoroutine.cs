using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public class UnityCoroutine : PersistentSingleton<UnityCoroutine>
    {
        private Dictionary<string, Coroutine> coroutines = new Dictionary<string, Coroutine>();

        public Coroutine Begin(IEnumerator routine)
        {
            return StartCoroutine(routine);
        }

        public Coroutine Begin(string hash, IEnumerator routine)
        {
            if (!Application.isPlaying)
            {
                GameLogger.LogError("Please execute in run mode");
                return null;
            }

            Stop(hash);
            Coroutine coroutine = StartCoroutine(routine);
            coroutines[hash] = coroutine;
            return coroutine;
        }

        public void Stop(IEnumerator routine)
        {
            StopCoroutine(routine);
        }

        public void Stop(Coroutine routine)
        {
            StopCoroutine(routine);
        }

        public void Stop(string hash)
        {
            if (coroutines.TryGetValue(hash, out Coroutine coroutine))
            {
                if (coroutine != null)
                {
                    StopCoroutine(coroutine);
                }

                coroutines.Remove(hash);
            }
        }

        public void StopAll()
        {
            StopAllCoroutines();
            coroutines.Clear();
        }
    }
}
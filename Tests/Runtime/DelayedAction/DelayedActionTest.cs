using System.Collections;
using GameFramework.DelayedActionService;
using GameFramework.Generic;
using UnityEngine;
using UnityEngine.TestTools;

public class DelayedActionTest
{
    [UnityTest]
    public IEnumerator UnityTest()
    {
        DelayedActionManager manager = Global.GetService<DelayedActionManager>();
        manager.AddAction(() =>
        {
            Debug.Log("first frame");
        });

        int lastId = -1;

        for (int i = 0; i < 10; i++)
        {
            int delay = i;
            lastId = manager.AddAction(() =>
            {
                Debug.Log($"call at {delay}s");
            }, i);
        }

        manager.AddAction(a =>
        {
            Debug.Log($"call args {a} at 1s");
        }, 1, 1.0f);

        manager.AddAction((a, b) =>
        {
            Debug.Log($"call args {a} {b} at 2s");
        }, 1, 2, 2.0f);

        manager.AddAction((a, b, c) =>
        {
            Debug.Log($"call args {a} {b} {c} at 3f");
        }, 1, 2, 3, 3.0f);

        manager.AddAction((a, b, c, d) =>
        {
            Debug.Log($"call args {a} {b} {c} {d} at 4s");
        }, 1, 2, 3, 4, 4.0f);

        manager.AddAction((a, b, c, d, e) =>
        {
            Debug.Log($"call args {a} {b} {c} {d} {e} at 5f");
        }, 1, 2, 3, 4, 5, 5.0f);

        manager.AddAction((a, b, c, d, e, f) =>
        {
            Debug.Log($"call args {a} {b} {c} {d} {e} {f} at 6f");
        }, 1, 2, 3, 4, 5, 6, 6.0f);

        manager.AddAction((a, b, c, d, e, f, g) =>
        {
            Debug.Log($"call args {a} {b} {c} {d} {e} {f} {g} at 7f");
        }, 1, 2, 3, 4, 5, 6, 7, 7.0f);

        manager.AddAction((a, b, c, d, e, f, g, h) =>
        {
            Debug.Log($"call args {a} {b} {c} {d} {e} {f} {g} {h} at 8f");
        }, 1, 2, 3, 4, 5, 6, 7, 8, 8.0f);

        manager.AddAction((a, b, c, d, e, f, g, h, i) =>
        {
            Debug.Log($"call args {a} {b} {c} {d} {e} {f} {g} {h} {i} at 9f");
        }, 1, 2, 3, 4, 5, 6, 7, 8, 9, 9.0f);

        yield return new WaitForSeconds(5.0f);
        
        manager.RemoveAction(lastId);

        yield return new WaitForSeconds(5.0f);
    }
}
using System.Collections;
using GameFramework.Generic;
using UnityEngine;
using UnityEngine.TestTools;

public class TimeTriggerTest
{
    [UnityTest]
    public IEnumerator UnityTest()
    {
        TimeTrigger trigger = 1.0f;
        bool isTriggerChange = false;
        trigger.Set(true);
        float duration = 3.0f;
        Debug.Log($"{duration} {trigger.value}");
        while (duration > 0)
        {
            duration -= Time.deltaTime;
            if (!trigger && !isTriggerChange)
            {
                isTriggerChange = true;
                Debug.Log($"{duration} {trigger.value}");
            }

            yield return null;
        }
    }
}
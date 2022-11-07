using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.Generic
{
    public class TimeTriggerAttribute : Service
    {
        private readonly Dictionary<int, TriggerData> triggers = new Dictionary<int, TriggerData>();

        public bool Get(int id)
        {
            return GetTrigger(id).value;
        }

        public void Set(int id, bool value, float life)
        {
            GetTrigger(id).Set(value, life);
        }

        private TriggerData GetTrigger(int id)
        {
            if (!triggers.TryGetValue(id, out TriggerData trigger))
            {
                trigger = new TriggerData();
                triggers.Add(id, trigger);
            }

            return trigger;
        }

        private void Update()
        {
            foreach (TriggerData trigger in triggers.Values)
            {
                trigger.Update();
            }
        }

        private class TriggerData
        {
            public bool value;
            public float life;

            public void Set(bool value, float life)
            {
                this.value = value;
                if (value)
                {
                    this.life = life;
                }
                else
                {
                    this.life = -1.0f;
                }
            }

            public void Update()
            {
                if (life > 0)
                {
                    life -= Time.deltaTime;
                }
                else if (value)
                {
                    value = false;
                }
            }
        }
    }

    public readonly struct TimeTrigger
    {
        private static int autoKey;

        public readonly int key;

        public readonly float life;

        public bool value
        {
            get
            {
                if (key == 0)
                {
                    GameLogger.LogError("Trigger is not initialized");
                    return false;
                }

                return Global.GetService<TimeTriggerAttribute>().Get(key);
            }
        }

        public TimeTrigger(float life)
        {
            key = ++autoKey;
            this.life = life;
        }

        public void Set(bool value)
        {
            if (key == 0)
            {
                GameLogger.LogError("Trigger is not initialized");
                return;
            }

            Global.GetService<TimeTriggerAttribute>().Set(key, value, life);
        }

        public static implicit operator TimeTrigger(float life)
        {
            return new TimeTrigger(life);
        }

        public static implicit operator bool(TimeTrigger trigger)
        {
            return trigger.value;
        }
    }
}
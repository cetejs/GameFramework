using System;
using UnityEngine.EventSystems;

namespace GameFramework.InputService
{
    public class CustomInput : BaseInput
    {
        public Func<string, bool> onGetButtonDown;
        public Func<string, float> onGetAxisRaw;

        public override float GetAxisRaw(string axisName)
        {
            if (onGetButtonDown == null)
            {
                return base.GetAxisRaw(axisName);
            }

            return onGetAxisRaw.Invoke(axisName);
        }

        public override bool GetButtonDown(string buttonName)
        {
            if (onGetButtonDown == null)
            {
                return base.GetButtonDown(buttonName);
            }

            return onGetButtonDown.Invoke(buttonName);
        }
    }
}
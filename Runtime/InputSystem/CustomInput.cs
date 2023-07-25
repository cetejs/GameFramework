using System;
using UnityEngine.EventSystems;

namespace GameFramework
{
    internal class CustomInput : BaseInput
    {
        public Func<string, bool> OnGetButtonDown;
        public Func<string, float> OnGetAxisRaw;

        public override float GetAxisRaw(string axisName)
        {
            if (OnGetAxisRaw == null)
            {
                return base.GetAxisRaw(axisName);
            }

            return OnGetAxisRaw.Invoke(axisName);
        }

        public override bool GetButtonDown(string buttonName)
        {
            if (OnGetButtonDown == null)
            {
                return base.GetButtonDown(buttonName);
            }

            return OnGetButtonDown.Invoke(buttonName);
        }
    }
}
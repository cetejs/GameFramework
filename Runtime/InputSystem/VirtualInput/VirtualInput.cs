using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    internal abstract class VirtualInput
    {
        private static Dictionary<string, KeyCode> KeyCodes = new Dictionary<string, KeyCode>();

        public abstract float GetAxis(InputMapping input);

        public abstract float GetAxisRaw(InputMapping input);

        public abstract bool GetButton(InputMapping input);

        public abstract bool GetButtonDown(InputMapping input);

        public abstract bool GetButtonUp(InputMapping input);

        public abstract void SetAxis(string name, float value);

        public abstract void SetButtonDown(string name);

        public abstract void SetButtonUp(string name);

        protected KeyCode ConvertToKeyCode(string name)
        {
            if (KeyCodes.TryGetValue(name, out KeyCode keyCode))
            {
                return keyCode;
            }

            if (Enum.TryParse(typeof(KeyCode), name, out object keyCodeObj))
            {
                keyCode = (KeyCode) keyCodeObj;
                KeyCodes.Add(name, keyCode);
            }

            return keyCode;
        }
    }
}
using UnityEngine;

namespace GameFramework.InputService
{
    public class StandaloneInput : VirtualInput
    {
        public override float GetAxis(InputMapping input)
        {
            return Input.GetAxis(input.keyboard.ToString());
        }

        public override float GetAxisRaw(InputMapping input)
        {
            return Input.GetAxisRaw(input.keyboard.ToString());
        }

        public override bool GetButton(InputMapping input)
        {
            if (input.keyboard == Keyboard.Mouse0)
            {
                return Input.GetMouseButton(0);
            }

            if (input.keyboard == Keyboard.Mouse1)
            {
                return Input.GetMouseButton(1);
            }

            if (input.keyboard == Keyboard.Mouse2)
            {
                return Input.GetMouseButton(2);
            }

            return Input.GetKey((KeyCode) input.keyboard);
        }

        public override bool GetButtonDown(InputMapping input)
        {
            if (input.keyboard == Keyboard.Mouse0)
            {
                return Input.GetMouseButtonDown(0);
            }

            if (input.keyboard == Keyboard.Mouse1)
            {
                return Input.GetMouseButtonDown(1);
            }

            if (input.keyboard == Keyboard.Mouse2)
            {
                return Input.GetMouseButtonDown(2);
            }
            
            return Input.GetKeyDown((KeyCode) input.keyboard);
        }

        public override bool GetButtonUp(InputMapping input)
        {
            if (input.keyboard == Keyboard.Mouse0)
            {
                return Input.GetMouseButtonUp(0);
            }

            if (input.keyboard == Keyboard.Mouse1)
            {
                return Input.GetMouseButtonUp(1);
            }

            if (input.keyboard == Keyboard.Mouse2)
            {
                return Input.GetMouseButtonUp(2);
            }
            
            return Input.GetKeyUp((KeyCode) input.keyboard);
        }

        public override void SetAxis(string name, float value)
        {
        }

        public override void SetButtonDown(string name)
        {
        }

        public override void SetButtonUp(string name)
        {
        }
    }
}
using UnityEngine;

namespace GameFramework
{
    internal class VirtualButton
    {
        private string name;
        private int lastPressedFrame = -1;
        private int releasedFrame = -1;
        private bool isPressed;

        public VirtualButton(string name)
        {
            this.name = name;
        }

        public void Pressed()
        {
            if (!isPressed)
            {
                isPressed = true;
                lastPressedFrame = Time.frameCount;
            }
        }

        public void Released()
        {
            isPressed = false;
            releasedFrame = Time.frameCount;
        }

        public bool GetButton()
        {
            return isPressed;
        }

        public bool GetButtonDown()
        {
            return lastPressedFrame - Time.frameCount == -1;
        }

        public bool GetButtonUp()
        {
            return releasedFrame - Time.frameCount == -1;
        }
    }
}
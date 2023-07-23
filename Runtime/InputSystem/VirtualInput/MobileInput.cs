using System.Collections.Generic;

namespace GameFramework
{
    internal class MobileInput : VirtualInput
    {
        private Dictionary<string, VirtualAxis> virtualAxes = new Dictionary<string, VirtualAxis>(16);
        private Dictionary<string, VirtualButton> virtualButtons = new Dictionary<string, VirtualButton>(32);

        public override float GetAxis(InputMapping input)
        {
            return GetVirtualAxis(input).GetAxis();
        }

        public override float GetAxisRaw(InputMapping input)
        {
            return GetVirtualAxis(input).GetAxisRaw();
        }

        public override bool GetButton(InputMapping input)
        {
            return GetVirtualButton(input).GetButton();
        }

        public override bool GetButtonDown(InputMapping input)
        {
            return GetVirtualButton(input).GetButtonDown();
        }

        public override bool GetButtonUp(InputMapping input)
        {
            return GetVirtualButton(input).GetButtonUp();
        }

        public override void SetAxis(string name, float value)
        {
            GetVirtualAxis(name).Update(value);
        }

        public override void SetButtonDown(string name)
        {
            GetVirtualButton(name).Pressed();
        }

        public override void SetButtonUp(string name)
        {
            GetVirtualButton(name).Released();
        }

        private VirtualAxis GetVirtualAxis(InputMapping input)
        {
            return GetVirtualAxis(input.Name);
        }

        private VirtualAxis GetVirtualAxis(string name)
        {
            if (!virtualAxes.TryGetValue(name, out VirtualAxis axis))
            {
                axis = new VirtualAxis(name);
                virtualAxes.Add(name, axis);
            }

            return axis;
        }

        private VirtualButton GetVirtualButton(InputMapping input)
        {
            return GetVirtualButton(input.Name);
        }

        private VirtualButton GetVirtualButton(string name)
        {
            if (!virtualButtons.TryGetValue(name, out VirtualButton button))
            {
                button = new VirtualButton(name);
                virtualButtons.Add(name, button);
            }

            return button;
        }
    }
}
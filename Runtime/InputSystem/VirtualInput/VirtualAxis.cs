namespace GameFramework
{
    internal class VirtualAxis
    {
        private string name;
        private float value;

        public VirtualAxis(string name)
        {
            this.name = name;
        }

        public void Update(float value)
        {
            this.value = value;
        }

        public float GetAxis()
        {
            return value;
        }

        public float GetAxisRaw()
        {
            if (value > 0f)
            {
                return 1f;
            }

            if (value < 0f)
            {
                return -1f;
            }

            return value;
        }
    }
}
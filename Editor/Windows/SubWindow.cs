namespace GameFramework
{
    internal abstract class SubWindow
    {
        public string Name;
        public GameWindow Parent;

        public virtual void Init(string name, GameWindow parent)
        {
            Name = name;
            Parent = parent;
        }

        public virtual void OnGUI()
        {
        }

        public virtual void OnDestroy()
        {
        }
    }
}
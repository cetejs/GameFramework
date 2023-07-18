namespace GameFramework
{
    public abstract class GameData : IReference
    {
        void IReference.Clear()
        {
        }
    }

    public class GameData<T> : GameData, IReference
    {
        public T Item;

        void IReference.Clear()
        {
            Item = default;
        }

        public override string ToString()
        {
            return $"item = {Item}";
        }
    }

    public class GameData<T1, T2> : GameData, IReference
    {
        public T1 Item1;
        public T2 Item2;

        void IReference.Clear()
        {
            Item1 = default;
            Item2 = default;
        }

        public override string ToString()
        {
            return $"item1 = {Item1}, item2 = {Item2}";
        }
    }

    public class GameData<T1, T2, T3> : GameData, IReference
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;

        void IReference.Clear()
        {
            Item1 = default;
            Item2 = default;
            Item3 = default;
        }

        public override string ToString()
        {
            return $"item1 = {Item1}, item2 = {Item2}, item3 = {Item3}";
        }
    }

    public class GameData<T1, T2, T3, T4> : GameData, IReference
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public T4 Item4;

        void IReference.Clear()
        {
            Item1 = default;
            Item2 = default;
            Item3 = default;
            Item4 = default;
        }

        public override string ToString()
        {
            return $"item1 = {Item1}, item2 = {Item2}, item3 = {Item3}, item4 = {Item4}";
        }
    }

    public class GameData<T1, T2, T3, T4, T5> : GameData, IReference
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public T4 Item4;
        public T5 Item5;

        void IReference.Clear()
        {
            Item1 = default;
            Item2 = default;
            Item3 = default;
            Item4 = default;
            Item5 = default;
        }

        public override string ToString()
        {
            return $"item1 = {Item1}, item2 = {Item2}, item3 = {Item3}, item4 = {Item4}, item5 = {Item5}";
        }
    }

    public class GameData<T1, T2, T3, T4, T5, T6> : GameData, IReference
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public T4 Item4;
        public T5 Item5;
        public T6 Item6;

        void IReference.Clear()
        {
            Item1 = default;
            Item2 = default;
            Item3 = default;
            Item4 = default;
            Item5 = default;
            Item6 = default;
        }

        public override string ToString()
        {
            return $"item1 = {Item1}, item2 = {Item2}, item3 = {Item3}, item4 = {Item4}, item5 = {Item5}, item6 = {Item6}";
        }
    }

    public class GameData<T1, T2, T3, T4, T5, T6, T7> : GameData, IReference
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public T4 Item4;
        public T5 Item5;
        public T6 Item6;
        public T7 Item7;

        void IReference.Clear()
        {
            Item1 = default;
            Item2 = default;
            Item3 = default;
            Item4 = default;
            Item5 = default;
            Item6 = default;
            Item7 = default;
        }

        public override string ToString()
        {
            return $"item1 = {Item1}, item2 = {Item2}, item3 = {Item3}, item4 = {Item4}, item5 = {Item5}, item6 = {Item6}, item7 = {Item7}";
        }
    }

    public class GameData<T1, T2, T3, T4, T5, T6, T7, T8> : GameData, IReference
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public T4 Item4;
        public T5 Item5;
        public T6 Item6;
        public T7 Item7;
        public T8 Item8;

        void IReference.Clear()
        {
            Item1 = default;
            Item2 = default;
            Item3 = default;
            Item4 = default;
            Item5 = default;
            Item6 = default;
            Item7 = default;
            Item8 = default;
        }

        public override string ToString()
        {
            return $"item1 = {Item1}, item2 = {Item2}, item3 = {Item3}, item4 = {Item4}, item5 = {Item5}, item6 = {Item6}, item7 = {Item7}, item8 = {Item8}";
        }
    }

    public class GameData<T1, T2, T3, T4, T5, T6, T7, T8, T9> : GameData, IReference
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public T4 Item4;
        public T5 Item5;
        public T6 Item6;
        public T7 Item7;
        public T8 Item8;
        public T9 Item9;

        void IReference.Clear()
        {
            Item1 = default;
            Item2 = default;
            Item3 = default;
            Item4 = default;
            Item5 = default;
            Item6 = default;
            Item7 = default;
            Item8 = default;
            Item9 = default;
        }

        public override string ToString()
        {
            return $"item1 = {Item1}, item2 = {Item2}, item3 = {Item3}, item4 = {Item4}, item5 = {Item5}, item6 = {Item6}, item7 = {Item7}, item8 = {Item8}, item9 = {Item9}";
        }
    }
}
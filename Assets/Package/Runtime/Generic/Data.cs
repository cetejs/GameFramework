namespace GameFramework.Generic
{
    public abstract class Data : IReference
    {
        public abstract void Clear();
    }

    public class Data<T> : Data
    {
        public T item;

        public override void Clear()
        {
            item = default;
        }

        public override string ToString()
        {
            return $"item = {item}";
        }
    }

    public class Data<T1, T2> : Data
    {
        public T1 item1;
        public T2 item2;

        public override void Clear()
        {
            item1 = default;
            item2 = default;
        }

        public override string ToString()
        {
            return $"item1 = {item1}, item2 = {item2}";
        }
    }

    public class Data<T1, T2, T3> : Data
    {
        public T1 item1;
        public T2 item2;
        public T3 item3;

        public override void Clear()
        {
            item1 = default;
            item2 = default;
            item3 = default;
        }

        public override string ToString()
        {
            return $"item1 = {item1}, item2 = {item2}, item3 = {item3}";
        }
    }

    public class Data<T1, T2, T3, T4> : Data
    {
        public T1 item1;
        public T2 item2;
        public T3 item3;
        public T4 item4;

        public override void Clear()
        {
            item1 = default;
            item2 = default;
            item3 = default;
            item4 = default;
        }

        public override string ToString()
        {
            return $"item1 = {item1}, item2 = {item2}, item3 = {item3}, item4 = {item4}";
        }
    }

    public class Data<T1, T2, T3, T4, T5> : Data
    {
        public T1 item1;
        public T2 item2;
        public T3 item3;
        public T4 item4;
        public T5 item5;

        public override void Clear()
        {
            item1 = default;
            item2 = default;
            item3 = default;
            item4 = default;
            item5 = default;
        }

        public override string ToString()
        {
            return $"item1 = {item1}, item2 = {item2}, item3 = {item3}, item4 = {item4}, item5 = {item5}";
        }
    }

    public class Data<T1, T2, T3, T4, T5, T6> : Data
    {
        public T1 item1;
        public T2 item2;
        public T3 item3;
        public T4 item4;
        public T5 item5;
        public T6 item6;

        public override void Clear()
        {
            item1 = default;
            item2 = default;
            item3 = default;
            item4 = default;
            item5 = default;
            item6 = default;
        }

        public override string ToString()
        {
            return $"item1 = {item1}, item2 = {item2}, item3 = {item3}, item4 = {item4}, item5 = {item5}, item6 = {item6}";
        }
    }

    public class Data<T1, T2, T3, T4, T5, T6, T7> : Data
    {
        public T1 item1;
        public T2 item2;
        public T3 item3;
        public T4 item4;
        public T5 item5;
        public T6 item6;
        public T7 item7;

        public override void Clear()
        {
            item1 = default;
            item2 = default;
            item3 = default;
            item4 = default;
            item5 = default;
            item6 = default;
            item7 = default;
        }

        public override string ToString()
        {
            return $"item1 = {item1}, item2 = {item2}, item3 = {item3}, item4 = {item4}, item5 = {item5}, item6 = {item6}, item7 = {item7}";
        }
    }

    public class Data<T1, T2, T3, T4, T5, T6, T7, T8> : Data
    {
        public T1 item1;
        public T2 item2;
        public T3 item3;
        public T4 item4;
        public T5 item5;
        public T6 item6;
        public T7 item7;
        public T8 item8;

        public override void Clear()
        {
            item1 = default;
            item2 = default;
            item3 = default;
            item4 = default;
            item5 = default;
            item6 = default;
            item7 = default;
            item8 = default;
        }

        public override string ToString()
        {
            return $"item1 = {item1}, item2 = {item2}, item3 = {item3}, item4 = {item4}, item5 = {item5}, item6 = {item6}, item7 = {item7}, item8 = {item8}";
        }
    }

    public class Data<T1, T2, T3, T4, T5, T6, T7, T8, T9> : Data
    {
        public T1 item1;
        public T2 item2;
        public T3 item3;
        public T4 item4;
        public T5 item5;
        public T6 item6;
        public T7 item7;
        public T8 item8;
        public T9 item9;

        public override void Clear()
        {
            item1 = default;
            item2 = default;
            item3 = default;
            item4 = default;
            item5 = default;
            item6 = default;
            item7 = default;
            item8 = default;
            item9 = default;
        }

        public override string ToString()
        {
            return $"item1 = {item1}, item2 = {item2}, item3 = {item3}, item4 = {item4}, item5 = {item5}, item6 = {item6}, item7 = {item7}, item8 = {item8}, item9 = {item9}";
        }
    }
}
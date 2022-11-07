/* AutoGenerate */

using GameFramework.DataTableService;

namespace GameFramework
{
    public class Test2 : IDataTable
    {
        public int Id { get; private set; }
        public bool Item1 { get; private set; }
        public byte Item2 { get; private set; }
        public short Item3 { get; private set; }
        public int Item4 { get; private set; }
        public long Item5 { get; private set; }
        public decimal Item6 { get; private set; }
        public float Item7 { get; private set; }
        public double Item8 { get; private set; }
        public bool[] Item9 { get; private set; }
        public byte[] Item10 { get; private set; }
        public short[] Item11 { get; private set; }
        public int[] Item12 { get; private set; }
        public long[] Item13 { get; private set; }
        public decimal[] Item14 { get; private set; }
        public float[] Item15 { get; private set; }
        public double[] Item16 { get; private set; }

        public void Read(string input)
        {
            string[] columns = input.Split(",");
            Id = int.Parse(columns[0]);
            Item1 = byte.Parse(columns[1]) > 0;
            Item2 = byte.Parse(columns[2]);
            Item3 = short.Parse(columns[3]);
            Item4 = int.Parse(columns[4]);
            Item5 = long.Parse(columns[5]);
            Item6 = decimal.Parse(columns[6]);
            Item7 = float.Parse(columns[7]);
            Item8 = double.Parse(columns[8]);
            string[] item9Array = columns[9].Split('|');
            Item9 = new bool[item9Array.Length];
            for (int j = 0; j < item9Array.Length; j++)
            {
                Item9[j] = byte.Parse(item9Array[j]) > 0;
            }

            string[] item10Array = columns[10].Split('|');
            Item10 = new byte[item10Array.Length];
            for (int j = 0; j < item10Array.Length; j++)
            {
                Item10[j] = byte.Parse(item10Array[j]);
            }

            string[] item11Array = columns[11].Split('|');
            Item11 = new short[item11Array.Length];
            for (int j = 0; j < item11Array.Length; j++)
            {
                Item11[j] = short.Parse(item11Array[j]);
            }

            string[] item12Array = columns[12].Split('|');
            Item12 = new int[item12Array.Length];
            for (int j = 0; j < item12Array.Length; j++)
            {
                Item12[j] = int.Parse(item12Array[j]);
            }

            string[] item13Array = columns[13].Split('|');
            Item13 = new long[item13Array.Length];
            for (int j = 0; j < item13Array.Length; j++)
            {
                Item13[j] = long.Parse(item13Array[j]);
            }

            string[] item14Array = columns[14].Split('|');
            Item14 = new decimal[item14Array.Length];
            for (int j = 0; j < item14Array.Length; j++)
            {
                Item14[j] = decimal.Parse(item14Array[j]);
            }

            string[] item15Array = columns[15].Split('|');
            Item15 = new float[item15Array.Length];
            for (int j = 0; j < item15Array.Length; j++)
            {
                Item15[j] = float.Parse(item15Array[j]);
            }

            string[] item16Array = columns[16].Split('|');
            Item16 = new double[item16Array.Length];
            for (int j = 0; j < item16Array.Length; j++)
            {
                Item16[j] = double.Parse(item16Array[j]);
            }
        }

        public override string ToString()
        {
            return $"Id = {Id}; Item1 = {Item1}; Item2 = {Item2}; Item3 = {Item3}; Item4 = {Item4}; Item5 = {Item5}; Item6 = {Item6}; Item7 = {Item7}; Item8 = {Item8}; Item9 = {string.Join(", ", Item9)}; Item10 = {string.Join(", ", Item10)}; Item11 = {string.Join(", ", Item11)}; Item12 = {string.Join(", ", Item12)}; Item13 = {string.Join(", ", Item13)}; Item14 = {string.Join(", ", Item14)}; Item15 = {string.Join(", ", Item15)}; Item16 = {string.Join(", ", Item16)};";
        }
    }
}
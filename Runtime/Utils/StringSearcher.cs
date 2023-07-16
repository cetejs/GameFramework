using System;

namespace GameFramework
{
    public static class StringSearcher
    {
        private static int[] bc;
        private static int[] suffix;
        private static bool[] prefix;

        private static void ClearIntArray(int[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = -1;
            }
        }

        private static bool GenerateBc(string pattern)
        {
            if (bc == null)
            {
                bc = new int[256];
            }

            ClearIntArray(bc);

            for (int i = 0; i < pattern.Length; i++)
            {
                if (pattern[i] > 255)
                {
                    return false;
                }

                bc[pattern[i]] = i;
            }

            return true;
        }

        private static void GenerateGs(string pattern)
        {
            int m = pattern.Length;
            if (suffix == null)
            {
                suffix = new int[m];
            }

            if (m > suffix.Length)
            {
                Array.Resize(ref suffix, m);
            }

            if (prefix == null)
            {
                prefix = new bool[m];
            }

            if (m > prefix.Length)
            {
                Array.Resize(ref prefix, m);
            }

            for (int i = 0; i < m; i++)
            {
                suffix[i] = -1;
                prefix[i] = false;
            }

            for (int i = 0; i < m - 1; i++)
            {
                int j = i;
                int k = 0;
                while (j >= 0 && pattern[j] == pattern[m - 1 - k])
                {
                    suffix[++k] = j--;
                }

                if (j == -1)
                {
                    prefix[k] = true;
                }
            }
        }

        private static int MoveByGs(int j, string pattern)
        {
            int m = pattern.Length;
            int k = m - 1 - j;
            if (suffix[k] != -1)
            {
                return j - suffix[k] + 1;
            }

            for (int r = j + 2; r < m; r++)
            {
                if (prefix[m - r])
                {
                    return r;
                }
            }

            return m;
        }

        public static int SearchOfBm(string text, string pattern, bool isIgnoreCase)
        {
            if (string.IsNullOrEmpty(text) || pattern == null)
            {
                return -1;
            }

            if (pattern == string.Empty)
            {
                return 0;
            }

            if (isIgnoreCase)
            {
                text = text.ToLower();
                pattern = pattern.ToLower();
            }

            if (!GenerateBc(pattern))
            {
                return -1;
            }

            GenerateGs(pattern);

            int i = 0;
            int n = text.Length;
            int m = pattern.Length;
            while (i <= n - m)
            {
                int j;
                for (j = m - 1; j >= 0; j--)
                {
                    if (text[i + j] != pattern[j])
                    {
                        break;
                    }
                }

                if (j < 0)
                {
                    return i;
                }

                int x = j - bc[text[i + j]];
                int y = 0;
                if (j < m - 1)
                {
                    y = MoveByGs(j, pattern);
                }

                i += Math.Max(x, y);
            }

            return -1;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTMLfile.CustomFeatures
{
    public class StrFeatures
    {

        private string Str { get; set; }

        public StrFeatures(string str)
        {
            Str = str;
        }

        // Split methods
        public string[] Split(char symbol)
        {
            var parts = new string[OccurencesCount(symbol) + 1];
            var index = 0;

            foreach (var t in Str)
            {
                if (t == symbol)
                {
                    index++;
                    continue;
                }
                parts[index] += t;
            }

            return parts;
        }

        public string[] Split(string delimiter)
        {
            var parts = new List<string>();
            var currentPart = "";
            var i = 0;

            while (i < Str.Length)
            {
                if (i + delimiter.Length <= Str.Length && Str.Substring(i, delimiter.Length) == delimiter)
                {
                    parts.Add(currentPart);
                    currentPart = "";
                    i += delimiter.Length;
                }
                else
                {
                    currentPart += Str[i];
                    i++;
                }
            }
            parts.Add(currentPart);

            return parts.ToArray();
        }

        public static string[] Split(string str, char delimiter)
        {
            var parts = new string[OccurencesCount(str, delimiter) + 1];
            var index = 0;

            foreach (var t in str)
            {
                if (t == delimiter)
                {
                    index++;
                    continue;
                }
                parts[index] += t;
            }

            return parts;
        }

        // Join strings
        //public static string Join(List<string> parts, string delimiter)
        //{
        //    var result = "";
        //    for (var i = 0; i < parts.Count; i++)
        //    {
        //        result += parts[i];
        //        if (i != parts.Count - 1)
        //            result += delimiter;
        //    }

        //    return result;
        //}

        //count occurrences
        private int OccurencesCount(char ch)
        {
            var count = 0;
            foreach (var c in Str)
            {
                if (c == ch)
                    count++;
            }

            return count;
        }

        private static int OccurencesCount(string str, char character)
        {
            var count = 0;
            foreach (var c in str)
            {
                if (c == character)
                    count++;
            }

            return count;
        }

        // Substrings
        public string Substring(int startIndex, int endIndex)
        {
            if (endIndex > Str.Length)
                throw new IndexOutOfRangeException("The length of the substring exceeds the length of the string.");
            if (startIndex < 0)
                throw new IndexOutOfRangeException("The start index of the substring is less than 0.");

            var result = "";
            for (var i = startIndex; i < endIndex; i++)
            {
                result += Str[i];
            }

            return result;
        }

        public string Substring(int startIndex)
        {
            if (startIndex < 0 || startIndex >= Str.Length)
                throw new IndexOutOfRangeException("The start index is out of range.");

            var result = "";
            for (var i = startIndex; i < Str.Length; i++)
            {
                result += Str[i];
            }

            return result;
        }

        public int Length()
        {
            return Str.Length;
        }

        //?contains a substring
        public bool Contains(string substr)
        {
            return FindFirstOccurrence(substr) != -1;
        }

        //first occurrence
        public int FindFirstOccurrence(string substr)
        {
            for (var i = 0; i < Str.Length; i++)
            {
                if (Str[i] != substr[0]) continue;
                var found = true;
                for (var j = 1; j < substr.Length; j++)
                {
                    if (Str[i + j] == substr[j]) continue;
                    found = false;
                    break;
                }

                if (found)
                {
                    return i;
                }
            }

            return -1;
        }

        //last occurrence
        public int FindLastOccurrence(string substr)
        {
            for (var i = Str.Length - substr.Length; i >= 0; i--)
            {
                if (Str[i] != substr[0]) continue;
                var found = true;
                for (var j = 1; j < substr.Length; j++)
                {
                    if (Str[i + j] == substr[j]) continue;
                    found = false;
                    break;
                }

                if (found) return i;
            }
            return -1;
        }

        public string ToUpper()
        {
            var result = Str.ToCharArray();
            for (var i = 0; i < Str.Length; i++)
            {
                if (Str[i] >= 'a' && Str[i] <= 'z')
                {
                    result[i] = (char)(Str[i] - 32);
                }
            }

            return new string(result);
        }

        public string ToLower()
        {
            var result = Str.ToCharArray();
            for (var i = 0; i < Str.Length; i++)
            {
                if (Str[i] >= 'A' && Str[i] <= 'Z')
                {
                    result[i] = (char)(Str[i] + 32);
                }
            }

            return new string(result);
        }

        public override string ToString()
        {
            return Str;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Utilities
{
    public class NaturalStringComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            string[] xParts = Regex.Split(x.Replace(" ", ""), "([0-9]+)");
            string[] yParts = Regex.Split(y.Replace(" ", ""), "([0-9]+)");

            int minLength = Math.Min(xParts.Length, yParts.Length);
            for (int i = 0; i < minLength; i++)
            {
                if (xParts[i] != yParts[i])
                {
                    if (int.TryParse(xParts[i], out int xNum) && int.TryParse(yParts[i], out int yNum))
                    {
                        return xNum.CompareTo(yNum);
                    }
                    else
                    {
                        return string.Compare(xParts[i], yParts[i], StringComparison.Ordinal);
                    }
                }
            }

            return xParts.Length.CompareTo(yParts.Length);
        }
    }
}

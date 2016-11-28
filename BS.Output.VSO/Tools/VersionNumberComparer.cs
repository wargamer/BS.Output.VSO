using System.Collections.Generic;
using System.Linq;

namespace BS.Output.VSO.Tools
{
    /// <summary>
    /// Compares strings by attempting to parse them as whole numbers of valid version numbers. <br />
    /// Uses default string comparison in all other cases. <br />
    /// </summary>
    public class VersionNumberComparer : IComparer<string>
    {
        private const char VersionNumberSeperator = '.';

        public int Compare(string x, string y)
        {
            int xInt, yInt;

            if (int.TryParse(x, out xInt) && int.TryParse(y, out yInt))
            {
                return Comparer<int>.Default.Compare(xInt, yInt);
            }

            if (!x.Contains(VersionNumberSeperator) || !y.Contains(VersionNumberSeperator))
            {
                return Comparer<string>.Default.Compare(x, y);
            }

            var xBits = x.Split(VersionNumberSeperator);
            var yBits = y.Split(VersionNumberSeperator);

            var defaultIntComparer = Comparer<int>.Default;
            for (int i = 0; i < xBits.Length; i++)
            {
                if (i >= yBits.Length)
                    return 1; // x represents a higher version

                int xBit, yBit;
                if (!int.TryParse(xBits[i], out xBit) || !int.TryParse(yBits[i], out yBit))
                {
                    // Not a valid version number
                    return Comparer<string>.Default.Compare(x, y);
                }

                int result = defaultIntComparer.Compare(xBit, yBit);
                if (result != 0)
                    return result;
            }

            return xBits.Length < yBits.Length
                ? -1 // x represents a lower version
                : 0; // Equal 
        }
    }
}
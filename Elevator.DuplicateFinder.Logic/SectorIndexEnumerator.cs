using System;
using System.Collections.Generic;

namespace Elevator.DuplicateFinder.Logic
{
    public class TriangleSectorIndexEnumerator : IIndexEnumerator2D
    {
        public IEnumerable<Tuple<int, int>> GetIndices(int sector, int length)
        {
            int half = length / 2;

            switch (sector)
            {
                case 0:
                    for (int y = 0; y < half; ++y)
                    {
                        for (int x = y + 1; x < half; ++x)
                        {
                            yield return new Tuple<int, int>(y, x);
                        }
                    }
                    yield break;
                case 1:
                    for (int y = 0; y < half; ++y)
                    {
                        for (int x = y + half + 1; x < length; ++x)
                        {
                            yield return new Tuple<int, int>(y, x);
                        }
                    }
                    yield break;
                case 2:
                    for (int y = 0; y < half; ++y)
                    {
                        for (int x = half; x <= y + half; ++x)
                        {
                            yield return new Tuple<int, int>(y, x);
                        }
                    }
                    yield break;
                case 3:
                    for (int y = half; y < length; ++y)
                    {
                        for (int x = y + 1; x < length; ++x)
                        {
                            yield return new Tuple<int, int>(y, x);
                        }
                    }
                    yield break;
            }
        }
    }
}

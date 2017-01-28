using System;
using System.Collections.Generic;

namespace Elevator.DuplicateFinder.Logic
{
    public interface IIndexEnumerator2D
    {
        IEnumerable<Tuple<int, int>> GetIndices(int sector, int length);
    }
}

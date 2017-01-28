using System.IO;

namespace Elevator.DuplicateFinder.Logic
{
    public class FileInfoEqualityComparer
    {
        public bool Equals(FileInfo x, FileInfo y)
        {
            return
                x.Name == y.Name &&
                x.LastWriteTime == y.LastWriteTime &&
                x.Length == y.Length;
        }

        public int GetHashCode(FileInfo obj)
        {
            return obj.GetHashCode();
        }
    }
}

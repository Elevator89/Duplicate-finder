using Elevator.DuplicateFinder.Logic;
using System;
using System.IO;
using System.Linq;

namespace DuplicateFinder
{
    class Program
    {
        internal delegate void CompareRoutine(FileInfo[] files, int sector, object locker, string outputFilePath);

        static void Main(string[] args)
        {
            if (args.Length < 2)
                return;

            string inputDirectoryPath = args[0];
            string outputFilePath = args[1];

            if (!Directory.Exists(inputDirectoryPath))
            {
                Console.WriteLine("Directory \"{0}\" does not exist", inputDirectoryPath);
                return;
            }

            Console.WriteLine("Scanning directory \"{0}\"...", inputDirectoryPath);

            FileInfo[] fileInfos = Directory.EnumerateFiles(inputDirectoryPath, "*.*", SearchOption.AllDirectories).Select(filePath => new FileInfo(filePath)).ToArray();

            Console.WriteLine("Scan complete");

            CompareRoutine sector0 = CompareSector;
            CompareRoutine sector1 = CompareSector;
            CompareRoutine sector2 = CompareSector;
            CompareRoutine sector3 = CompareSector;

            object locker = new object();

            IAsyncResult sector0Result = sector0.BeginInvoke(fileInfos, 0, locker, outputFilePath, null, null);
            IAsyncResult sector1Result = sector1.BeginInvoke(fileInfos, 1, locker, outputFilePath, null, null);
            IAsyncResult sector2Result = sector2.BeginInvoke(fileInfos, 2, locker, outputFilePath, null, null);
            IAsyncResult sector3Result = sector3.BeginInvoke(fileInfos, 3, locker, outputFilePath, null, null);

            sector0.EndInvoke(sector0Result);
            sector1.EndInvoke(sector1Result);
            sector2.EndInvoke(sector2Result);
            sector3.EndInvoke(sector3Result);

            Console.WriteLine("Compare complete!");
        }

        static void CompareFiles(Tuple<FileInfo, FileInfo>[] comparisons, object locker, string outputFilePath)
        {
            FileInfoEqualityComparer comparer = new FileInfoEqualityComparer();

            int period = 10000;

            for (int i = 0; i < comparisons.Length; ++i)
            {
                CompareFilesAndWriteResult(comparisons[i].Item1, comparisons[i].Item2, comparer, locker, outputFilePath);

                if (i % period == 0)
                {
                    Console.WriteLine("{0} of {1} complete", i, comparisons.Length);
                }
            }
        }

        static void CompareSector(FileInfo[] files, int sector, object locker, string outputFilePath)
        {
            FileInfoEqualityComparer comparer = new FileInfoEqualityComparer();

            int half = files.Length / 2;
            int totalWork = half * half / 2;
            int period = totalWork / 100;

            int processed = 0;
            IIndexEnumerator2D enumerator = new TriangleSectorIndexEnumerator();

            foreach (Tuple<int, int> indices in enumerator.GetIndices(sector, files.Length))
            {
                CompareFilesAndWriteResult(files[indices.Item1], files[indices.Item2], comparer, locker, outputFilePath);

                processed++;

                if (processed % period == 0)
                {
                    Console.WriteLine("{0} of {1} complete", processed, totalWork);
                }
            }
        }

        static void CompareFilesAndWriteResult(FileInfo x, FileInfo y, FileInfoEqualityComparer comparer, object locker, string outputFilePath)
        {
            if (comparer.Equals(x, y))
            {
                string outputLine = string.Format("{0}\t{1}", x.FullName, y.FullName);
                Console.WriteLine(outputLine);
                lock (locker)
                {
                    File.AppendAllLines(outputFilePath, new string[] { outputLine });
                }
            }
        }
    }
}

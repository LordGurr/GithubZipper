using Ionic.Zip;
using System;
using System.IO;

//using System.IO.Compression;

namespace GithubZipper
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //Directory.CreateDirectory("ZipStorer");
            //System.IO.Compression.ZipFile.CreateFromDirectory("ExampleFiles", "ZipStorer", System.IO.Compression.CompressionLevel.Optimal, false);
            //int segmentsCreated;
            //using (ZipFile zip = new ZipFile())
            //{
            //    //zip.UseUnicodeAsNecessary = true;  // utf-8
            //    zip.AddDirectory("ExampleFiles");
            //    zip.Comment = "This zip was created at " + System.DateTime.Now.ToString("G");
            //    zip.MaxOutputSegmentSize = 99614720; // 100m segments
            //    zip.Save("ZipStorer/MyFiles.zip");

            //    segmentsCreated = zip.NumberOfSegmentsForMostRecentSave;
            //}
            EmptyFolder("ZipStorer");
            CopyFiles("ExampleFiles", "ZipStorer");
        }

        private static void EmptyFolder(string path)
        {
            string[] files = Directory.GetFiles(path);
            for (int i = 0; i < files.Length; i++)
            {
                File.Delete(files[i]);
            }
            string[] directories = Directory.GetDirectories(path);
            for (int i = 0; i < directories.Length; i++)
            {
                try
                {
                    DirectoryInfo dir = new DirectoryInfo(directories[i]);
                    EmptyFolder(directories[i]);
                    setAttributesNormal(dir);
                    dir.Delete(false);
                    Directory.Delete(directories[i]);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        private static void setAttributesNormal(DirectoryInfo dir)
        {
            foreach (var subDir in dir.GetDirectories())
            {
                setAttributesNormal(subDir);
                subDir.Attributes = FileAttributes.Normal;
            }
            foreach (var file in dir.GetFiles())
            {
                file.Attributes = FileAttributes.Normal;
            }
        }

        private static void CopyFiles(string pathFrom, string pathToo)
        {
            string[] files = Directory.GetFiles(pathFrom);
            for (int i = 0; i < files.Length; i++)
            {
                if (new FileInfo(files[i]).Length > 99000000)
                {
                    ZipFile(files[i], pathToo + "/" + files[i].Replace(pathFrom, ""));
                }
                else
                {
                    File.Copy(files[i], pathToo + "/" + files[i].Substring(files[i].LastIndexOf('\\')));
                }
            }
            string[] directories = Directory.GetDirectories(pathFrom);
            for (int i = 0; i < directories.Length; i++)
            {
                Directory.CreateDirectory(pathToo + "/" + directories[i].Substring(directories[i].LastIndexOf('\\')));
                //string fullPath = Path.GetFullPath(pathToo + directories[i].Substring(directories[i].Length - pathFrom.Length));
                //Environment.CurrentDirectory = pathFrom;
                //CopyFiles(directories[i].Replace(pathFrom + '\\', ""), fullPath);

                CopyFiles(directories[i], pathToo + directories[i].Substring(directories[i].IndexOf('\\')));
            }
        }

        private static void ZipFile(string pathFrom, string pathToo)
        {
            int segmentsCreated;
            using (ZipFile zip = new ZipFile())
            {
                //zip.UseUnicodeAsNecessary = true;  // utf-8
                zip.AddFile(pathFrom);
                zip.Comment = "This zip was created at " + System.DateTime.Now.ToString("G");
                zip.MaxOutputSegmentSize = 99614720; // 100m segments
                zip.Save(pathToo);
                segmentsCreated = zip.NumberOfSegmentsForMostRecentSave;
            }
            string from = pathToo.Remove(pathToo.LastIndexOf('\\')) + pathFrom.Substring(pathFrom.LastIndexOf('\\'));
            string to = pathToo.Remove(pathToo.LastIndexOf('\\')) + pathFrom.Substring(pathFrom.LastIndexOf('\\'), pathFrom.LastIndexOf('.') - pathFrom.LastIndexOf('\\')) + ".zip";
            File.Move(from, to);
        }
    }
}
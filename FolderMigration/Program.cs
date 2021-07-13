using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace FolderMigration
{
    class Program
    {
        public static long sourceFileCount { get; set; }
        public static long totalFilesCoppied = 0;

        static void Main(string[] args)
        {

            string userName = Environment.UserName;
            string sharePath = Path.GetFullPath(@"\\fsb01.blues.healthnow.local\users\" + userName);
            string userOneDriveBasePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\OneDrive - healthnow.org";
            string currentDate = DateTime.Now.ToString("MM_dd_yyyy_HHmm");
            string foldertoCreate = @"HDriveFilesCopied_" + currentDate;
            string fullnewPath = $"{userOneDriveBasePath}\\{foldertoCreate}" ; // update string with what directory you want

                      


            //check if backup already has been ran
            Console.WriteLine("Checking for previous ran copies");
            string[] oneDriveSubDir = Directory.GetDirectories(userOneDriveBasePath);
            int dirFoundCount = 0;
            foreach (string subdirectory in oneDriveSubDir)
            {

                if (subdirectory.Contains("HDriveFilesCopied_"))
                {
                    Console.WriteLine("");
                    Console.WriteLine("Found: " + subdirectory);
                    Console.WriteLine("");
                    dirFoundCount++;
                }
            }
            if (dirFoundCount > 0)
            {                      
                    if (Confirmation())
                    {
                        Directory.CreateDirectory(fullnewPath);
                  
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("Transfer Previously Completed");
                        Console.WriteLine("Press Enter key To Exit.");
                        Console.ReadLine();
                        return;

                    }
                

            }
            
            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine("Transfering files for " + userName);
            Console.WriteLine("");
            sourceFileCount = GetDirectoryFileCount(sharePath);
            DirectoryCopy(sharePath, fullnewPath);

            Console.Clear();
            Console.WriteLine($"Transfer Complete; {sourceFileCount} files transfered");        
            Console.WriteLine("Press Enter key To Exit.");
            Console.ReadLine();

        }

        public static bool Confirmation()
        {
            bool resp = false;
            ConsoleKey response;
            do
            {
                
                Console.WriteLine("Run Copy Again? Yes(y) , No(n)");
                response = Console.ReadKey(false).Key;
                if (response != ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    Console.SetCursorPosition(5, Console.CursorTop - 1);
                    ClearCurrentConsoleLine();
                    Console.SetCursorPosition(6, Console.CursorTop - 1);
                    ClearCurrentConsoleLine();
                }
                else
                {
                    Console.SetCursorPosition(5, Console.CursorTop - 1);
                    ClearCurrentConsoleLine();

                }

            } while (response != ConsoleKey.Y && response != ConsoleKey.N);

            resp = response == ConsoleKey.Y;

            return resp;
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs = true)
        {
            

            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            
            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                
                
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
                totalFilesCoppied++;
                drawTextProgressBar(totalFilesCoppied, sourceFileCount, "Files Copied");
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }

            
            
        }

        private static void drawTextProgressBar(long progress, long total, string label = "")
        {
            int percentage = (int)Math.Round(((double)progress / (double)total) * 100);
            
            Console.CursorVisible = false;
            //draw empty progress bar
            Console.CursorLeft = 0;
            Console.Write("["); //start
            Console.CursorLeft = 32;
            Console.Write("]"); //end
            Console.CursorLeft = 1;
            float onechunk = 30.0f / total;

            //draw filled part
            int position = 1;
            for (int i = 0; i < onechunk * progress; i++)
            {
                Console.BackgroundColor = ConsoleColor.Green;
                Console.CursorLeft = position++;
                Console.Write(" ");
            }

            //draw unfilled part
            for (int i = position; i <= 31; i++)
            {
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.CursorLeft = position++;
                Console.Write(" ");
            }

            //draw totals
            Console.CursorLeft = 35;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write($"{progress.ToString()} of {total}; {percentage}% {label}   "); //blanks at the end remove any excess
        }

        public static long GetDirectoryFileCount(string path)
        {
            long finalSize = 0;
            
            string[] subDirectories = Directory.GetDirectories(path);

            finalSize +=  Directory.GetFiles(path).Length;

            foreach (string subDir in subDirectories)
            {
                finalSize += GetDirectoryFileCount(subDir);

            }

            return finalSize;
        }

        public static void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }

    }
}

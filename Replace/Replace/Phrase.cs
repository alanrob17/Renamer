// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Phrase.cs" company="Software Inc.">
//   A.Robson
// </copyright>
// <summary>
//   Replace a phrase in every file within a directory structure.
//   I had a list of files with a website's url at the start of each file.
//   This program was written to remove the url and can be used to remove 
//   any phrase from a set of files.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Replace
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// The program.
    /// </summary>
    public class Phrase
    {
        /// <summary>
        /// Main entry point.
        /// </summary>
        /// <param name="args">The args.</param>
        public static int Main(string[] args)
        {
            var argList = GetArguments(args);

            if (argList.Phrase.Contains("Error:"))
            {
                PrintErrorMessage(argList.Phrase);

                return 1;
            }

            var fileDirectory = Environment.CurrentDirectory + @"\";

            var fileList = GetFileList(fileDirectory, argList.SubFolder);
            var count = 0;

            var items = CreateItems(fileList, argList.Phrase);
                
            if (argList.ChangeFileName)
            {
                ChangeFileNames(items);
            }

            WriteReport(items);

            Console.WriteLine("Finished...");

            return 0;
        }

        /// <summary>
        /// Create the items objects.
        /// </summary>
        /// <param name="fileList">The file list.</param>
        /// <param name="phrase">set of words to be removed.</param>
        /// <returns>The <see cref="List"/> items.</returns>
        private static List<Item> CreateItems(IEnumerable<string> fileList, string phrase)
        {
            var count = 0;
            var itemList = new List<Item>();

            foreach (var item in fileList.Select(file => new Item { ItemId = ++count, Name = file, ChangeName = file.Replace(phrase, string.Empty) }))
            {
                item.Changed = item.Name != item.ChangeName;

                itemList.Add(item);
            }

            return itemList;
        }

        /// <summary>
        /// Change filenames.
        /// </summary>
        /// <param name="items">The file items.</param>
        private static void ChangeFileNames(IEnumerable<Item> items)
        {
            foreach (var item in items)
            {
                if (item.Changed)
                {
                    try
                    {
                        File.Move(item.Name, item.ChangeName);
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine(ex + "\n{0}", item.ChangeName);
                    }
                }
            }
        }

        /// <summary>
        /// Get a list of all files in a folder structure.
        /// </summary>
        /// <param name="folder">The folder name.</param>
        /// <param name="subFolders">The sub Folders.</param>
        /// <returns>A list of text files.</returns>
        private static IEnumerable<string> GetFileList(string folder, bool subFolders)
        {
            var dir = new DirectoryInfo(folder);
            var fileList = new List<string>();

            if (subFolders)
            {
                GetFiles(dir, fileList);
            }
            else
            {
                GetFiles(fileList);
            }

            return fileList;
        }

        /// <summary>
        /// Recursive list of files.
        /// </summary>
        /// <param name="d">Directory name.</param>
        /// <param name="fileList">The file List.</param>
        private static void GetFiles(DirectoryInfo d, ICollection<string> fileList)
        {
            var files = d.GetFiles("*.*");

            foreach (var fileName in files.Select(file => file.FullName))
            {
                // TODO: remove .rar and .zip extensions once I figure out how to change these filenames
                if (Path.GetExtension(fileName.ToLowerInvariant()) != ".exe" && Path.GetExtension(fileName.ToLowerInvariant()) != ".bak" && Path.GetExtension(fileName.ToLowerInvariant()) != ".log")
                {
                    fileList.Add(fileName);
                }
            }

            // get sub-folders for the current directory
            var dirs = d.GetDirectories("*.*");

            // recurse
            foreach (var dir in dirs)
            {
                // Console.WriteLine(dir.FullName);
                GetFiles(dir, fileList);
            }
        }

        /// <summary>
        /// Get list of files.
        /// </summary>
        /// <param name="fileList">The image List.</param>
        private static void GetFiles(ICollection<string> fileList)
        {
            var imageDirectory = Environment.CurrentDirectory + @"\";
            var d = new DirectoryInfo(imageDirectory);

            var files = d.GetFiles("*.*");

            foreach (var fileName in files.Select(file => file.FullName))
            {
                if (Path.GetExtension(fileName.ToLowerInvariant()) != ".exe" && Path.GetExtension(fileName.ToLowerInvariant()) != ".bak")
                {
                    fileList.Add(fileName);
                }
            }
        }

        /// <summary>
        /// Write report on what needs to be changed.
        /// </summary>
        /// <param name="items">The items.</param>
        public static void WriteReport(List<Item> items)
        {
            var outFile = Environment.CurrentDirectory + "\\alan.log";
            var outStream = File.Create(outFile);
            var sw = new StreamWriter(outStream);

            // TODO: delete the log file if it exists
            foreach (var item in items)
            {
                if (item.Changed)
                {
                    sw.WriteLine("{0}\nto\n{1}\n\n", item.Name, item.ChangeName);    
                }
            }

            // flush and close
            sw.Flush();
            sw.Close();
        }

        /// <summary>
        /// Get command line arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The <see cref="bool"/>subfolder status.</returns>
        private static ArgList GetArguments(IList<string> args)
        {
            // TODO: add multiple arguments S = subdir, W = write only, F = change folders           
            var subFolders = false;
            var changeFileNames = false;
            var changeFolders = false;
            var phrase = string.Empty;

            if (args.Count == 2)
            {
                if (args[0].ToLowerInvariant().Contains("s"))
                {
                    subFolders = true;
                }

                if (args[0].ToLowerInvariant().Contains("w"))
                {
                    changeFileNames = true;
                }

                if (args[0].ToLowerInvariant().Contains("f"))
                {
                    changeFolders = true;
                }

                if (!string.IsNullOrEmpty(args[1]))
                {
                    phrase = args[1];
                }
            }

            if (args.Count == 2)
            {
                var argList = new ArgList(subFolders, changeFileNames, changeFolders, phrase);

                return argList;
            }
            else
            {
                var arglist = new ArgList(false, false, false, "Error: Incorrect number ofr arguments.");

                return arglist;
            }
        }

        /// <summary>
        /// Print an error message if there aren't enough arguments.
        /// </summary>
        /// <param name="phrase">The phrase.</param>
        private static void PrintErrorMessage(string phrase)
        {
            Console.WriteLine("\nUsage:\n\tRenamer -sw \"phrase to remove\"\n");
            Console.WriteLine("\t\ts = sub directories and w = change filenames.\n");
        }

        /// <summary>
        /// ReplaceEX: a case insensitive replace method.
        /// </summary>
        /// <param name="original">original string</param>
        /// <param name="pattern">pattern to replace</param>
        /// <param name="replacement">replacement text</param>
        /// <returns>the modified string</returns>
        private static string ReplaceEx(string original, string pattern, string replacement)
        {
            int position0, position1;
            var count = position0 = position1 = 0;
            var upperString = original.ToUpper();
            var upperPattern = pattern.ToUpper();
            var inc = (original.Length / pattern.Length) * (replacement.Length - pattern.Length);
            var chars = new char[original.Length + Math.Max(0, inc)];
            while ((position1 = upperString.IndexOf(upperPattern, position0, StringComparison.Ordinal)) != -1)
            {
                for (var i = position0; i < position1; ++i)
                {
                    chars[count++] = original[i];
                }

                for (var i = 0; i < replacement.Length; ++i)
                {
                    chars[count++] = replacement[i];
                }

                position0 = position1 + pattern.Length;
            }

            if (position0 == 0)
            {
                return original;
            }

            for (var i = position0; i < original.Length; ++i)
            {
                chars[count++] = original[i];
            }

            return new string(chars, 0, count);
        } 
    }
}

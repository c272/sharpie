using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Text;

namespace sharpie
{
    public class Sources
    {
        public static void ManageSources(string[] args)
        {   
            //List all sources and exit.
            if (args.Length==0)
            {
                ListSources();
                Environment.Exit(0);
            }

            //Switching for source argument.
            switch (args[0])
            {
                case "add":
                    AddSource(args.Slice(1,-1));
                    break;
                case "remove":
                    RemoveSource(args.Slice(1, -1));
                    break;
                case "update":
                    UpdateSources();
                    break;
                case "clear":
                    ClearSources();
                    break;
                default:
                    Console.WriteLine("S_ERR: Invalid parameter passed to sources.\n" +
                        "For more information on sources, use \"sharpie help sources\".");
                    break;
            }
        }

        /// <summary>
        /// Lists all the sources currently in the sources.txt file.
        /// </summary>
        private static void ListSources()
        {
            Console.WriteLine("Sharpie Source Master List");
            Console.WriteLine("----------------------------");
            Console.WriteLine("FORMAT: Name | Source | Priority");

            List<Source> sources = GetSources();
            foreach (var source in sources)
            {
                Console.WriteLine(source.Name + " | " + source.Link + " | " + source.Priority);
            }

            Console.WriteLine("----------------------------");
        }

        private static void RemoveSource(string[] sources)
        {
            //Get the sources from file.
            List<Source> sourcesList = GetSources();

            //Foreach and remove the source.
            foreach (var source in sources)
            {
                if (sourcesList.RemoveAll(x => x.Name == source)==0)
                {
                    Console.WriteLine("S_ERR: Failed to remove source \"" + source + "\", does not exist in sources.txt master list.");
                } else
                {
                    Console.WriteLine("Successfully removed source \"" + source + "\".");
                }
            }

            //Writing sources back to file.
            string toWrite = "";
            foreach (var source in sourcesList)
            {
                toWrite += source.Name + "|" + source.Link + "\n";
            }

            File.WriteAllText(Constants.SourcesFile, toWrite);
        }

        private static void AddSource(string[] sources)
        {
           //Pull all the given sources, and add them to the source file.
            foreach (string source in sources)
            {
                Console.WriteLine("Attempting to add source with link \"" + source + "\".");

                //Connect and stream source.
                string srcString = "";
                try
                {
                    var client = new WebClient();
                    Stream stream = client.OpenRead(source);
                    var sr = new StreamReader(stream);

                    //Read text to memory.
                    srcString = sr.ReadToEnd();
                    sr.Close();
                } catch
                {
                    Console.WriteLine("S_ERR: Failed to pull from the given source link.\nSource not available, or an incorrect link.");
                    Environment.Exit(0);
                }

                //Getting the name of the source file and adding to sources list file.
                string sourceName = srcString.Split("\n")[0];
                //Clearing rubbish from the source name.
                sourceName = sourceName.Replace("\r", "").Replace("\n", "");
                File.AppendAllText(Constants.SourcesFile, sourceName + "|" + source + "\n");

                //Writing source contents into source directory.
                File.WriteAllText(Constants.SourcesLocation + sourceName + ".txt", srcString);
                Console.WriteLine("Successfully added source \"" + sourceName + "\".");
            }
        }

        /// <summary>
        /// Gets a list of sources from the base sources.txt file.
        /// </summary>
        /// <returns>A list of Source structs.</returns>
        public static List<Source> GetSources()
        {
            var sources = new List<Source>();

            //Iterating through lines in the sources.txt file.
            //Open file, read per line.
            var sr = new StreamReader(Constants.SourcesFile);
            string line = ""; int lineNum = 1;
            while ((line = sr.ReadLine()) != null)
            {
                //Split by name and link.
                string[] srcInfo = line.Replace(" ", "").Split("|");

                //Add to list.
                try
                {
                    sources.Add(new Source() { Name = srcInfo[0], Link = srcInfo[1], Priority = lineNum });
                } catch
                {
                    Console.WriteLine("S_ERR: Error parsing source in sources.txt, at line " + lineNum + "." +
                        "\nThis must be resolved by clearing the sources file, by using:\n" +
                        "\"sharpie sources clear\".");
                    Environment.Exit(0);
                }
            }

            sr.Close();
            return sources;
        }

        private static void UpdateSources()
        {
            throw new NotImplementedException();
        }

        private static void ClearSources()
        {
            //Write blank to the sources file.
            File.WriteAllText(Constants.SourcesFile, "");
            Console.WriteLine("Cleared all sources from the source master list.");
        }
    }

    public struct Source
    {
        public string Name;
        public string Link;
        public int Priority;
    }
}
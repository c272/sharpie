using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace sharpie
{
    public class Sources
    {
        public static void ManageSources(string[] args)
        {
            //Switching for source argument.
            switch (args[0])
            {
                case "add":
                    AddSource(args.Slice(1,-1));
                    break;
                case "remove":
                    RemoveSource(args.Slice(1, -1));
                    break;
                default:
                    Console.WriteLine("SHARPIE: Invalid parameter passed to sources.\n" +
                        "For more information on sources, use --help sources");
                    break;
            }
        }

        private static void RemoveSource(string[] sources)
        {
            throw new NotImplementedException();
        }

        private static void AddSource(string[] sources)
        {
           //Pull all the given sources, and add them to the source file.
            foreach (string source in sources)
            {
                Console.WriteLine(source);
                //Connect and stream source.
                //TODO: Catch the stream failing due to bad address.
                var client = new WebClient();
                Stream stream = client.OpenRead(source);
                var sr = new StreamReader(stream);

                //Read text to memory.
                string srcString = sr.ReadToEnd();
                sr.Close();

                //Getting the name of the source file and adding to sources list file.
                string sourceName = srcString.Split("\n")[0];
                //Clearing rubbish from the source name.
                sourceName = sourceName.Replace("\r", "").Replace("\n", "");
                File.AppendAllText(Constants.SourcesFile, sourceName + "|" + source + "\n");

                //Writing source contents into source directory.
                File.WriteAllText(Constants.SourcesLocation + sourceName + ".txt", srcString);
            }

            Console.WriteLine("Added the given sources to Sharpie.\nCount: " + sources.Length);
        }

        private static void Update()
        {
            throw new NotImplementedException();
        }
    }
}

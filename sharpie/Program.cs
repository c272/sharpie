using System;
using System.Collections.Generic;
using System.IO;

namespace sharpie
{
    class Program
    {
        public static double version = 0.1;

        static void Main(string[] args)
        {
            //No arguments, just show version.
            if (args.Length==0)
            {
                Console.WriteLine("Sharpie, v" + version + ".\nUpdate with --update, or add parameters (--help).");
                return;
            }

            //Create Sharpie directories, if not already there.
            Directory.CreateDirectory(Constants.ConfigLocation);
            Directory.CreateDirectory(Constants.SourcesLocation);

            //Create Sharpie files, if not already there.
            File.Create(Constants.SourcesFile);

            //Switching over main argument to find command.
            switch (args[0])
            {
                case "sources":
                    //Complete a task relating to package sources.
                    Sources.ManageSources(args.Slice(1, -1));
                    break;
                case "add":
                    //Install a given package.
                    Packages.AddPackage(args.Slice(1, -1));
                    break;
                case "remove":
                    //Uninstall a given package.
                    Packages.RemovePackage(args.Slice(1, -1));
                    break;
                case "config":
                    //Configure a given package.
                    Packages.ConfigPackage(args.Slice(1, -1));
                    break;
                case "--help":
                    //Log the help description, or a specific page. 
                    LogHelp(args.Slice(1, -1));
                    break;
                default:
                    //No command with that name, print error.
                    Console.WriteLine("SHARPIE: No command with the name \"" + args[0] + "\" exists.\nUse \"sharpie --help\" for more information.");
                    return;
            }
        }

        private static void LogHelp(string[] v)
        {
            throw new NotImplementedException();
        }
    }
}

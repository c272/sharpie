using System;
using System.Collections.Generic;
using System.IO;

namespace sharpie
{
    class Program
    {
        public static double version = 0.2;

        static void Main(string[] args)
        {
            //No arguments, just show version.
            if (args.Length==0)
            {
                Console.WriteLine("Sharpie, v" + version + ".\nUpdate with --update, or add parameters (help).");
                return;
            }

            //Create Sharpie directories, if not already there.
            Directory.CreateDirectory(Constants.ConfigLocation);
            Directory.CreateDirectory(Constants.SourcesLocation);
            Directory.CreateDirectory(Constants.PackagesLocation);

            //Create Sharpie files, if not already there.
            File.AppendAllText(Constants.SourcesFile, "");
       
            //Switching over main argument to find command.
            switch (args[0])
            {
                case "sources":
                    //Complete a task relating to package sources.
                    Sources.ManageSources(args.Slice(1, -1));
                    break;
                case "add":
                    //Install a given package.
                    Packages.AddPackage(args[1]);
                    break;
                case "remove":
                    //Uninstall a given package.
                    Packages.RemovePackage(args[1]);
                    break;
                case "update":
                    //Update a given package.
                    Packages.UpdatePackage(args.Slice(1, -1));
                    break;
                case "listpkg":
                    //List packages.
                    Packages.List();
                    break;
                case "help":
                    //Log the help description, or a specific page. 
                    if (args.Length > 1)
                    {
                        LogHelp(args[1]);
                    } else
                    {
                        LogHelp("");
                    }
                    break;
                default:
                    //No command with that name, print error.
                    Console.WriteLine("S_ERR: No command with the name \"" + args[0] + "\" exists.\nUse \"sharpie help\" for more information.");
                    return;
            }
        }

        private static void LogHelp(string pageOrFunc)
        {
            if (pageOrFunc=="")
            {
                //Default page, print default advice.
                Console.WriteLine("Sharpie Commands:\n" +
                    "sources - Lists all installed sources, locally and globally.\n" +
                    "sources add [link] - Adds a given source link to the source master list.\n" +
                    "sources remove [name] - Removes a given source by its name.\n" +
                    "sources update - Updates all the sources in the source master list.\n" +
                    "add [package] - Adds a given package to the local C# project, or the global PATH.\n" +
                    "remove [package] - Removes a given package from the local C# project, or the global PATH.\n" +
                    "update [package] - Updates a given package from the local C# project, or the global PATH.\n" +
                    "listpkg - Lists the installed packages locally, and on the global PATH.\n" +
                    "--update - Updates Sharpie to the latest version.");
            }
        }
    }
}

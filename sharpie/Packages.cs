using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace sharpie
{
    public class Packages
    {
        public static void ConfigPackage(string[] v)
        {
            throw new NotImplementedException();
        }

        public static void RemovePackage(string[] v)
        {
            throw new NotImplementedException();
        }

        public static void AddPackage(string[] v)
        {
            //Opening the sources file, checking for packages.
            List<Package> pkgs = GetPackages();
            foreach (var pkg in pkgs)
            {
                Console.WriteLine(pkg.Name+" | "+pkg.Link+" | "+pkg.Source);
            }
        }

        private static List<Package> GetPackages()
        {
            var packages = new List<Package>();

            //Opening file from config directory.
            string[] sources = Directory.GetFiles(Constants.SourcesLocation);
            if (sources.Length == 0)
            {
                //No sources.
                Console.WriteLine("SHARPIE: No sources to pull packages from. Add sources by using:\n" +"\"sharpie sources add [url]\"" +
                    "\nFor more information, use --help packages.");
                Environment.Exit(0);
            }

            //Reading all sources in sequence.
            foreach (var file in sources)
            {
                //Open file, read per line.
                var sr = new StreamReader(file);
                string line = "", sourceName = "";
                int lineNum = 1;
                while ((line = sr.ReadLine()) != null)
                {
                    //First line is the name of the source.
                    if (lineNum==1)
                    {
                        sourceName = line;
                        sr.Close();
                        continue;
                    }

                    //Checking if packages list already contains the given package.
                    try
                    {
                        string[] pkgInfo = line.Replace(" ", "").Split("|");
                        bool isDupePackage = false;
                        foreach (var pkg in packages)
                        {
                            if (pkg.Name == pkgInfo[0])
                            {
                                //Duplicate, don't add.
                                isDupePackage = true;
                            }
                        }

                        //Don't add package if it's a dupe, FOR NOW! Priority settings later.
                        if (isDupePackage) { sr.Close(); continue; }

                        //Add the package to the list.
                        packages.Add(new Package() { Name = pkgInfo[0], Link = pkgInfo[1], Source = sourceName });

                        sr.Close();
                        lineNum++;
                    } catch
                    {
                        //Invalid source at a given line. Error and exit.
                        var fi = new FileInfo(file);
                        Console.WriteLine("SHARPIE: Invalid package given in source \"" + fi.Name + "\", at line "+lineNum+"."
                            +"\nUpdating, editing or removing the source may fix the issue."
                        );
                        Environment.Exit(0);
                    }
                }
            }

            return packages;
        }
    }

    public struct Package
    {
        public string Name;
        public string Link;
        public string Source;
    }
}

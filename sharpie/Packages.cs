using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
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

        public static void AddPackage(string package)
        {
            //Opening the sources file, checking for packages.
            List<Package> pkgs = GetPackages();
            Console.WriteLine(pkgs.Count);
            foreach (var pkg in pkgs)
            {
                Console.WriteLine(pkg.Name+" | "+pkg.Link+" | "+pkg.Source);
            }

            //Checking if the given package exists.
            string link = "";
            PackageType pkgType = PackageType.ZIP;
            bool found = false;
            foreach (var pkg in pkgs)
            {
                if (pkg.Name==package)
                {
                    //Found it.
                    link = pkg.Link;
                    pkgType = pkg.Type;
                    found = true;
                }
            }

            //Not found.
            if (!found)
            {
                Console.WriteLine("SHARPIE: Invalid package name given, not in any source."
                    +"\nHave you added the correct sources? If so, the requested package may be unavailable."
                );
            }

            //Attempting to download file.
            using (var client = new WebClient())
            {
                if (pkgType == PackageType.ZIP)
                {
                    client.DownloadFile(link, "pkg.zip");
                }
                if (pkgType == PackageType.EXE)
                {
                    client.DownloadFile(link, Constants.PackagesLocation + package + ".exe");
                }
            }

            if (pkgType == PackageType.ZIP)
            {
                //Unzip to the packages directory within the C# project.
                string workingDir = System.Environment.CurrentDirectory;
                if (Directory.GetFiles(workingDir, "*.sln").Length == 0)
                {
                    //No solution files here, therefore no project.
                    Console.WriteLine("SHARPIE: No solution file found, cannot install package.");
                }

                //Unzip!
                string packageLoc = workingDir + "package\\" + package + "\\";
                Directory.CreateDirectory(packageLoc);
                ZipFile.ExtractToDirectory("pkg.zip", packageLoc);

                //Delete zip.
                File.Delete("pkg.zip");

                //Show success.
                Console.WriteLine("SHARPIE: Successfully installed package \"" + package + "\".");
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
                        lineNum++;
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
                        if (isDupePackage) { continue; }

                        //Don't add package if pkgInfo is the wrong length.
                        if (pkgInfo.Length!=3)
                        {
                            Console.WriteLine("Invalid amount of parameters on package in source \""+sourceName+"\".\nConsider updating or removing the source.");
                            Environment.Exit(0);
                        }

                        //Extract the type from pkgInfo[2].
                        PackageType pkgType;
                        switch (pkgInfo[2])
                        {
                            case "zip":
                                pkgType = PackageType.ZIP;
                                break;
                            case "exe":
                                pkgType = PackageType.EXE;
                                break;
                            default:
                                Console.WriteLine("Invalid type given in package \""+pkgInfo[0]+"\", source \""+sourceName+"\"."+
                                    "\nConsider updating or removing the source."
                                    );
                                Environment.Exit(0);
                                return null;
                        }

                        //Add the package to the list.
                        packages.Add(new Package() {
                            Name = pkgInfo[0],
                            Link = pkgInfo[1],
                            Source = sourceName
                        });

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

                sr.Close();
            }

            return packages;
        }
    }

    public struct Package
    {
        public string Name;
        public string Link;
        public string Source;
        public PackageType Type;
    }

    public enum PackageType
    {
        ZIP,
        EXE
    }
}

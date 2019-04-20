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
        public static void UpdatePackage(string[] packages)
        {
            //Loop over individual packages, attempt to pull from source.
            List<Package> pkgList = GetPackages();
            foreach (var package in packages)
            {
                //Check it exists in sources, and is already installed.
                int pkgIndex = pkgList.FindIndex(x => x.Name == package);
                string packageDir = Constants.WorkingDirectory + "\\packages\\" + package + "\\";
                if (pkgIndex==-1 || !Directory.Exists(packageDir))
                {
                    Console.WriteLine("S_ERR: Requested package \"" + package + "\" not installed or not available from any sources.");
                    continue;
                }
                
                //Attempt to update it.
                RemovePackage(package, true);
                AddPackage(package, true);
                Console.WriteLine("Successfully updated package \"" + package + "\".");
            }
        }

        //Removes a package from either the current project or the PATH.
        //ADD SUPPORT FOR CLI PACKAGES!!
        public static void RemovePackage(string package, bool silent=false)
        {
            //Checking if the given package is already installed.
            string packageDir = Constants.WorkingDirectory + "\\packages\\" + package + "\\";
            //Getting package from list.
            int pkgIndex = GetPackages().FindIndex(x => x.Name == package);
            if (pkgIndex==-1) { Console.WriteLine("S_ERR: Package does not exist in master list.");
                Environment.Exit(0);
            }
            Package pkgInfo = GetPackages()[pkgIndex];

            //Safechecking directory.
            bool pkgDirNonexistant = false;
            try
            {
                pkgDirNonexistant = !Directory.Exists(packageDir);
            }
            catch { }

            if (pkgDirNonexistant)
            {
                //Checking if the package is a PATH executable package.
                string exeLoc = Constants.PackagesLocation + package + ".exe";
                if (File.Exists(exeLoc))
                {
                    //Delete package.
                    try
                    {
                        File.Delete(exeLoc);
                        if (!silent)
                        {
                            Console.WriteLine("Successfully removed package \"" + package + "\".");
                        }
                    } catch
                    {
                        Console.WriteLine("S_ERR: Failed to delete package executable. Reboot and try again.");
                        Environment.Exit(0);
                    }
                }
                else
                {
                    //Not already installed, send error.
                    Console.WriteLine("S_ERR: Package is not already installed.");
                    Environment.Exit(0);
                }
            }

            //Is installed, so delete folder.
            if (pkgInfo.Type==PackageType.ZIP)
            {
                Directory.Delete(packageDir, true);
            }

            if (!silent && pkgInfo.Type==PackageType.ZIP)
            {
                Console.WriteLine("Successfully removed package \"" + package + "\" from project.");
            }
        }

        public static void AddPackage(string package, bool silent=false)
        {
            //Opening the sources file, checking for packages.
            List<Package> pkgs = GetPackages();

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
                    if (!silent)
                    {
                        Console.WriteLine("Adding package \"" + pkg.Name + "\", from source \"" + pkg.Source + "\"...");
                    }
                }
            }

            //Not found.
            if (!found)
            {
                Console.WriteLine("S_ERR: Invalid package name given, not in any source."
                    +"\nHave you added the correct sources? If so, the requested package may be unavailable."
                );
                Environment.Exit(0);
            }

            //Check if a solution file exists in the working directory.
            string workingDir = System.Environment.CurrentDirectory;
            if (Directory.GetFiles(workingDir, "*.sln").Length == 0 && pkgType == PackageType.ZIP)
            {
                //No solution files here, therefore no project.
                Console.WriteLine("S_ERR: No solution file found, cannot install package.");
                Environment.Exit(0);
            }

            //Check if the package is already installed.
            if (Directory.Exists(workingDir+"\\packages\\"+package+"\\") && pkgType == PackageType.ZIP)
            {
                Console.WriteLine("S_ERR: Package is already installed for this solution.\nUse \"sharpie update [package]\" to update.");
                Environment.Exit(0);
            }

            //Attempting to download file.
            using (var client = new WebClient())
            {
                if (pkgType == PackageType.ZIP)
                {
                    try
                    {
                        client.DownloadFile(link, "pkg.zip");
                    } catch
                    {
                        Console.WriteLine("S_ERR: Could not pull package from source, unavailable or broken.");
                        Environment.Exit(0);
                    }
                }
                if (pkgType == PackageType.EXE)
                {
                    try
                    {
                        client.DownloadFile(link, Constants.PackagesLocation + package + ".exe");
                    } catch
                    {
                        Console.WriteLine("S_ERR: Could not pull package from source, unavailable or broken.");
                        Environment.Exit(0);
                    }
                    if (!silent)
                    {
                        Console.WriteLine("Successfully installed package \"" + package + "\".");
                    }
                }
            }

            //ZIP type, so a C# package.
            if (pkgType == PackageType.ZIP)
            {
                //Unzip to the packages directory within the C# project.
                string packageLoc = workingDir + "\\packages\\" + package + "\\";
                Directory.CreateDirectory(packageLoc);
                ZipFile.ExtractToDirectory("pkg.zip", packageLoc);

                //Delete zip.
                File.Delete("pkg.zip");

                //Show success.
                if (!silent)
                {
                    Console.WriteLine("Successfully installed package \"" + package + "\".");
                }
            }
        }

        /// <summary>
        /// List all of the installed packages in PATH, and at location.
        /// </summary>
        public static void List()
        {
            var local_packages = new List<string>();
            var path_packages = new List<string>();

            //Getting packages from current directory.
            if (Directory.Exists(Constants.WorkingDirectory+"\\packages\\"))
            {
                //Grabbing.
                local_packages.AddRange(Directory.GetDirectories(Constants.WorkingDirectory + "\\packages\\"));
            }

            //Getting PATH packages.
            string[] packageFileNames = Directory.GetFiles(Constants.PackagesLocation);
            foreach (var pkg in packageFileNames)
            {
                path_packages.Add(pkg.Replace(".exe", ""));
            }

            //Printing!
            Console.WriteLine("Sharpie Packages Master List:");
            Console.WriteLine("-----------------------------");
            foreach (var pkg in path_packages)
            {
                var di = new DirectoryInfo(pkg);
                Console.WriteLine(di.Name);
            }
            if (path_packages.Count == 0) { Console.WriteLine("None installed."); }

            Console.WriteLine("-----------------------------");
            Console.WriteLine("\nSharpie Packages (Local):");
            Console.WriteLine("-----------------------------");
            foreach (var pkg in local_packages)
            {
                var di = new DirectoryInfo(pkg);
                Console.WriteLine(di.Name);
            }
            if (local_packages.Count == 0) { Console.WriteLine("None installed."); }
            Console.WriteLine("-----------------------------");
        }

        private static List<Package> GetPackages()
        {
            var packages = new List<Package>();

            //Opening file from config directory.
            string[] sources = Directory.GetFiles(Constants.SourcesLocation);
            if (sources.Length == 0)
            {
                //No sources.
                Console.WriteLine("S_ERR: No sources to pull packages from. Add sources by using:\n" +"\"sharpie sources add [url]\"" +
                    "\nFor more information, use \"sharpie help packages\".");
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
                            Source = sourceName,
                            Type = pkgType
                        });

                        lineNum++;
                    } catch
                    {
                        //Invalid source at a given line. Error and exit.
                        var fi = new FileInfo(file);
                        Console.WriteLine("S_ERR: Invalid package given in source \"" + fi.Name + "\", at line "+lineNum+"."
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

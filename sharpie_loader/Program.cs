using System;
using System.Diagnostics;
using System.Net;

namespace sharpie_loader
{
    class Program
    {
        public const int Version = 1;

        static void Main(string[] args)
        {
            //Checking if the user is requesting to update.
            if (args.Length>0 && args[0]=="--update")
            {
                //Attempting to grab from the GitHub latest directory.
                using (var client = new WebClient())
                {
                    try
                    {
                        client.DownloadFile("https://github.com/c272/sharpie/raw/master/latest/sharpie_ex.exe", "sharpie_ex.exe");
                        client.DownloadFile("https://github.com/c272/sharpie/raw/master/latest/sharpie_ex.dll", "sharpie_ex.dll");
                        Console.WriteLine("Successfully pulled the latest Sharpie build from the master update source.");
                    } catch
                    {
                        //Failed the download.
                        Console.WriteLine("S_ERR: Failed to pull from master update source, GitHub may be offline or rate limiting you. Please try again later.");
                    }
                }

                Environment.Exit(0);
            }

            //Start the child process.
            Process p = new Process();

            //Redirect the output stream of the child process.
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = "sharpie_ex.exe";
            p.StartInfo.Arguments = string.Join(" ", args);
            p.Start();

            // Do not wait for the child process to exit before
            // reading to the end of its redirected stream.
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            Console.Write(output);
        }
    }
}

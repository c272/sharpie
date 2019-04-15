using System;
using System.Diagnostics;

namespace sharpie_loader
{
    class Program
    {
        public const int Version = 1;

        static void Main(string[] args)
        {
            // Checking if the user is requesting to update.
            if (args.Length>0 && args[0]=="--update")
            {
                Console.WriteLine("UPDATE DETECTED! :)");
                Environment.Exit(0);
            }

            // Start the child process.
            Process p = new Process();

            // Redirect the output stream of the child process.
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

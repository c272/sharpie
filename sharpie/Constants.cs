using System;
using System.Collections.Generic;
using System.Text;

namespace sharpie
{
    public class Constants
    {
        //This is Windows-specific. Change for Mac and Linux.
        public static string ConfigLocation = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "sharpie\\";
        public static string SourcesLocation = ConfigLocation + "sources\\";
        public static string SourcesFile = ConfigLocation + "sources.txt";
    }
}

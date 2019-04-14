using System;
using System.Collections.Generic;
using System.Text;

namespace sharpie
{
    public class Constants
    {
        //This is Windows-specific. Change for Mac and Linux.
        public static string ConfigLocation = AppDomain.CurrentDomain.BaseDirectory + "\\sharpie\\";
        public static string SourcesLocation = ConfigLocation + "sources\\";
        public static string PackagesLocation = ConfigLocation + "packages\\";
        public static string SourcesFile = ConfigLocation + "sources.txt";
    }
}

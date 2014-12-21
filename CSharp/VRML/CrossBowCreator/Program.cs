using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossBowCreator
{
    class Program
    {
        private const string ExportFileName = "CrossBow-DeyanYosifov-M24906.wrl";

        static void Main(string[] args)
        {

            if (File.Exists(ExportFileName))
            {
                File.Delete(ExportFileName);
            }

            // TODO: Create file here

            Process.Start(ExportFileName);
        }
    }
}

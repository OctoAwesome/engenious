using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentTool.Commandline
{
    class Options
    {
        public bool Hidden { get; set; }

        public string ProjectPath { get; set; }

        public string ContentDirectory { get; set; }

        public string ConfigurationPath { get; set; }
    }
}

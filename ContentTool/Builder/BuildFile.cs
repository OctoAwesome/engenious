﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentTool.Builder
{
    [Serializable()]
    public class BuildFile
    {
        public string InputFilePath { get; private set; }
        public string OutputFilePath { get; private set; }

        public DateTime InputFileModifiedTime { get; private set; }
        public DateTime OutputFileModifiedTime { get; private set; }

        public List<string> Dependencies { get; private set; }

        public BuildFile(string inputFilePath, string outputFilePath)
        {
            InputFilePath = inputFilePath;
            OutputFilePath = outputFilePath;
            Dependencies = new List<string>();
            RefreshModifiedTime();
        }

        public BuildFile(string inputFilePath, string outputFileDirectory, string name)
            : this(inputFilePath, Path.Combine(outputFileDirectory, name)){}

        public void RefreshModifiedTime()
        {
            InputFileModifiedTime = new FileInfo(InputFilePath).LastWriteTimeUtc;

            if(File.Exists(OutputFilePath))
                OutputFileModifiedTime = new FileInfo(OutputFilePath).LastWriteTimeUtc;
        }

        public bool IsBuilt()
        {
            if (string.IsNullOrEmpty(OutputFilePath))
                return true;
            if (!File.Exists(OutputFilePath))
                return false;
            return true;
        }

        public bool NeedsRebuild()
        {
            //RefreshModifiedTime();

            if (!IsBuilt())
                return true;
            return (!File.Exists(InputFilePath) || InputFileModifiedTime != new FileInfo(InputFilePath).LastWriteTimeUtc || (OutputFilePath != null && (!File.Exists(OutputFilePath) || OutputFileModifiedTime != new FileInfo(OutputFilePath).LastWriteTimeUtc)));
                
        }
    }
}
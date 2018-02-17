using System;
using System.Collections.Generic;
using System.IO;

namespace engenious.Content.Pipeline
{
	public abstract class ContentContext : IContentContext
	{
        public event BuildMessageDel BuildMessage;

	    protected ContentContext (string workingDirectory = "")
		{
			Dependencies = new List<string> ();
			if (workingDirectory.Length > 0)
			{
				char lastChar = workingDirectory[workingDirectory.Length - 1];
				workingDirectory = (lastChar == Path.DirectorySeparatorChar || lastChar == Path.AltDirectorySeparatorChar)
					? workingDirectory
					: workingDirectory + Path.DirectorySeparatorChar;
			}

			WorkingDirectory = workingDirectory;
		}

		public List<string> Dependencies{ get; }



        public void AddDependency (string file)
		{
			Dependencies.Add (file);
		}


        public abstract void Dispose();

        public void RaiseBuildMessage(string filename,string message, BuildMessageEventArgs.BuildMessageType messageType)
        {
            BuildMessage?.Invoke(this, new BuildMessageEventArgs(filename,message, messageType));
        }
		
		public string WorkingDirectory { get; }
		
		public string GetRelativePath(string subPath)
		{
			try
			{
				var parentUri = new Uri(WorkingDirectory);
				var subUri = new Uri(subPath);
				var relUri = parentUri.MakeRelativeUri(subUri);
				return relUri.ToString();
			}
			catch (Exception ex)
			{
				return subPath;
			}
		}
    }
}


using System.Collections.Generic;

namespace engenious.Content.Pipeline
{
	public abstract class ContentContext : IContentContext
	{
        public event BuildMessageDel BuildMessage;

	    protected ContentContext ()
		{
			Dependencies = new List<string> ();
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
    }
}


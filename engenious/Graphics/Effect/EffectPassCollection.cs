using System;
using System.Collections.Generic;
using System.Collections;

namespace engenious.Graphics
{
	public sealed class EffectPassCollection //: IEnumerable<EffectPass>
	{
		public List<EffectPass> PassesList;
		private Dictionary<string,EffectPass> passes;
		public EffectPassCollection ()
		{
			PassesList = new List<EffectPass> ();
			passes = new Dictionary<string, EffectPass> ();
		}
		internal void Add(EffectPass pass)
		{
			PassesList.Add (pass);
			passes.Add (pass.Name, pass);
		}
		public EffectPass this [int index] { 
			get {
				return PassesList[index];
			} 
		}
		public EffectPass this [string name] { 
			get {
				return passes[name];
			} 
		}
	}
}


using System;
using System.Collections.Generic;
using System.Collections;

namespace engenious.Graphics
{
	public sealed class EffectPassCollection //: IEnumerable<EffectPass>
	{
		public List<EffectPass> PassesList;
		private readonly Dictionary<string,EffectPass> _passes;
		public EffectPassCollection ()
		{
			PassesList = new List<EffectPass> ();
			_passes = new Dictionary<string, EffectPass> ();
		}
		internal void Add(EffectPass pass)
		{
			PassesList.Add (pass);
			_passes.Add (pass.Name, pass);
		}
		public EffectPass this [int index] => PassesList[index];

	    public EffectPass this [string name] => _passes[name];
	}
}


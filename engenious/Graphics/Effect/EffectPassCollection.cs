using System.Collections;
using System.Collections.Generic;

namespace engenious.Graphics
{
	public sealed class EffectPassCollection : IEnumerable<EffectPass>
	{
		private readonly List<EffectPass> _passesList;
		private readonly Dictionary<string,EffectPass> _passes;
		public EffectPassCollection ()
		{
			_passesList = new List<EffectPass> ();
			_passes = new Dictionary<string, EffectPass> ();
		}
		internal void Add(EffectPass pass)
		{
			_passesList.Add (pass);
			_passes.Add (pass.Name, pass);
		}
		public EffectPass this [int index] => _passesList[index];

	    public EffectPass this [string name] => _passes[name];

		IEnumerator<EffectPass> IEnumerable<EffectPass>.GetEnumerator()
		{
			return _passesList.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _passesList.GetEnumerator();
		}

		public List<EffectPass>.Enumerator GetEnumerator()
		{
			return _passesList.GetEnumerator();
		}
	}
}


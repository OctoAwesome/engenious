using System;
using System.Collections.Generic;
using System.Collections;

namespace engenious.Graphics
{
    public sealed class EffectTechniqueCollection : IEnumerable<EffectTechnique>
    {
		private readonly Dictionary<string,EffectTechnique> _techniques;
	    private readonly List<EffectTechnique> _techniqueList;

		public EffectTechniqueCollection ()
		{
			_techniques = new Dictionary<string, EffectTechnique> ();
			_techniqueList = new List<EffectTechnique> ();
		}

		internal void Add (EffectTechnique technique)
		{
			_techniques.Add (technique.Name, technique);
			_techniqueList.Add (technique);
		}

		public EffectTechnique this [int index] => _techniqueList [index];

        public EffectTechnique this [string name] => _techniques [name];

	    IEnumerator<EffectTechnique> IEnumerable<EffectTechnique>.GetEnumerator()
	    {
		    return _techniqueList.GetEnumerator();
	    }

	    IEnumerator IEnumerable.GetEnumerator()
	    {
		    return _techniqueList.GetEnumerator();
	    }

	    public List<EffectTechnique>.Enumerator GetEnumerator()
	    {
		    return _techniqueList.GetEnumerator();
	    }
    }
}


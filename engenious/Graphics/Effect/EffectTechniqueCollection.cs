using System;
using System.Collections.Generic;
using System.Collections;

namespace engenious.Graphics
{
    public sealed class EffectTechniqueCollection : IEnumerable<EffectTechnique>
    {
		private readonly Dictionary<string,EffectTechnique> _techniques;
		public List<EffectTechnique> TechniqueList;

		public EffectTechniqueCollection ()
		{
			_techniques = new Dictionary<string, EffectTechnique> ();
			TechniqueList = new List<EffectTechnique> ();
		}

		internal void Add (EffectTechnique technique)
		{
			_techniques.Add (technique.Name, technique);
			TechniqueList.Add (technique);
		}

		public EffectTechnique this [int index] => TechniqueList [index];

        public EffectTechnique this [string name] => _techniques [name];

        [Obsolete("Use member " + nameof(TechniqueList))]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return TechniqueList.GetEnumerator();
        }

        [Obsolete("Use member " + nameof(TechniqueList))]
        public IEnumerator<EffectTechnique> GetEnumerator()
        {
            return TechniqueList.GetEnumerator();
        }
    }
}


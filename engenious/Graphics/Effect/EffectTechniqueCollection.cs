using System;
using System.Collections.Generic;
using System.Collections;

namespace engenious.Graphics
{
    public sealed class EffectTechniqueCollection : IEnumerable<EffectTechnique>
    {
		private Dictionary<string,EffectTechnique> techniques;
		public List<EffectTechnique> TechniqueList;

		public EffectTechniqueCollection ()
		{
			techniques = new Dictionary<string, EffectTechnique> ();
			TechniqueList = new List<EffectTechnique> ();
		}

		internal void Add (EffectTechnique technique)
		{
			techniques.Add (technique.Name, technique);
			TechniqueList.Add (technique);
		}

		public EffectTechnique this [int index] { 
			get {
				return TechniqueList [index];
			} 
		}

		public EffectTechnique this [string name] { 
			get {
				return techniques [name];
			} 
		}

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


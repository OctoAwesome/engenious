using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;


namespace engenious.Graphics
{
	public sealed class EffectParameterCollection : IEnumerable<EffectParameter>
	{
		private Dictionary<string,EffectParameter> parameters;
		public List<EffectParameter> ParameterList;

		public EffectParameterCollection (EffectTechniqueCollection techniques)
		{
		    using (Execute.OnUiContext)
		    {
		        parameters = new Dictionary<string, EffectParameter>();
		        ParameterList = new List<EffectParameter>();

		        foreach (EffectTechnique technique in techniques.TechniqueList)
		        {
		            foreach (EffectPass pass in technique.Passes.PassesList)
		            {
		                pass.CacheParameters();

		                foreach (EffectPassParameter param in pass.Parameters.ParameterList)
		                {
		                    EffectParameter current = null;
		                    if (!parameters.TryGetValue(param.Name, out current))
		                    {
		                        current = new EffectParameter(param.Name);
		                        Add(current);
		                    }
		                    current.Add(param);
		                }
		            }
		        }
		    }
		}

		internal void Add (EffectParameter parameter)
		{
			ParameterList.Add (parameter);
			parameters.Add (parameter.Name, parameter);
		}

		public EffectParameter this [int index] { 
			get {
				return ParameterList [index];
			} 
		}

		public EffectParameter this [string name] { 
			get {
				return parameters [name];
			} 
		}

        [Obsolete("Use member " + nameof(ParameterList))]
        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ParameterList.GetEnumerator();
        }
        [Obsolete("Use member " + nameof(ParameterList))]
        public IEnumerator<EffectParameter> GetEnumerator()
        {
            return ParameterList.GetEnumerator();
        }
    }
}


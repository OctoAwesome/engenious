using System;
using System.Collections;
using System.Collections.Generic;

namespace engenious.Graphics
{
	public sealed class EffectParameterCollection : IEnumerable<EffectParameter>
	{
		private readonly Dictionary<string,EffectParameter> _parameters;
		public List<EffectParameter> ParameterList;

		public EffectParameterCollection (EffectTechniqueCollection techniques)
		{
		    using (Execute.OnUiContext)
		    {
		        _parameters = new Dictionary<string, EffectParameter>();
		        ParameterList = new List<EffectParameter>();

		        foreach (EffectTechnique technique in techniques.TechniqueList)
		        {
		            foreach (EffectPass pass in technique.Passes.PassesList)
		            {
		                pass.CacheParameters();

		                foreach (EffectPassParameter param in pass.Parameters.ParameterList)
		                {
		                    EffectParameter current;
		                    if (!_parameters.TryGetValue(param.Name, out current))
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
			_parameters.Add (parameter.Name, parameter);
		}

		public EffectParameter this [int index] { 
			get {
				return ParameterList [index];
			} 
		}

		public EffectParameter this [string name] { 
			get {
				return _parameters [name];
			} 
		}

        [Obsolete("Use member " + nameof(ParameterList))]
        IEnumerator IEnumerable.GetEnumerator()
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


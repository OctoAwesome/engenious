using System;
using System.Collections;
using System.Collections.Generic;
using engenious.Helper;

namespace engenious.Graphics
{
	public sealed class EffectParameterCollection : IEnumerable<EffectParameter>
	{
		private readonly Dictionary<string,EffectParameter> _parameters;
		private readonly List<EffectParameter> _parameterList;

		public EffectParameterCollection (EffectTechniqueCollection techniques)
		{
		    using (Execute.OnUiContext)
		    {
		        _parameters = new Dictionary<string, EffectParameter>();
		        _parameterList = new List<EffectParameter>();

		        foreach (var technique in techniques)
		        {
		            foreach (var pass in technique.Passes)
		            {
		                pass.CacheParameters();

		                foreach (var param in pass.Parameters)
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
			_parameterList.Add (parameter);
			_parameters.Add (parameter.Name, parameter);
		}

		public EffectParameter this [int index] { 
			get {
				return _parameterList [index];
			} 
		}

		public EffectParameter this [string name] { 
			get {
				return _parameters [name];
			} 
		}

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _parameterList.GetEnumerator();
        }
        IEnumerator<EffectParameter> IEnumerable<EffectParameter>.GetEnumerator()
        {
            return _parameterList.GetEnumerator();
        }
		public List<EffectParameter>.Enumerator GetEnumerator()
		{
			return _parameterList.GetEnumerator();
		}
    }
}


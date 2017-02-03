using System;
using System.Collections;
using System.Collections.Generic;

namespace engenious.Graphics
{
	public sealed class EffectPassParameterCollection : IEnumerable<EffectPassParameter>
	{
		public List<EffectPassParameter> ParameterList;
		private readonly Dictionary<string,EffectPassParameter> _parameters;
		private readonly EffectPass _pass;

		internal EffectPassParameterCollection (EffectPass pass)
		{
			_pass = pass;
			ParameterList = new List<EffectPassParameter> ();
			_parameters = new Dictionary<string, EffectPassParameter> ();
		}

		internal void Add (EffectPassParameter parameter)
		{
			ParameterList.Add (parameter);
			_parameters.Add (parameter.Name, parameter);
		}

		private EffectPassParameter CacheParameter (string name)
		{
            EffectPassParameter param;
		    using (Execute.OnUiContext)
		    {
		        int location = _pass.GetUniformLocation(name);
		        param = new EffectPassParameter(_pass, name, location);
		    }
		    return param;
		}

		public EffectPassParameter this [int index] { 
			get {
				return ParameterList [index];
			} 
		}

		public EffectPassParameter this [string name] { 
			get {
				EffectPassParameter val;
				if (!_parameters.TryGetValue (name, out val)) {
					val = CacheParameter (name);
					_parameters.Add (name, val);
				}
				return val;
			} 
		}
        
        [Obsolete("Use member " + nameof(ParameterList))]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ParameterList.GetEnumerator();
        }

        [Obsolete("Use member " + nameof(ParameterList))]
        public IEnumerator<EffectPassParameter> GetEnumerator()
        {
            return ParameterList.GetEnumerator();
        }
    }
}


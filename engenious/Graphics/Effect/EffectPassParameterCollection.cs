using System;
using System.Collections.Generic;
using System.Collections;

namespace engenious.Graphics
{
	public sealed class EffectPassParameterCollection : IEnumerable<EffectPassParameter>
	{
		public List<EffectPassParameter> ParameterList;
		private Dictionary<string,EffectPassParameter> parameters;
		private EffectPass pass;

		internal EffectPassParameterCollection (EffectPass pass)
		{
			this.pass = pass;
			ParameterList = new List<EffectPassParameter> ();
			parameters = new Dictionary<string, EffectPassParameter> ();
		}

		internal void Add (EffectPassParameter parameter)
		{
			ParameterList.Add (parameter);
			parameters.Add (parameter.Name, parameter);
		}

		private EffectPassParameter CacheParameter (string name)
		{
            EffectPassParameter param=null;
		    using (Execute.OnUiContext)
		    {
		        int location = pass.GetUniformLocation(name);
		        param = new EffectPassParameter(pass, name, location);
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
				if (!parameters.TryGetValue (name, out val)) {
					val = CacheParameter (name);
					parameters.Add (name, val);
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


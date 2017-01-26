using System;
using System.Collections.Generic;
using System.Collections;

namespace engenious.Graphics
{
	public sealed class EffectPassParameterCollection : IEnumerable<EffectPassParameter>
	{
		private List<EffectPassParameter> parameterList;
		private Dictionary<string,EffectPassParameter> parameters;
		private EffectPass pass;

		internal EffectPassParameterCollection (EffectPass pass)
		{
			this.pass = pass;
			parameterList = new List<EffectPassParameter> ();
			parameters = new Dictionary<string, EffectPassParameter> ();
		}

		internal void Add (EffectPassParameter parameter)
		{
			parameterList.Add (parameter);
			parameters.Add (parameter.Name, parameter);
		}

		private EffectPassParameter CacheParameter (string name)
		{
            EffectPassParameter param=null;
		    using (Execute.OnUiThread)
		    {
		        int location = pass.GetUniformLocation(name);
		        param = new EffectPassParameter(pass, name, location);
		    }
		    return param;
		}

		public EffectPassParameter this [int index] { 
			get {
				return parameterList [index];
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

		IEnumerator System.Collections.IEnumerable.GetEnumerator ()
		{
			return parameterList.GetEnumerator ();
		}

		public IEnumerator<EffectPassParameter> GetEnumerator ()
		{
			return parameterList.GetEnumerator ();
		}
	}
}


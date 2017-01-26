using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;


namespace engenious.Graphics
{
	public sealed class EffectParameterCollection :IEnumerable<EffectParameter>
	{
		private Dictionary<string,EffectParameter> parameters;
		private List<EffectParameter> parameterList;

		public EffectParameterCollection (EffectTechniqueCollection techniques)
		{
		    using (Execute.OnUiThread)
		    {
		        parameters = new Dictionary<string, EffectParameter>();
		        parameterList = new List<EffectParameter>();

		        foreach (EffectTechnique technique in techniques)
		        {
		            foreach (EffectPass pass in technique.Passes)
		            {
		                pass.CacheParameters();

		                foreach (EffectPassParameter param in pass.Parameters)
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
			parameterList.Add (parameter);
			parameters.Add (parameter.Name, parameter);
		}

		public EffectParameter this [int index] { 
			get {
				return parameterList [index];
			} 
		}

		public EffectParameter this [string name] { 
			get {
				return parameters [name];
			} 
		}

		IEnumerator System.Collections.IEnumerable.GetEnumerator ()
		{
			return parameterList.GetEnumerator ();
		}

		public IEnumerator<EffectParameter> GetEnumerator ()
		{
			return parameterList.GetEnumerator ();
		}

	}
}


using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Builders
{
	public class ParameterDataProvider : IParameterDataProvider
	{
		private List<IParameterDataProvider> _providers = new List<IParameterDataProvider>();

		public ParameterDataProvider(params IParameterDataProvider[] providers)
		{
			_providers.AddRange(providers);
		}

		public bool HasDataFor(IParameterInfo parameter)
		{
			foreach (IParameterDataProvider provider in _providers)
			{
				if (provider.HasDataFor(parameter))
				{
					return true;
				}
			}
			return false;
		}

		public IEnumerable GetDataFor(IParameterInfo parameter)
		{
			foreach (IParameterDataProvider provider in _providers)
			{
				foreach (object item in provider.GetDataFor(parameter))
				{
					yield return item;
				}
			}
		}
	}
}

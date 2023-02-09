using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
	public class ParameterizedMethodSuite : TestSuite
	{
		private bool _isTheory;

		public override string TestType
		{
			get
			{
				if (_isTheory)
				{
					return "Theory";
				}
				if (base.Method.ContainsGenericParameters)
				{
					return "GenericMethod";
				}
				return "ParameterizedMethod";
			}
		}

		public ParameterizedMethodSuite(IMethodInfo method)
			: base(method.TypeInfo.FullName, method.Name)
		{
			base.Method = method;
			_isTheory = method.IsDefined<TheoryAttribute>(true);
			base.MaintainTestOrder = true;
		}
	}
}

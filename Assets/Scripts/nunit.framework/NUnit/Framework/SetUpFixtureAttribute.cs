using System;
using System.Collections.Generic;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class SetUpFixtureAttribute : NUnitAttribute, IFixtureBuilder
	{
		public IEnumerable<TestSuite> BuildFrom(ITypeInfo typeInfo)
		{
			SetUpFixture setUpFixture = new SetUpFixture(typeInfo);
			if (setUpFixture.RunState != 0)
			{
				string reason = null;
				if (!IsValidFixtureType(typeInfo, ref reason))
				{
					setUpFixture.RunState = RunState.NotRunnable;
					setUpFixture.Properties.Set("_SKIPREASON", reason);
				}
			}
			return new TestSuite[1] { setUpFixture };
		}

		private bool IsValidFixtureType(ITypeInfo typeInfo, ref string reason)
		{
			if (typeInfo.IsAbstract)
			{
				reason = string.Format("{0} is an abstract class", typeInfo.FullName);
				return false;
			}
			if (!typeInfo.HasConstructor(new Type[0]))
			{
				reason = string.Format("{0} does not have a default constructor", typeInfo.FullName);
				return false;
			}
			Type[] array = new Type[4]
			{
				typeof(SetUpAttribute),
				typeof(TearDownAttribute),
				typeof(TestFixtureSetUpAttribute),
				typeof(TestFixtureTearDownAttribute)
			};
			Type[] array2 = array;
			foreach (Type type in array2)
			{
				if (typeInfo.HasMethodWithAttribute(type))
				{
					reason = type.Name + " attribute not allowed in a SetUpFixture";
					return false;
				}
			}
			return true;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Builders
{
	public class DefaultSuiteBuilder : ISuiteBuilder
	{
		private NUnitTestFixtureBuilder _defaultBuilder = new NUnitTestFixtureBuilder();

		public bool CanBuildFrom(ITypeInfo typeInfo)
		{
			if (typeInfo.IsAbstract && !typeInfo.IsSealed)
			{
				return false;
			}
			if (typeInfo.IsDefined<IFixtureBuilder>(true))
			{
				return true;
			}
			if (typeInfo.IsGenericTypeDefinition)
			{
				return false;
			}
			return typeInfo.HasMethodWithAttribute(typeof(IImplyFixture));
		}

		public TestSuite BuildFrom(ITypeInfo typeInfo)
		{
			List<TestSuite> list = new List<TestSuite>();
			try
			{
				IFixtureBuilder[] fixtureBuilderAttributes = GetFixtureBuilderAttributes(typeInfo);
				IFixtureBuilder[] array = fixtureBuilderAttributes;
				foreach (IFixtureBuilder fixtureBuilder in array)
				{
					foreach (TestSuite item in fixtureBuilder.BuildFrom(typeInfo))
					{
						list.Add(item);
					}
				}
				if (typeInfo.IsGenericType)
				{
					return BuildMultipleFixtures(typeInfo, list);
				}
				switch (list.Count)
				{
				case 0:
					return _defaultBuilder.BuildFrom(typeInfo);
				case 1:
					return list[0];
				default:
					return BuildMultipleFixtures(typeInfo, list);
				}
			}
			catch (Exception innerException)
			{
				TestFixture testFixture = new TestFixture(typeInfo);
				testFixture.RunState = RunState.NotRunnable;
				if (innerException is TargetInvocationException)
				{
					innerException = innerException.InnerException;
				}
				string value = "An exception was thrown while loading the test." + Env.NewLine + innerException.ToString();
				testFixture.Properties.Add("_SKIPREASON", value);
				return testFixture;
			}
		}

		private TestSuite BuildMultipleFixtures(ITypeInfo typeInfo, IEnumerable<TestSuite> fixtures)
		{
			TestSuite testSuite = new ParameterizedFixtureSuite(typeInfo);
			foreach (TestSuite fixture in fixtures)
			{
				testSuite.Add(fixture);
			}
			return testSuite;
		}

		private IFixtureBuilder[] GetFixtureBuilderAttributes(ITypeInfo typeInfo)
		{
			IFixtureBuilder[] array = new IFixtureBuilder[0];
			while (typeInfo != null && !typeInfo.IsType(typeof(object)))
			{
				array = typeInfo.GetCustomAttributes<IFixtureBuilder>(false);
				if (array.Length > 0)
				{
					if (array.Length == 1)
					{
						return array;
					}
					int num = 0;
					IFixtureBuilder[] array2 = array;
					foreach (IFixtureBuilder attr in array2)
					{
						if (HasArguments(attr))
						{
							num++;
						}
					}
					if (num == array.Length)
					{
						return array;
					}
					if (num == 0)
					{
						return new IFixtureBuilder[1] { array[0] };
					}
					IFixtureBuilder[] array3 = new IFixtureBuilder[num];
					int num2 = 0;
					array2 = array;
					foreach (IFixtureBuilder attr in array2)
					{
						if (HasArguments(attr))
						{
							array3[num2++] = attr;
						}
					}
					return array3;
				}
				typeInfo = typeInfo.BaseType;
			}
			return array;
		}

		private bool HasArguments(IFixtureBuilder attr)
		{
			TestFixtureAttribute testFixtureAttribute = attr as TestFixtureAttribute;
			return testFixtureAttribute == null || testFixtureAttribute.Arguments.Length > 0 || testFixtureAttribute.TypeArgs.Length > 0;
		}
	}
}

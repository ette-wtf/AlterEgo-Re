using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public class TestFixtureAttribute : NUnitAttribute, IFixtureBuilder, ITestFixtureData, ITestData
	{
		private readonly NUnitTestFixtureBuilder _builder = new NUnitTestFixtureBuilder();

		private Type _testOf;

		public string TestName { get; set; }

		public RunState RunState { get; private set; }

		public object[] Arguments { get; private set; }

		public IPropertyBag Properties { get; private set; }

		public Type[] TypeArgs { get; set; }

		public string Description
		{
			get
			{
				return Properties.Get("Description") as string;
			}
			set
			{
				Properties.Set("Description", value);
			}
		}

		public string Author
		{
			get
			{
				return Properties.Get("Author") as string;
			}
			set
			{
				Properties.Set("Author", value);
			}
		}

		public Type TestOf
		{
			get
			{
				return _testOf;
			}
			set
			{
				_testOf = value;
				Properties.Set("TestOf", value.FullName);
			}
		}

		public string Ignore
		{
			get
			{
				return IgnoreReason;
			}
			set
			{
				IgnoreReason = value;
			}
		}

		public string Reason
		{
			get
			{
				return Properties.Get("_SKIPREASON") as string;
			}
			set
			{
				Properties.Set("_SKIPREASON", value);
			}
		}

		public string IgnoreReason
		{
			get
			{
				return Reason;
			}
			set
			{
				RunState = RunState.Ignored;
				Reason = value;
			}
		}

		public bool Explicit
		{
			get
			{
				return RunState == RunState.Explicit;
			}
			set
			{
				RunState = ((!value) ? RunState.Runnable : RunState.Explicit);
			}
		}

		public string Category
		{
			get
			{
				IList list = Properties["Category"];
				if (list == null)
				{
					return null;
				}
				switch (list.Count)
				{
				case 0:
					return null;
				case 1:
					return list[0] as string;
				default:
				{
					string[] array = new string[list.Count];
					int num = 0;
					foreach (string item in list)
					{
						array[num++] = item;
					}
					return string.Join(",", array);
				}
				}
			}
			set
			{
				string[] array = value.Split(',');
				foreach (string value2 in array)
				{
					Properties.Add("Category", value2);
				}
			}
		}

		public TestFixtureAttribute()
			: this(new object[0])
		{
		}

		public TestFixtureAttribute(params object[] arguments)
		{
			RunState = RunState.Runnable;
			Arguments = arguments;
			TypeArgs = new Type[0];
			Properties = new PropertyBag();
		}

		public IEnumerable<TestSuite> BuildFrom(ITypeInfo typeInfo)
		{
			yield return _builder.BuildFrom(typeInfo, this);
		}
	}
}

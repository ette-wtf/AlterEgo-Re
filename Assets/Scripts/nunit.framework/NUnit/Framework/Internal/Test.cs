using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
	public abstract class Test : ITest, IXmlNodeBuilder, IComparable
	{
		private static int _nextID = 1000;

		protected MethodInfo[] setUpMethods;

		protected MethodInfo[] tearDownMethods;

		protected ITypeInfo DeclaringTypeInfo;

		private IMethodInfo _method;

		public string Id { get; set; }

		public string Name { get; set; }

		public string FullName { get; set; }

		public string ClassName
		{
			get
			{
				ITypeInfo typeInfo = TypeInfo;
				if (Method != null)
				{
					if (DeclaringTypeInfo == null)
					{
						DeclaringTypeInfo = new TypeWrapper(Method.MethodInfo.DeclaringType);
					}
					typeInfo = DeclaringTypeInfo;
				}
				if (typeInfo == null)
				{
					return null;
				}
				return typeInfo.IsGenericType ? typeInfo.GetGenericTypeDefinition().FullName : typeInfo.FullName;
			}
		}

		public virtual string MethodName
		{
			get
			{
				return null;
			}
		}

		public ITypeInfo TypeInfo { get; private set; }

		public IMethodInfo Method
		{
			get
			{
				return _method;
			}
			set
			{
				DeclaringTypeInfo = null;
				_method = value;
			}
		}

		public RunState RunState { get; set; }

		public abstract string XmlElementName { get; }

		public virtual string TestType
		{
			get
			{
				return GetType().Name;
			}
		}

		public virtual int TestCaseCount
		{
			get
			{
				return 1;
			}
		}

		public IPropertyBag Properties { get; private set; }

		public bool IsSuite
		{
			get
			{
				return this is TestSuite;
			}
		}

		public abstract bool HasChildren { get; }

		public ITest Parent { get; set; }

		public abstract IList<ITest> Tests { get; }

		public virtual object Fixture { get; set; }

		public static string IdPrefix { get; set; }

		public int Seed { get; set; }

		internal bool RequiresThread { get; set; }

		protected Test(string name)
		{
			Guard.ArgumentNotNullOrEmpty(name, "name");
			Initialize(name);
		}

		protected Test(string pathName, string name)
		{
			Guard.ArgumentNotNullOrEmpty(pathName, "pathName");
			Initialize(name);
			FullName = pathName + "." + name;
		}

		protected Test(ITypeInfo typeInfo)
		{
			Initialize(typeInfo.GetDisplayName());
			string @namespace = typeInfo.Namespace;
			if (@namespace != null && @namespace != "")
			{
				FullName = @namespace + "." + Name;
			}
			TypeInfo = typeInfo;
		}

		protected Test(IMethodInfo method)
		{
			Initialize(method.Name);
			Method = method;
			TypeInfo = method.TypeInfo;
			FullName = method.TypeInfo.FullName + "." + Name;
		}

		private void Initialize(string name)
		{
			string fullName = (Name = name);
			FullName = fullName;
			Id = GetNextId();
			Properties = new PropertyBag();
			RunState = RunState.Runnable;
		}

		private static string GetNextId()
		{
			return IdPrefix + _nextID++;
		}

		public abstract TestResult MakeTestResult();

		public void ApplyAttributesToTest(ICustomAttributeProvider provider)
		{
			object[] customAttributes = provider.GetCustomAttributes(typeof(IApplyToTest), true);
			for (int i = 0; i < customAttributes.Length; i++)
			{
				IApplyToTest applyToTest = (IApplyToTest)customAttributes[i];
				applyToTest.ApplyToTest(this);
			}
		}

		protected void PopulateTestNode(TNode thisNode, bool recursive)
		{
			thisNode.AddAttribute("id", Id.ToString());
			thisNode.AddAttribute("name", Name);
			thisNode.AddAttribute("fullname", FullName);
			if (MethodName != null)
			{
				thisNode.AddAttribute("methodname", MethodName);
			}
			if (ClassName != null)
			{
				thisNode.AddAttribute("classname", ClassName);
			}
			thisNode.AddAttribute("runstate", RunState.ToString());
			if (Properties.Keys.Count > 0)
			{
				Properties.AddToXml(thisNode, recursive);
			}
		}

		public TNode ToXml(bool recursive)
		{
			return AddToXml(new TNode("dummy"), recursive);
		}

		public abstract TNode AddToXml(TNode parentNode, bool recursive);

		public int CompareTo(object obj)
		{
			Test test = obj as Test;
			if (test == null)
			{
				return -1;
			}
			return FullName.CompareTo(test.FullName);
		}
	}
}

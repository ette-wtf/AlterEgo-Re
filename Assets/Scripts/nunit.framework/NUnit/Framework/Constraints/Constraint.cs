using System;
using System.Globalization;
using System.Text;
using NUnit.Compatibility;

namespace NUnit.Framework.Constraints
{
	public abstract class Constraint : IConstraint, IResolveConstraint
	{
		private Lazy<string> _displayName;

		public virtual string DisplayName
		{
			get
			{
				return _displayName.Value;
			}
		}

		public virtual string Description { get; protected set; }

		public object[] Arguments { get; private set; }

		public ConstraintBuilder Builder { get; set; }

		public ConstraintExpression And
		{
			get
			{
				ConstraintBuilder constraintBuilder = Builder;
				if (constraintBuilder == null)
				{
					constraintBuilder = new ConstraintBuilder();
					constraintBuilder.Append(this);
				}
				constraintBuilder.Append(new AndOperator());
				return new ConstraintExpression(constraintBuilder);
			}
		}

		public ConstraintExpression With
		{
			get
			{
				return And;
			}
		}

		public ConstraintExpression Or
		{
			get
			{
				ConstraintBuilder constraintBuilder = Builder;
				if (constraintBuilder == null)
				{
					constraintBuilder = new ConstraintBuilder();
					constraintBuilder.Append(this);
				}
				constraintBuilder.Append(new OrOperator());
				return new ConstraintExpression(constraintBuilder);
			}
		}

		protected Constraint(params object[] args)
		{
			Arguments = args;
			Func<string> valueFactory = delegate
			{
				Type type = GetType();
				string text = type.Name;
				if (type.GetTypeInfo().IsGenericType)
				{
					text = text.Substring(0, text.Length - 2);
				}
				if (text.EndsWith("Constraint", StringComparison.Ordinal))
				{
					text = text.Substring(0, text.Length - 10);
				}
				return text;
			};
			_displayName = new Lazy<string>(valueFactory);
		}

		public abstract ConstraintResult ApplyTo(object actual);

		public virtual ConstraintResult ApplyTo<TActual>(ActualValueDelegate<TActual> del)
		{
			return ApplyTo(GetTestObject(del));
		}

		public virtual ConstraintResult ApplyTo<TActual>(ref TActual actual)
		{
			return ApplyTo(actual);
		}

		protected virtual object GetTestObject<TActual>(ActualValueDelegate<TActual> del)
		{
			return del();
		}

		public override string ToString()
		{
			string stringRepresentation = GetStringRepresentation();
			return (Builder == null) ? stringRepresentation : string.Format("<unresolved {0}>", stringRepresentation);
		}

		protected virtual string GetStringRepresentation()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<");
			stringBuilder.Append(DisplayName.ToLower());
			object[] arguments = Arguments;
			foreach (object o in arguments)
			{
				stringBuilder.Append(" ");
				stringBuilder.Append(_displayable(o));
			}
			stringBuilder.Append(">");
			return stringBuilder.ToString();
		}

		private static string _displayable(object o)
		{
			if (o == null)
			{
				return "null";
			}
			string format = ((o is string) ? "\"{0}\"" : "{0}");
			return string.Format(CultureInfo.InvariantCulture, format, new object[1] { o });
		}

		public static Constraint operator &(Constraint left, Constraint right)
		{
			return new AndConstraint(((IResolveConstraint)left).Resolve(), ((IResolveConstraint)right).Resolve());
		}

		public static Constraint operator |(Constraint left, Constraint right)
		{
			return new OrConstraint(((IResolveConstraint)left).Resolve(), ((IResolveConstraint)right).Resolve());
		}

		public static Constraint operator !(Constraint constraint)
		{
			return new NotConstraint(((IResolveConstraint)constraint).Resolve());
		}

		public DelayedConstraint After(int delayInMilliseconds)
		{
			return new DelayedConstraint((Builder == null) ? this : Builder.Resolve(), delayInMilliseconds);
		}

		public DelayedConstraint After(int delayInMilliseconds, int pollingInterval)
		{
			return new DelayedConstraint((Builder == null) ? this : Builder.Resolve(), delayInMilliseconds, pollingInterval);
		}

		IConstraint IResolveConstraint.Resolve()
		{
			return (Builder == null) ? this : Builder.Resolve();
		}
	}
}

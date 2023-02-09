using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using NUnit.Compatibility;

namespace NUnit.Framework.Constraints
{
	public class EqualConstraint : Constraint
	{
		private readonly object _expected;

		private Tolerance _tolerance = Tolerance.Default;

		private NUnitEqualityComparer _comparer = new NUnitEqualityComparer();

		public Tolerance Tolerance
		{
			get
			{
				return _tolerance;
			}
		}

		public bool CaseInsensitive
		{
			get
			{
				return _comparer.IgnoreCase;
			}
		}

		public bool ClipStrings { get; private set; }

		public IList<NUnitEqualityComparer.FailurePoint> FailurePoints
		{
			get
			{
				return _comparer.FailurePoints;
			}
		}

		public EqualConstraint IgnoreCase
		{
			get
			{
				_comparer.IgnoreCase = true;
				return this;
			}
		}

		public EqualConstraint NoClip
		{
			get
			{
				ClipStrings = false;
				return this;
			}
		}

		public EqualConstraint AsCollection
		{
			get
			{
				_comparer.CompareAsCollection = true;
				return this;
			}
		}

		public EqualConstraint WithSameOffset
		{
			get
			{
				_comparer.WithSameOffset = true;
				return this;
			}
		}

		public EqualConstraint Ulps
		{
			get
			{
				_tolerance = _tolerance.Ulps;
				return this;
			}
		}

		public EqualConstraint Percent
		{
			get
			{
				_tolerance = _tolerance.Percent;
				return this;
			}
		}

		public EqualConstraint Days
		{
			get
			{
				_tolerance = _tolerance.Days;
				return this;
			}
		}

		public EqualConstraint Hours
		{
			get
			{
				_tolerance = _tolerance.Hours;
				return this;
			}
		}

		public EqualConstraint Minutes
		{
			get
			{
				_tolerance = _tolerance.Minutes;
				return this;
			}
		}

		public EqualConstraint Seconds
		{
			get
			{
				_tolerance = _tolerance.Seconds;
				return this;
			}
		}

		public EqualConstraint Milliseconds
		{
			get
			{
				_tolerance = _tolerance.Milliseconds;
				return this;
			}
		}

		public EqualConstraint Ticks
		{
			get
			{
				_tolerance = _tolerance.Ticks;
				return this;
			}
		}

		public override string Description
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder(MsgUtils.FormatValue(_expected));
				if (_tolerance != null && !_tolerance.IsUnsetOrDefault)
				{
					stringBuilder.Append(" +/- ");
					stringBuilder.Append(MsgUtils.FormatValue(_tolerance.Value));
					if (_tolerance.Mode != ToleranceMode.Linear)
					{
						stringBuilder.Append(" ");
						stringBuilder.Append(_tolerance.Mode.ToString());
					}
				}
				if (_comparer.IgnoreCase)
				{
					stringBuilder.Append(", ignoring case");
				}
				return stringBuilder.ToString();
			}
		}

		public EqualConstraint(object expected)
			: base(expected)
		{
			AdjustArgumentIfNeeded(ref expected);
			_expected = expected;
			ClipStrings = true;
		}

		public EqualConstraint Within(object amount)
		{
			if (!_tolerance.IsUnsetOrDefault)
			{
				throw new InvalidOperationException("Within modifier may appear only once in a constraint expression");
			}
			_tolerance = new Tolerance(amount);
			return this;
		}

		public EqualConstraint Using(IComparer comparer)
		{
			_comparer.ExternalComparers.Add(EqualityAdapter.For(comparer));
			return this;
		}

		public EqualConstraint Using<T>(IComparer<T> comparer)
		{
			_comparer.ExternalComparers.Add(EqualityAdapter.For(comparer));
			return this;
		}

		public EqualConstraint Using<T>(Comparison<T> comparer)
		{
			_comparer.ExternalComparers.Add(EqualityAdapter.For(comparer));
			return this;
		}

		public EqualConstraint Using(IEqualityComparer comparer)
		{
			_comparer.ExternalComparers.Add(EqualityAdapter.For(comparer));
			return this;
		}

		public EqualConstraint Using<T>(IEqualityComparer<T> comparer)
		{
			_comparer.ExternalComparers.Add(EqualityAdapter.For(comparer));
			return this;
		}

		public override ConstraintResult ApplyTo(object actual)
		{
			AdjustArgumentIfNeeded(ref actual);
			return new EqualConstraintResult(this, actual, _comparer.AreEqual(_expected, actual, ref _tolerance));
		}

		private void AdjustArgumentIfNeeded<T>(ref T arg)
		{
			if (arg != null)
			{
				Type type = arg.GetType();
				Type type2 = (TypeExtensions.GetTypeInfo(type).IsGenericType ? type.GetGenericTypeDefinition() : null);
				if ((object)type2 == typeof(ArraySegment<>) && type.GetProperty("Array").GetValue(arg, null) == null)
				{
					Type elementType = type.GetGenericArguments()[0];
					Array array = Array.CreateInstance(elementType, 0);
					ConstructorInfo constructor = type.GetConstructor(new Type[1] { array.GetType() });
					arg = (T)constructor.Invoke(new object[1] { array });
				}
			}
		}
	}
}

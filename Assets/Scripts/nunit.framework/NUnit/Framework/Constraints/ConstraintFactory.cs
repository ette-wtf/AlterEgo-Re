using System;
using System.Collections;

namespace NUnit.Framework.Constraints
{
	public class ConstraintFactory
	{
		public ConstraintExpression Not
		{
			get
			{
				return Is.Not;
			}
		}

		public ConstraintExpression No
		{
			get
			{
				return Has.No;
			}
		}

		public ConstraintExpression All
		{
			get
			{
				return Is.All;
			}
		}

		public ConstraintExpression Some
		{
			get
			{
				return Has.Some;
			}
		}

		public ConstraintExpression None
		{
			get
			{
				return Has.None;
			}
		}

		public ResolvableConstraintExpression Length
		{
			get
			{
				return Has.Length;
			}
		}

		public ResolvableConstraintExpression Count
		{
			get
			{
				return Has.Count;
			}
		}

		public ResolvableConstraintExpression Message
		{
			get
			{
				return Has.Message;
			}
		}

		public ResolvableConstraintExpression InnerException
		{
			get
			{
				return Has.InnerException;
			}
		}

		public NullConstraint Null
		{
			get
			{
				return new NullConstraint();
			}
		}

		public TrueConstraint True
		{
			get
			{
				return new TrueConstraint();
			}
		}

		public FalseConstraint False
		{
			get
			{
				return new FalseConstraint();
			}
		}

		public GreaterThanConstraint Positive
		{
			get
			{
				return new GreaterThanConstraint(0);
			}
		}

		public LessThanConstraint Negative
		{
			get
			{
				return new LessThanConstraint(0);
			}
		}

		public EqualConstraint Zero
		{
			get
			{
				return new EqualConstraint(0);
			}
		}

		public NaNConstraint NaN
		{
			get
			{
				return new NaNConstraint();
			}
		}

		public EmptyConstraint Empty
		{
			get
			{
				return new EmptyConstraint();
			}
		}

		public UniqueItemsConstraint Unique
		{
			get
			{
				return new UniqueItemsConstraint();
			}
		}

		public BinarySerializableConstraint BinarySerializable
		{
			get
			{
				return new BinarySerializableConstraint();
			}
		}

		public XmlSerializableConstraint XmlSerializable
		{
			get
			{
				return new XmlSerializableConstraint();
			}
		}

		public CollectionOrderedConstraint Ordered
		{
			get
			{
				return new CollectionOrderedConstraint();
			}
		}

		public static ConstraintExpression Exactly(int expectedCount)
		{
			return Has.Exactly(expectedCount);
		}

		public ResolvableConstraintExpression Property(string name)
		{
			return Has.Property(name);
		}

		public ResolvableConstraintExpression Attribute(Type expectedType)
		{
			return Has.Attribute(expectedType);
		}

		public ResolvableConstraintExpression Attribute<TExpected>()
		{
			return Attribute(typeof(TExpected));
		}

		public EqualConstraint EqualTo(object expected)
		{
			return new EqualConstraint(expected);
		}

		public SameAsConstraint SameAs(object expected)
		{
			return new SameAsConstraint(expected);
		}

		public GreaterThanConstraint GreaterThan(object expected)
		{
			return new GreaterThanConstraint(expected);
		}

		public GreaterThanOrEqualConstraint GreaterThanOrEqualTo(object expected)
		{
			return new GreaterThanOrEqualConstraint(expected);
		}

		public GreaterThanOrEqualConstraint AtLeast(object expected)
		{
			return new GreaterThanOrEqualConstraint(expected);
		}

		public LessThanConstraint LessThan(object expected)
		{
			return new LessThanConstraint(expected);
		}

		public LessThanOrEqualConstraint LessThanOrEqualTo(object expected)
		{
			return new LessThanOrEqualConstraint(expected);
		}

		public LessThanOrEqualConstraint AtMost(object expected)
		{
			return new LessThanOrEqualConstraint(expected);
		}

		public ExactTypeConstraint TypeOf(Type expectedType)
		{
			return new ExactTypeConstraint(expectedType);
		}

		public ExactTypeConstraint TypeOf<TExpected>()
		{
			return new ExactTypeConstraint(typeof(TExpected));
		}

		public InstanceOfTypeConstraint InstanceOf(Type expectedType)
		{
			return new InstanceOfTypeConstraint(expectedType);
		}

		public InstanceOfTypeConstraint InstanceOf<TExpected>()
		{
			return new InstanceOfTypeConstraint(typeof(TExpected));
		}

		public AssignableFromConstraint AssignableFrom(Type expectedType)
		{
			return new AssignableFromConstraint(expectedType);
		}

		public AssignableFromConstraint AssignableFrom<TExpected>()
		{
			return new AssignableFromConstraint(typeof(TExpected));
		}

		public AssignableToConstraint AssignableTo(Type expectedType)
		{
			return new AssignableToConstraint(expectedType);
		}

		public AssignableToConstraint AssignableTo<TExpected>()
		{
			return new AssignableToConstraint(typeof(TExpected));
		}

		public CollectionEquivalentConstraint EquivalentTo(IEnumerable expected)
		{
			return new CollectionEquivalentConstraint(expected);
		}

		public CollectionSubsetConstraint SubsetOf(IEnumerable expected)
		{
			return new CollectionSubsetConstraint(expected);
		}

		public CollectionSupersetConstraint SupersetOf(IEnumerable expected)
		{
			return new CollectionSupersetConstraint(expected);
		}

		public CollectionContainsConstraint Member(object expected)
		{
			return new CollectionContainsConstraint(expected);
		}

		public CollectionContainsConstraint Contains(object expected)
		{
			return new CollectionContainsConstraint(expected);
		}

		public ContainsConstraint Contains(string expected)
		{
			return new ContainsConstraint(expected);
		}

		[Obsolete("Deprecated, use Contains")]
		public SubstringConstraint StringContaining(string expected)
		{
			return new SubstringConstraint(expected);
		}

		[Obsolete("Deprecated, use Contains")]
		public SubstringConstraint ContainsSubstring(string expected)
		{
			return new SubstringConstraint(expected);
		}

		[Obsolete("Deprecated, use Does.Not.Contain")]
		public SubstringConstraint DoesNotContain(string expected)
		{
			return new ConstraintExpression().Not.ContainsSubstring(expected);
		}

		public StartsWithConstraint StartWith(string expected)
		{
			return new StartsWithConstraint(expected);
		}

		public StartsWithConstraint StartsWith(string expected)
		{
			return new StartsWithConstraint(expected);
		}

		[Obsolete("Deprecated, use Does.StartWith or StartsWith")]
		public StartsWithConstraint StringStarting(string expected)
		{
			return new StartsWithConstraint(expected);
		}

		[Obsolete("Deprecated, use Does.Not.StartWith")]
		public StartsWithConstraint DoesNotStartWith(string expected)
		{
			return new ConstraintExpression().Not.StartsWith(expected);
		}

		public EndsWithConstraint EndWith(string expected)
		{
			return new EndsWithConstraint(expected);
		}

		public EndsWithConstraint EndsWith(string expected)
		{
			return new EndsWithConstraint(expected);
		}

		[Obsolete("Deprecated, use Does.EndWith or EndsWith")]
		public EndsWithConstraint StringEnding(string expected)
		{
			return new EndsWithConstraint(expected);
		}

		[Obsolete("Deprecated, use Does.Not.EndWith")]
		public EndsWithConstraint DoesNotEndWith(string expected)
		{
			return new ConstraintExpression().Not.EndsWith(expected);
		}

		public RegexConstraint Match(string pattern)
		{
			return new RegexConstraint(pattern);
		}

		public RegexConstraint Matches(string pattern)
		{
			return new RegexConstraint(pattern);
		}

		[Obsolete("Deprecated, use Does.Match or Matches")]
		public RegexConstraint StringMatching(string pattern)
		{
			return new RegexConstraint(pattern);
		}

		[Obsolete("Deprecated, use Does.Not.Match")]
		public RegexConstraint DoesNotMatch(string pattern)
		{
			return new ConstraintExpression().Not.Matches(pattern);
		}

		public SamePathConstraint SamePath(string expected)
		{
			return new SamePathConstraint(expected);
		}

		public SubPathConstraint SubPathOf(string expected)
		{
			return new SubPathConstraint(expected);
		}

		public SamePathOrUnderConstraint SamePathOrUnder(string expected)
		{
			return new SamePathOrUnderConstraint(expected);
		}

		public RangeConstraint InRange(IComparable from, IComparable to)
		{
			return new RangeConstraint(from, to);
		}
	}
}

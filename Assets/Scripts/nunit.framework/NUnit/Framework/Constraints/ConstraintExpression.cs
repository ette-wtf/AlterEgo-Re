using System;
using System.Collections;

namespace NUnit.Framework.Constraints
{
	public class ConstraintExpression
	{
		protected ConstraintBuilder builder;

		public ConstraintExpression Not
		{
			get
			{
				return Append(new NotOperator());
			}
		}

		public ConstraintExpression No
		{
			get
			{
				return Append(new NotOperator());
			}
		}

		public ConstraintExpression All
		{
			get
			{
				return Append(new AllOperator());
			}
		}

		public ConstraintExpression Some
		{
			get
			{
				return Append(new SomeOperator());
			}
		}

		public ConstraintExpression None
		{
			get
			{
				return Append(new NoneOperator());
			}
		}

		public ResolvableConstraintExpression Length
		{
			get
			{
				return Property("Length");
			}
		}

		public ResolvableConstraintExpression Count
		{
			get
			{
				return Property("Count");
			}
		}

		public ResolvableConstraintExpression Message
		{
			get
			{
				return Property("Message");
			}
		}

		public ResolvableConstraintExpression InnerException
		{
			get
			{
				return Property("InnerException");
			}
		}

		public ConstraintExpression With
		{
			get
			{
				return Append(new WithOperator());
			}
		}

		public NullConstraint Null
		{
			get
			{
				return (NullConstraint)Append(new NullConstraint());
			}
		}

		public TrueConstraint True
		{
			get
			{
				return (TrueConstraint)Append(new TrueConstraint());
			}
		}

		public FalseConstraint False
		{
			get
			{
				return (FalseConstraint)Append(new FalseConstraint());
			}
		}

		public GreaterThanConstraint Positive
		{
			get
			{
				return (GreaterThanConstraint)Append(new GreaterThanConstraint(0));
			}
		}

		public LessThanConstraint Negative
		{
			get
			{
				return (LessThanConstraint)Append(new LessThanConstraint(0));
			}
		}

		public EqualConstraint Zero
		{
			get
			{
				return (EqualConstraint)Append(new EqualConstraint(0));
			}
		}

		public NaNConstraint NaN
		{
			get
			{
				return (NaNConstraint)Append(new NaNConstraint());
			}
		}

		public EmptyConstraint Empty
		{
			get
			{
				return (EmptyConstraint)Append(new EmptyConstraint());
			}
		}

		public UniqueItemsConstraint Unique
		{
			get
			{
				return (UniqueItemsConstraint)Append(new UniqueItemsConstraint());
			}
		}

		public BinarySerializableConstraint BinarySerializable
		{
			get
			{
				return (BinarySerializableConstraint)Append(new BinarySerializableConstraint());
			}
		}

		public XmlSerializableConstraint XmlSerializable
		{
			get
			{
				return (XmlSerializableConstraint)Append(new XmlSerializableConstraint());
			}
		}

		public CollectionOrderedConstraint Ordered
		{
			get
			{
				return (CollectionOrderedConstraint)Append(new CollectionOrderedConstraint());
			}
		}

		public Constraint Exist
		{
			get
			{
				return Append(new FileOrDirectoryExistsConstraint());
			}
		}

		public ConstraintExpression()
		{
			builder = new ConstraintBuilder();
		}

		public ConstraintExpression(ConstraintBuilder builder)
		{
			this.builder = builder;
		}

		public override string ToString()
		{
			return builder.Resolve().ToString();
		}

		public ConstraintExpression Append(ConstraintOperator op)
		{
			builder.Append(op);
			return this;
		}

		public ResolvableConstraintExpression Append(SelfResolvingOperator op)
		{
			builder.Append(op);
			return new ResolvableConstraintExpression(builder);
		}

		public Constraint Append(Constraint constraint)
		{
			builder.Append(constraint);
			return constraint;
		}

		public ConstraintExpression Exactly(int expectedCount)
		{
			return Append(new ExactCountOperator(expectedCount));
		}

		public ResolvableConstraintExpression Property(string name)
		{
			return Append(new PropOperator(name));
		}

		public ResolvableConstraintExpression Attribute(Type expectedType)
		{
			return Append(new AttributeOperator(expectedType));
		}

		public ResolvableConstraintExpression Attribute<TExpected>()
		{
			return Attribute(typeof(TExpected));
		}

		public Constraint Matches(IResolveConstraint constraint)
		{
			return Append((Constraint)constraint.Resolve());
		}

		public Constraint Matches<TActual>(Predicate<TActual> predicate)
		{
			return Append(new PredicateConstraint<TActual>(predicate));
		}

		public EqualConstraint EqualTo(object expected)
		{
			return (EqualConstraint)Append(new EqualConstraint(expected));
		}

		public SameAsConstraint SameAs(object expected)
		{
			return (SameAsConstraint)Append(new SameAsConstraint(expected));
		}

		public GreaterThanConstraint GreaterThan(object expected)
		{
			return (GreaterThanConstraint)Append(new GreaterThanConstraint(expected));
		}

		public GreaterThanOrEqualConstraint GreaterThanOrEqualTo(object expected)
		{
			return (GreaterThanOrEqualConstraint)Append(new GreaterThanOrEqualConstraint(expected));
		}

		public GreaterThanOrEqualConstraint AtLeast(object expected)
		{
			return (GreaterThanOrEqualConstraint)Append(new GreaterThanOrEqualConstraint(expected));
		}

		public LessThanConstraint LessThan(object expected)
		{
			return (LessThanConstraint)Append(new LessThanConstraint(expected));
		}

		public LessThanOrEqualConstraint LessThanOrEqualTo(object expected)
		{
			return (LessThanOrEqualConstraint)Append(new LessThanOrEqualConstraint(expected));
		}

		public LessThanOrEqualConstraint AtMost(object expected)
		{
			return (LessThanOrEqualConstraint)Append(new LessThanOrEqualConstraint(expected));
		}

		public ExactTypeConstraint TypeOf(Type expectedType)
		{
			return (ExactTypeConstraint)Append(new ExactTypeConstraint(expectedType));
		}

		public ExactTypeConstraint TypeOf<TExpected>()
		{
			return (ExactTypeConstraint)Append(new ExactTypeConstraint(typeof(TExpected)));
		}

		public InstanceOfTypeConstraint InstanceOf(Type expectedType)
		{
			return (InstanceOfTypeConstraint)Append(new InstanceOfTypeConstraint(expectedType));
		}

		public InstanceOfTypeConstraint InstanceOf<TExpected>()
		{
			return (InstanceOfTypeConstraint)Append(new InstanceOfTypeConstraint(typeof(TExpected)));
		}

		public AssignableFromConstraint AssignableFrom(Type expectedType)
		{
			return (AssignableFromConstraint)Append(new AssignableFromConstraint(expectedType));
		}

		public AssignableFromConstraint AssignableFrom<TExpected>()
		{
			return (AssignableFromConstraint)Append(new AssignableFromConstraint(typeof(TExpected)));
		}

		public AssignableToConstraint AssignableTo(Type expectedType)
		{
			return (AssignableToConstraint)Append(new AssignableToConstraint(expectedType));
		}

		public AssignableToConstraint AssignableTo<TExpected>()
		{
			return (AssignableToConstraint)Append(new AssignableToConstraint(typeof(TExpected)));
		}

		public CollectionEquivalentConstraint EquivalentTo(IEnumerable expected)
		{
			return (CollectionEquivalentConstraint)Append(new CollectionEquivalentConstraint(expected));
		}

		public CollectionSubsetConstraint SubsetOf(IEnumerable expected)
		{
			return (CollectionSubsetConstraint)Append(new CollectionSubsetConstraint(expected));
		}

		public CollectionSupersetConstraint SupersetOf(IEnumerable expected)
		{
			return (CollectionSupersetConstraint)Append(new CollectionSupersetConstraint(expected));
		}

		public CollectionContainsConstraint Member(object expected)
		{
			return (CollectionContainsConstraint)Append(new CollectionContainsConstraint(expected));
		}

		public CollectionContainsConstraint Contains(object expected)
		{
			return (CollectionContainsConstraint)Append(new CollectionContainsConstraint(expected));
		}

		public ContainsConstraint Contains(string expected)
		{
			return (ContainsConstraint)Append(new ContainsConstraint(expected));
		}

		public ContainsConstraint Contain(string expected)
		{
			return (ContainsConstraint)Append(new ContainsConstraint(expected));
		}

		[Obsolete("Deprecated, use Contains")]
		public SubstringConstraint StringContaining(string expected)
		{
			return (SubstringConstraint)Append(new SubstringConstraint(expected));
		}

		[Obsolete("Deprecated, use Contains")]
		public SubstringConstraint ContainsSubstring(string expected)
		{
			return (SubstringConstraint)Append(new SubstringConstraint(expected));
		}

		public StartsWithConstraint StartWith(string expected)
		{
			return (StartsWithConstraint)Append(new StartsWithConstraint(expected));
		}

		public StartsWithConstraint StartsWith(string expected)
		{
			return (StartsWithConstraint)Append(new StartsWithConstraint(expected));
		}

		[Obsolete("Deprecated, use Does.StartWith or StartsWith")]
		public StartsWithConstraint StringStarting(string expected)
		{
			return (StartsWithConstraint)Append(new StartsWithConstraint(expected));
		}

		public EndsWithConstraint EndWith(string expected)
		{
			return (EndsWithConstraint)Append(new EndsWithConstraint(expected));
		}

		public EndsWithConstraint EndsWith(string expected)
		{
			return (EndsWithConstraint)Append(new EndsWithConstraint(expected));
		}

		[Obsolete("Deprecated, use Does.EndWith or EndsWith")]
		public EndsWithConstraint StringEnding(string expected)
		{
			return (EndsWithConstraint)Append(new EndsWithConstraint(expected));
		}

		public RegexConstraint Match(string pattern)
		{
			return (RegexConstraint)Append(new RegexConstraint(pattern));
		}

		public RegexConstraint Matches(string pattern)
		{
			return (RegexConstraint)Append(new RegexConstraint(pattern));
		}

		[Obsolete("Deprecated, use Does.Match or Matches")]
		public RegexConstraint StringMatching(string pattern)
		{
			return (RegexConstraint)Append(new RegexConstraint(pattern));
		}

		public SamePathConstraint SamePath(string expected)
		{
			return (SamePathConstraint)Append(new SamePathConstraint(expected));
		}

		public SubPathConstraint SubPathOf(string expected)
		{
			return (SubPathConstraint)Append(new SubPathConstraint(expected));
		}

		public SamePathOrUnderConstraint SamePathOrUnder(string expected)
		{
			return (SamePathOrUnderConstraint)Append(new SamePathOrUnderConstraint(expected));
		}

		public RangeConstraint InRange(IComparable from, IComparable to)
		{
			return (RangeConstraint)Append(new RangeConstraint(from, to));
		}
	}
}

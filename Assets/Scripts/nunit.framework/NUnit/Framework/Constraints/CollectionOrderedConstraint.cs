using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NUnit.Framework.Constraints
{
	public class CollectionOrderedConstraint : CollectionConstraint
	{
		private enum OrderDirection
		{
			Unspecified = 0,
			Ascending = 1,
			Descending = 2
		}

		private class OrderingStep
		{
			public string PropertyName { get; set; }

			public OrderDirection Direction { get; set; }

			public ComparisonAdapter Comparer { get; set; }

			public string ComparerName { get; set; }

			public OrderingStep(string propertyName)
			{
				PropertyName = propertyName;
				Comparer = ComparisonAdapter.Default;
			}
		}

		private List<OrderingStep> _steps;

		private OrderingStep _activeStep;

		public override string DisplayName
		{
			get
			{
				return "Ordered";
			}
		}

		public CollectionOrderedConstraint Ascending
		{
			get
			{
				if (_activeStep.Direction != 0)
				{
					throw new InvalidOperationException("Only one directional modifier may be used");
				}
				_activeStep.Direction = OrderDirection.Ascending;
				return this;
			}
		}

		public CollectionOrderedConstraint Descending
		{
			get
			{
				if (_activeStep.Direction != 0)
				{
					throw new InvalidOperationException("Only one directional modifier may be used");
				}
				_activeStep.Direction = OrderDirection.Descending;
				return this;
			}
		}

		public CollectionOrderedConstraint Then
		{
			get
			{
				CreateNextStep(null);
				return this;
			}
		}

		public override string Description
		{
			get
			{
				string text = "collection ordered";
				int num = 0;
				foreach (OrderingStep step in _steps)
				{
					if (num++ != 0)
					{
						text += " then";
					}
					if (step.PropertyName != null)
					{
						text = text + " by " + MsgUtils.FormatValue(step.PropertyName);
					}
					if (step.Direction == OrderDirection.Descending)
					{
						text += ", descending";
					}
				}
				return text;
			}
		}

		public CollectionOrderedConstraint()
		{
			_steps = new List<OrderingStep>();
			CreateNextStep(null);
		}

		public CollectionOrderedConstraint Using(IComparer comparer)
		{
			if (_activeStep.ComparerName != null)
			{
				throw new InvalidOperationException("Only one Using modifier may be used");
			}
			_activeStep.Comparer = ComparisonAdapter.For(comparer);
			_activeStep.ComparerName = comparer.GetType().FullName;
			return this;
		}

		public CollectionOrderedConstraint Using<T>(IComparer<T> comparer)
		{
			if (_activeStep.ComparerName != null)
			{
				throw new InvalidOperationException("Only one Using modifier may be used");
			}
			_activeStep.Comparer = ComparisonAdapter.For(comparer);
			_activeStep.ComparerName = comparer.GetType().FullName;
			return this;
		}

		public CollectionOrderedConstraint Using<T>(Comparison<T> comparer)
		{
			if (_activeStep.ComparerName != null)
			{
				throw new InvalidOperationException("Only one Using modifier may be used");
			}
			_activeStep.Comparer = ComparisonAdapter.For(comparer);
			_activeStep.ComparerName = comparer.GetType().FullName;
			return this;
		}

		public CollectionOrderedConstraint By(string propertyName)
		{
			if (_activeStep.PropertyName == null)
			{
				_activeStep.PropertyName = propertyName;
			}
			else
			{
				CreateNextStep(propertyName);
			}
			return this;
		}

		protected override bool Matches(IEnumerable actual)
		{
			object obj = null;
			int num = 0;
			foreach (object item in actual)
			{
				if (item == null)
				{
					throw new ArgumentNullException("actual", "Null value at index " + num);
				}
				if (obj != null)
				{
					if (_steps[0].PropertyName != null)
					{
						foreach (OrderingStep step in _steps)
						{
							string propertyName = step.PropertyName;
							PropertyInfo property = obj.GetType().GetProperty(propertyName);
							PropertyInfo property2 = item.GetType().GetProperty(propertyName);
							object value = property.GetValue(obj, null);
							object value2 = property2.GetValue(item, null);
							if (value2 == null)
							{
								throw new ArgumentException("actual", "Null property value at index " + num);
							}
							int num2 = step.Comparer.Compare(value, value2);
							if (num2 < 0)
							{
								if (step.Direction == OrderDirection.Descending)
								{
									return false;
								}
								break;
							}
							if (num2 > 0)
							{
								if (step.Direction != OrderDirection.Descending)
								{
									return false;
								}
								break;
							}
						}
					}
					else
					{
						int num2 = _activeStep.Comparer.Compare(obj, item);
						if (_activeStep.Direction == OrderDirection.Descending && num2 < 0)
						{
							return false;
						}
						if (_activeStep.Direction != OrderDirection.Descending && num2 > 0)
						{
							return false;
						}
					}
				}
				obj = item;
				num++;
			}
			return true;
		}

		protected override string GetStringRepresentation()
		{
			StringBuilder stringBuilder = new StringBuilder("<ordered");
			if (_steps.Count > 0)
			{
				OrderingStep orderingStep = _steps[0];
				if (orderingStep.PropertyName != null)
				{
					stringBuilder.Append("by " + orderingStep.PropertyName);
				}
				if (orderingStep.Direction == OrderDirection.Descending)
				{
					stringBuilder.Append(" descending");
				}
				if (orderingStep.ComparerName != null)
				{
					stringBuilder.Append(" " + orderingStep.ComparerName);
				}
			}
			stringBuilder.Append(">");
			return stringBuilder.ToString();
		}

		private void CreateNextStep(string propertyName)
		{
			_activeStep = new OrderingStep(propertyName);
			_steps.Add(_activeStep);
		}
	}
}

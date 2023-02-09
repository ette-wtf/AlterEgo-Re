using System.Text;

namespace NUnit.Framework.Interfaces
{
	public class ResultState
	{
		public static readonly ResultState Inconclusive = new ResultState(TestStatus.Inconclusive);

		public static readonly ResultState Skipped = new ResultState(TestStatus.Skipped);

		public static readonly ResultState Ignored = new ResultState(TestStatus.Skipped, "Ignored");

		public static readonly ResultState Explicit = new ResultState(TestStatus.Skipped, "Explicit");

		public static readonly ResultState Success = new ResultState(TestStatus.Passed);

		public static readonly ResultState Failure = new ResultState(TestStatus.Failed);

		public static readonly ResultState Error = new ResultState(TestStatus.Failed, "Error");

		public static readonly ResultState Cancelled = new ResultState(TestStatus.Failed, "Cancelled");

		public static readonly ResultState NotRunnable = new ResultState(TestStatus.Failed, "Invalid");

		public static readonly ResultState ChildFailure = Failure.WithSite(FailureSite.Child);

		public static readonly ResultState SetUpFailure = Failure.WithSite(FailureSite.SetUp);

		public static readonly ResultState SetUpError = Error.WithSite(FailureSite.SetUp);

		public static readonly ResultState TearDownError = Error.WithSite(FailureSite.TearDown);

		public TestStatus Status { get; private set; }

		public string Label { get; private set; }

		public FailureSite Site { get; private set; }

		public ResultState(TestStatus status)
			: this(status, string.Empty, FailureSite.Test)
		{
		}

		public ResultState(TestStatus status, string label)
			: this(status, label, FailureSite.Test)
		{
		}

		public ResultState(TestStatus status, FailureSite site)
			: this(status, string.Empty, site)
		{
		}

		public ResultState(TestStatus status, string label, FailureSite site)
		{
			Status = status;
			Label = ((label == null) ? string.Empty : label);
			Site = site;
		}

		public ResultState WithSite(FailureSite site)
		{
			return new ResultState(Status, Label, site);
		}

		public override bool Equals(object obj)
		{
			ResultState resultState = obj as ResultState;
			if (resultState == null)
			{
				return false;
			}
			return Status.Equals(resultState.Status) && Label.Equals(resultState.Label) && Site.Equals(resultState.Site);
		}

		public override int GetHashCode()
		{
			return ((int)Status << (int)(8 + Site)) ^ Label.GetHashCode();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(Status.ToString());
			if (Label != null && Label.Length > 0)
			{
				stringBuilder.AppendFormat(":{0}", Label);
			}
			if (Site != 0)
			{
				stringBuilder.AppendFormat("({0})", Site.ToString());
			}
			return stringBuilder.ToString();
		}
	}
}

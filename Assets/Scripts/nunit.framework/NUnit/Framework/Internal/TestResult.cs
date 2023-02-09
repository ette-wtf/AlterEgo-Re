using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
	public abstract class TestResult : ITestResult, IXmlNodeBuilder
	{
		internal const double MIN_DURATION = 1E-06;

		internal static readonly string CHILD_ERRORS_MESSAGE = "One or more child tests had errors";

		internal static readonly string CHILD_IGNORE_MESSAGE = "One or more child tests were ignored";

		private StringBuilder _output = new StringBuilder();

		private double _duration;

		protected int InternalAssertCount;

		private ResultState _resultState;

		private string _message;

		private string _stackTrace;

		public ITest Test { get; private set; }

		public ResultState ResultState
		{
			get
			{
				try
				{
					return _resultState;
				}
				finally
				{
				}
			}
			private set
			{
				_resultState = value;
			}
		}

		public virtual string Name
		{
			get
			{
				return Test.Name;
			}
		}

		public virtual string FullName
		{
			get
			{
				return Test.FullName;
			}
		}

		public double Duration
		{
			get
			{
				return _duration;
			}
			set
			{
				_duration = ((value >= 1E-06) ? value : 1E-06);
			}
		}

		public DateTime StartTime { get; set; }

		public DateTime EndTime { get; set; }

		public string Message
		{
			get
			{
				try
				{
					return _message;
				}
				finally
				{
				}
			}
			private set
			{
				_message = value;
			}
		}

		public virtual string StackTrace
		{
			get
			{
				try
				{
					return _stackTrace;
				}
				finally
				{
				}
			}
			private set
			{
				_stackTrace = value;
			}
		}

		public int AssertCount
		{
			get
			{
				try
				{
					return InternalAssertCount;
				}
				finally
				{
				}
			}
			internal set
			{
				InternalAssertCount = value;
			}
		}

		public abstract int FailCount { get; }

		public abstract int PassCount { get; }

		public abstract int SkipCount { get; }

		public abstract int InconclusiveCount { get; }

		public abstract bool HasChildren { get; }

		public abstract IEnumerable<ITestResult> Children { get; }

		public TextWriter OutWriter { get; private set; }

		public string Output
		{
			get
			{
				return _output.ToString();
			}
		}

		public TestResult(ITest test)
		{
			Test = test;
			ResultState = ResultState.Inconclusive;
			OutWriter = TextWriter.Synchronized(new StringWriter(_output));
		}

		public TNode ToXml(bool recursive)
		{
			return AddToXml(new TNode("dummy"), recursive);
		}

		public virtual TNode AddToXml(TNode parentNode, bool recursive)
		{
			TNode tNode = Test.AddToXml(parentNode, false);
			tNode.AddAttribute("result", ResultState.Status.ToString());
			if (ResultState.Label != string.Empty)
			{
				tNode.AddAttribute("label", ResultState.Label);
			}
			if (ResultState.Site != 0)
			{
				tNode.AddAttribute("site", ResultState.Site.ToString());
			}
			tNode.AddAttribute("start-time", StartTime.ToString("u"));
			tNode.AddAttribute("end-time", EndTime.ToString("u"));
			tNode.AddAttribute("duration", Duration.ToString("0.000000", NumberFormatInfo.InvariantInfo));
			if (Test is TestSuite)
			{
				tNode.AddAttribute("total", (PassCount + FailCount + SkipCount + InconclusiveCount).ToString());
				tNode.AddAttribute("passed", PassCount.ToString());
				tNode.AddAttribute("failed", FailCount.ToString());
				tNode.AddAttribute("inconclusive", InconclusiveCount.ToString());
				tNode.AddAttribute("skipped", SkipCount.ToString());
			}
			tNode.AddAttribute("asserts", AssertCount.ToString());
			switch (ResultState.Status)
			{
			case TestStatus.Failed:
				AddFailureElement(tNode);
				break;
			case TestStatus.Inconclusive:
			case TestStatus.Skipped:
			case TestStatus.Passed:
				if (Message != null)
				{
					AddReasonElement(tNode);
				}
				break;
			}
			if (Output.Length > 0)
			{
				AddOutputElement(tNode);
			}
			if (recursive && HasChildren)
			{
				foreach (TestResult child in Children)
				{
					child.AddToXml(tNode, recursive);
				}
			}
			return tNode;
		}

		public void SetResult(ResultState resultState)
		{
			SetResult(resultState, null, null);
		}

		public void SetResult(ResultState resultState, string message)
		{
			SetResult(resultState, message, null);
		}

		public void SetResult(ResultState resultState, string message, string stackTrace)
		{
			try
			{
				ResultState = resultState;
				Message = message;
				StackTrace = stackTrace;
			}
			finally
			{
			}
		}

		public void RecordException(Exception ex)
		{
			if (ex is NUnitException)
			{
				ex = ex.InnerException;
			}
			if (ex is ResultStateException)
			{
				SetResult(((ResultStateException)ex).ResultState, ex.Message, StackFilter.Filter(ex.StackTrace));
			}
			else if (ex is ThreadAbortException)
			{
				SetResult(ResultState.Cancelled, "Test cancelled by user", ex.StackTrace);
			}
			else
			{
				SetResult(ResultState.Error, ExceptionHelper.BuildMessage(ex), ExceptionHelper.BuildStackTrace(ex));
			}
		}

		public void RecordException(Exception ex, FailureSite site)
		{
			if (ex is NUnitException)
			{
				ex = ex.InnerException;
			}
			if (ex is ResultStateException)
			{
				SetResult(((ResultStateException)ex).ResultState.WithSite(site), ex.Message, StackFilter.Filter(ex.StackTrace));
			}
			else if (ex is ThreadAbortException)
			{
				SetResult(ResultState.Cancelled.WithSite(site), "Test cancelled by user", ex.StackTrace);
			}
			else
			{
				SetResult(ResultState.Error.WithSite(site), ExceptionHelper.BuildMessage(ex), ExceptionHelper.BuildStackTrace(ex));
			}
		}

		public void RecordTearDownException(Exception ex)
		{
			if (ex is NUnitException)
			{
				ex = ex.InnerException;
			}
			ResultState resultState = ((ResultState == ResultState.Cancelled) ? ResultState.Cancelled : ResultState.Error);
			if (Test.IsSuite)
			{
				resultState = resultState.WithSite(FailureSite.TearDown);
			}
			string text = "TearDown : " + ExceptionHelper.BuildMessage(ex);
			if (Message != null)
			{
				text = Message + Env.NewLine + text;
			}
			string text2 = "--TearDown" + Env.NewLine + ExceptionHelper.BuildStackTrace(ex);
			if (StackTrace != null)
			{
				text2 = StackTrace + Env.NewLine + text2;
			}
			SetResult(resultState, text, text2);
		}

		private TNode AddReasonElement(TNode targetNode)
		{
			TNode tNode = targetNode.AddElement("reason");
			return tNode.AddElementWithCDATA("message", Message);
		}

		private TNode AddFailureElement(TNode targetNode)
		{
			TNode tNode = targetNode.AddElement("failure");
			if (Message != null)
			{
				tNode.AddElementWithCDATA("message", Message);
			}
			if (StackTrace != null)
			{
				tNode.AddElementWithCDATA("stack-trace", StackTrace);
			}
			return tNode;
		}

		private TNode AddOutputElement(TNode targetNode)
		{
			return targetNode.AddElementWithCDATA("output", Output);
		}
	}
}

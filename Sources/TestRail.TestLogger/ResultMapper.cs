using System.Linq;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using TestRail.Types;

namespace TestRail.TestLogger
{
    public static class ResultMapper
    {
        private const string TestCaseId = "CaseId";
        private const string TestCaseTitle = "CaseTitle";

        public static CaseResult MapToCaseResult(this TestResultEventArgs args)
        {
            var caseResult = new CaseResult
            {
                CaseId = GetTestCaseId(args),
                Title = GetTitle(args),
                Status = GetResult(args),
                Comment = GetComment(args)
            };
            return caseResult;
        }

        private static string GetTitle(TestResultEventArgs args)
        {
            var trait = args.Result.TestCase.Traits.FirstOrDefault(t => t.Name == TestCaseTitle);
            return trait?.Value;
        }

        private static string GetComment(TestResultEventArgs args)
        {
            return args.Result.ErrorMessage;
        }

        private static ResultStatus GetResult(TestResultEventArgs args)
        {
            switch (args.Result.Outcome)
            {
                case TestOutcome.Passed:
                    return ResultStatus.Passed;
                case TestOutcome.Failed:
                    return ResultStatus.Failed;
                case TestOutcome.Skipped:
                    return ResultStatus.Blocked;
                default:
                    // NA - status
                    return ResultStatus.CustomStatus2;
            }
        }

        private static ulong? GetTestCaseId(TestResultEventArgs args)
        {
            var trait = args.Result.TestCase.Traits.FirstOrDefault(t => t.Name == TestCaseId);
            if (trait == null) return null;
            return ulong.Parse(trait.Value);
        }
    }
}
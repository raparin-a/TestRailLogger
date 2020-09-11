using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TestRail.Types;
using Config = TestRail.TestLogger.Configuration.Config;

namespace TestRail.TestLogger
{
    public interface ITestManager
    {
        void AddResultToCollection(ulong? runId, CaseResult caseResult);
        ulong? GetTestRun();
        void ResultBulkSend();
    }

    public class TestManager : ITestManager
    {
        private readonly TestRailClient _client;
        private readonly Config _config;
        private readonly ulong? _testRunId;
        private readonly ConcurrentDictionary<ulong, ResultStatus> _statuses = new ConcurrentDictionary<ulong, ResultStatus>();
        private readonly ConcurrentDictionary<ulong, ConcurrentBag<CaseResult>> _results = new ConcurrentDictionary<ulong, ConcurrentBag<CaseResult>>();

        public TestManager()
        {
            _config = Config.GetInstance();
            _client = new TestRailClient(_config.Url, _config.User.Name, _config.User.Password);
            _testRunId = GetTestRun();
        }

        public void AddResultToCollection(ulong? runId, CaseResult caseResult)
        {
            if (runId == null) return;
            if (caseResult.CaseId == null && !string.IsNullOrEmpty(caseResult.Title))
            {
                var run = _client.GetRun(runId.Value);
                var cases = _client.GetCases(run.ProjectID.Value, run.SuiteID.Value);
                var @case = cases.FirstOrDefault(i => i.Title == caseResult.Title);
                caseResult.CaseId = @case?.ID;
            }

            if (caseResult.CaseId == null) return;
            // Do not override Failed test result with Passed status
            AddToCollection(caseResult.CaseId.Value, caseResult);
        }

        private bool IsMarkedAsFailed(ulong caseId)
        {
            if (_statuses.TryGetValue(caseId, out var value))
            {
                return value != ResultStatus.Passed;
            }

            return false;
        }

        public ulong? GetTestRun()
        {
            Project project = null;
            Run run = null;
            var projects = _client.GetProjects();
            if (projects.Count == 0)
            {
                Console.WriteLine("An error occurred while getting the list of projects from TestRail. " +
                    "Check the availability of TestRail from the current environment.");
            }
            try
            {
                project = projects.First(p => p.Name == _config.ProjectName);
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine($"The ProjectName '{_config.ProjectName}' from the TestRail.json file was not found in the project list. " +
                    "Please check that the ProjectName is correct.");
                throw e;
            }
            var runs = _client.GetRuns(project.ID);
            try
            {
                run = runs.First(i => i.IsCompleted == false && Regex.IsMatch(i.Name, _config.RunNameTemplate));
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine($"Test run '{_config.RunNameTemplate}' from TestRail.json file not found. " +
                    "Please make sure the template is correct.");
                throw e;
            }
            return run.ID;
        }

        public void AddToCollection(ulong testRailCaseId, CaseResult caseResult)
        {
            if (_results.TryGetValue(testRailCaseId, out var list))
            {
                list.Add(caseResult);
            }
            else
            {
                list = new ConcurrentBag<CaseResult>();
                list.Add(caseResult);
                _results.TryAdd(testRailCaseId, list);
            }
        }

        public void ResultBulkSend()
        {
            foreach (var item in _results)
            {
                var testCaseId = item.Key;
                var testStatus = item.Value.All(x => x.Status == ResultStatus.Passed)
                    ? ResultStatus.Passed
                    : ResultStatus.Failed;

                string message;

                if (testStatus == ResultStatus.Passed)
                    message = "No errors";
                else
                {
                    var failedTestCases = item.Value.Where(x => x.Status == ResultStatus.Failed).ToList();
                    StringBuilder sb = new StringBuilder();
                    foreach (CaseResult caseResult in failedTestCases)
                    {
                        sb.AppendLine($"Error message: {caseResult.Comment}");
                    }
                    message = sb.ToString();
                }

                var result = _client.AddResultForCase((ulong)_testRunId, testCaseId, testStatus, message);
                if (!result.WasSuccessful)
                {
                    var errorMessage = result.Exception.Message;
                    switch (errorMessage)
                    {
                        case string a when a.Contains("400"):
                            Console.WriteLine($"TestCaseId {testCaseId} not found in TestRail. " +
                             $"Please check if the id is correct.");
                            Console.WriteLine($"Exception: {errorMessage}");
                            break;
                        case string a when a.Contains("403"):
                            Console.WriteLine("The user from the TestRail.json file does not have sufficient rights to " +
                            "work with the project specified in this file.");
                            Console.WriteLine($"Exception: {errorMessage}");
                            break;
                        default:
                            Console.WriteLine($"Error sending results to TestRail: {errorMessage}");
                            break;
                    }
                }
                _results.TryRemove(testCaseId, out _);
            }
        }
    }
}
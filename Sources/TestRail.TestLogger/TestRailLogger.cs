using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System.Collections.Generic;
using System.Linq;
using TestRail.TestLogger.Configuration;

namespace TestRail.TestLogger
{
    [ExtensionUri("logger://TestRail")]
    [FriendlyName("TestRail")]
    public class TestRailLogger : ITestLoggerWithParameters
    {
        private readonly ITestManager _testManager;
        private ulong? _runId;

        public TestRailLogger()
        {
            _testManager = new TestManager();
        }

        public void Initialize(TestLoggerEvents events, string testRunDirectory)
        {
            if (Config.GetInstance().Disabled) return;
            events.TestRunStart += CreateTestRun;
            events.TestResult += AddResultToCollection;
            events.TestRunComplete += ResultBulkSend;
        }

        public void Initialize(TestLoggerEvents events, Dictionary<string, string> parameters)
        {
            Initialize(events, parameters.Single(p => p.Key == "TestRunDirectory").Value);
        }

        private void AddResultToCollection(object sender, TestResultEventArgs e)
        {
            var caseResult = e.MapToCaseResult();
            _testManager.AddResultToCollection(_runId, caseResult);
        }

        private void ResultBulkSend(object sender, TestRunCompleteEventArgs e)
        {
            _testManager.ResultBulkSend();
        }

        private void CreateTestRun(object sender, TestRunStartEventArgs e)
        {
            _runId = _testManager.GetTestRun();
        }
    }
}
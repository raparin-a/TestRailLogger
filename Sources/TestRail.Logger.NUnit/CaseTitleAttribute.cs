using System;
using NUnit.Framework;

namespace TestRail.Logger.NUnit
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CaseTitleAttribute : PropertyAttribute
    {
        private const string TestCaseTitle = "CaseTitle";

        public CaseTitleAttribute(string title) : base(TestCaseTitle, title)
        {
        }
    }
}
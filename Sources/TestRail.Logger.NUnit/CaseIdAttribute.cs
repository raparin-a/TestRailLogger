using System;
using NUnit.Framework;

namespace TestRail.Logger.NUnit
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CaseIdAttribute : PropertyAttribute
    {
        private const string TestCaseId = "CaseId";

        public CaseIdAttribute(ulong id) : base(TestCaseId, id)
        {
        }
    }
}
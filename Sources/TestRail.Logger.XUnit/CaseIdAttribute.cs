using System;
using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace TestRail.Logger.XUnit
{
    public class CaseIdDiscoverer : ITraitDiscoverer
    {
        private const string TestCaseId = "CaseId";

        public IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute)
        {
            var constructorArguments = traitAttribute.GetConstructorArguments().ToList();
            yield return new KeyValuePair<string, string>(TestCaseId, constructorArguments[0].ToString());
        }
    }

    [TraitDiscoverer("TestRail.Logger.XUnit.CaseIdDiscoverer", "TestRail.Logger.XUnit")]
    [AttributeUsage(AttributeTargets.Method)]
    public class CaseIdAttribute : Attribute, ITraitAttribute
    {
        public CaseIdAttribute(ulong id) { }
    }
}
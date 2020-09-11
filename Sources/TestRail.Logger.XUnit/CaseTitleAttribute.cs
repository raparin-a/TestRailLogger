using System;
using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace TestRail.Logger.XUnit
{
    public class CaseTitleDiscoverer : ITraitDiscoverer
    {
        private const string TestCaseTitle = "CaseTitle";

        public IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute)
        {
            var constructorArguments = traitAttribute.GetConstructorArguments().ToList();
            yield return new KeyValuePair<string, string>(TestCaseTitle, constructorArguments[0].ToString());
        }
    }

    [TraitDiscoverer("TestRail.Logger.XUnit.CaseTitleDiscoverer", "TestRail.Logger.XUnit")]
    [AttributeUsage(AttributeTargets.Method)]
    public class CaseTitleAttribute : Attribute, ITraitAttribute
    {
        public CaseTitleAttribute(string title)
        {
        }
    }
}
using RuleEngine;
using Xunit.Sdk;

namespace RuleEngineTests;

[GenerateRules]
public class PersonParameters
{
	public string Name { get; set; }
}

public class UnitTest1
{
	[Fact]
	public void TestNonCompiled()
	{
		var rule = new NameEqualsRule("Tal");
		var personParameters = new PersonParameters
		{
			Name = "Tal"
		};

		Assert.True(rule.Evaluate(personParameters));
	}
	[Fact]
	public void TestCompiled()
	{
		var rule = new NameEqualsRule("Tal").ToExpression().Compile();
		var personParameters = new PersonParameters
		{
			Name = "Tal"
		};

		Assert.True(rule(personParameters));
	}

}
using RuleEngine;
using Xunit.Sdk;

namespace RuleEngineTests;

[GenerateRules]
public sealed record Person(string Name, int Age);

public class UnitTest1
{
	public static readonly IRuleNode<Person> Rule = new AgeEqualsRule(19) & new NameEqualsRule("Tal");
	public static readonly Func<Person, bool> CompiledRule = Rule.ToFunc();

	[Fact]
	public void TestYesCompiled()
	{
		var personParameters = new Person("Tal", 19);

		Assert.True(CompiledRule(personParameters));
		Assert.False(CompiledRule(personParameters with { Name = "Blah" }));
	}
	[Fact]
	public void TestNonCompiled()
	{
		var personParameters = new Person("Tal", 19);

		Assert.True(Rule.Evaluate(personParameters));
		Assert.False(Rule.Evaluate(personParameters with { Name = "Blah" }));
	}
}
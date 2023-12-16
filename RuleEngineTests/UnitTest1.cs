using RuleEngine;
using Xunit.Sdk;

namespace RuleEngineTests;

public record Person(string Name) : INameTrait;

public class UnitTest1
{
	public static readonly NameEqualsRule Rule = new("Tal");
	public static readonly Func<INameTrait, bool> CompiledRule = Rule.ToExpression().Compile();
	[Fact]
	public void TestNonCompiled()
	{
		var personParameters = new Person("Tal");

		Assert.True(Rule.Evaluate(personParameters));
		Assert.False(Rule.Evaluate(personParameters with { Name = "Blah" }));
	}
	[Fact]
	public void TestCompiled()
	{
		var personParameters = new Person("Tal");

		Assert.True(CompiledRule(personParameters));
		Assert.False(CompiledRule(personParameters with { Name = "Blah" }));
	}
}
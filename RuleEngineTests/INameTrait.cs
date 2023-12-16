using RuleEngine;

namespace RuleEngineTests
{
	[GenerateRules]
	public interface INameTrait
	{
		string Name { get; init; }
	}
}
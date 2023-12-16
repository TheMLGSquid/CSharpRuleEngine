using System.Linq.Expressions;

namespace RuleEngine;

public interface IRuleNode<TParams>
{
	bool Evaluate(in TParams parameters);
	Expression<Func<TParams, bool>> ToExpression();
	string Describe();
}

public static class RuleExtensions
{
	public static Func<TParams, bool> ToFunc<TParams>(this IRuleNode<TParams> rule)
		=> rule.ToExpression().Compile();
}
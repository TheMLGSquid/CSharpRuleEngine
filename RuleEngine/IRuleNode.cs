using System.Linq.Expressions;

namespace RuleEngine;

public interface IRuleNode<TParams>
{
	bool Evaluate(in TParams parameters);
	Expression<Func<TParams, bool>> ToExpression();
	string Describe();
}
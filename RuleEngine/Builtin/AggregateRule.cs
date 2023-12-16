using System.Collections.Immutable;
using System.Linq.Expressions;

namespace RuleEngine.Builtin;

public sealed record AggregateRule<TParams>(params IRuleNode<TParams>[] Rules)
	: RuleNodeBase<TParams>
{
	public override string Describe()
	{
		return "All["
			+ string.Join(", ", Rules.Select(rule => rule.Describe()))
			+ ']';
	}

	public override bool Evaluate(in TParams parameters)
	{
		foreach (var rule in Rules)
		{
			if (!rule.Evaluate(parameters))
				return false;
		}
		return true;
	}

	public override Expression<Func<TParams, bool>> ToExpression()
	{
		Expression? aggregateExpression = Rules.Aggregate(
			null as Expression,
			(aggregate, rule) => aggregate is null
				? ParameterReplacer<TParams>.Replace(rule.ToExpression().Body)
				: Expression.AndAlso(aggregate, ParameterReplacer<TParams>.Replace(rule.ToExpression().Body))
			);

		return Expression.Lambda<Func<TParams, bool>>(
			aggregateExpression ?? throw new InvalidOperationException("Rules can't be empty"),
			ParameterReplacer<TParams>.ParamExpr
			);
	}
}

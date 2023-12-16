﻿using System.Collections.Immutable;
using System.Linq.Expressions;

namespace RuleEngine.Builtin;

public sealed record AggregateRule<TParams>(ImmutableArray<IRuleNode<TParams>> Rules)
	: RuleNodeBase<TParams>
{
	public AggregateRule(params IRuleNode<TParams>[] rules)
		: this(rules.ToImmutableArray()) { }
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
				? ParameterReplacer<TParams>.Replace(rule.ToExpression())
				: Expression.AndAlso(aggregate, ParameterReplacer<TParams>.Replace(rule.ToExpression()))
			);

		return Expression.Lambda<Func<TParams, bool>>(
			aggregateExpression ?? throw new InvalidOperationException("Rules can't be empty"),
			ParameterReplacer<TParams>.ParamExpr
			);
	}
}
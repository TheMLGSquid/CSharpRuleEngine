using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RuleEngine.Builtin;

public sealed record OneOfRule<TParams>(ImmutableArray<IRuleNode<TParams>> Rules)
	: RuleNodeBase<TParams>
{
	public OneOfRule(params IRuleNode<TParams>[] rules)
		: this(rules.ToImmutableArray()) { }

	public override string Describe()
	{
		return "Any["
			+ string.Join(", ", Rules.Select(rule => rule.Describe()))
			+ ']';
	}

	public override bool Evaluate(in TParams parameters)
	{
		foreach (var rule in Rules)
		{
			if (rule.Evaluate(parameters))
				return true;
		}
		return false;
	}

	public override Expression<Func<TParams, bool>> ToExpression()
	{
		Expression? aggregateExpression = Rules.Aggregate(
			null as Expression,
			(aggregate, rule) => aggregate is null
				? ParameterReplacer<TParams>.Replace(rule.ToExpression())
				: Expression.OrElse(aggregate, ParameterReplacer<TParams>.Replace(rule.ToExpression()))
			);

		return Expression.Lambda<Func<TParams, bool>>(
			aggregateExpression ?? throw new InvalidOperationException("Rules can't be empty"),
			ParameterReplacer<TParams>.ParamExpr
			);
	}
}

using System.Linq.Expressions;

namespace RuleEngine.Builtin;

public sealed record NotRule<TParams>(IRuleNode<TParams> Rule) : RuleNodeBase<TParams>
{
	public override string Describe()
	{
		return $"Not {Rule.Describe}";
	}

	public override bool Evaluate(in TParams parameters)
	{
		return !Rule.Evaluate(parameters);
	}

	public override Expression<Func<TParams, bool>> ToExpression()
	{
		return Expression.Lambda<Func<TParams, bool>>(
			Expression.Not(ParameterReplacer<TParams>.Replace(Rule.ToExpression())),
			true,
			ParameterReplacer<TParams>.ParamExpr
			);
	}
}

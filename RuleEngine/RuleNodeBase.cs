using RuleEngine.Builtin;
using System.Linq.Expressions;

namespace RuleEngine;

public abstract record RuleNodeBase<TParams> : IRuleNode<TParams>
{
	public static OneOfRule<TParams> operator | (RuleNodeBase<TParams> a, RuleNodeBase<TParams> b)
	{
		if (a is OneOfRule<TParams> oneof)
		{
			return oneof with { Rules = oneof.Rules.Add(b) };
		}

		return new OneOfRule<TParams>(a, b);
	}
	public static AggregateRule<TParams> operator & (RuleNodeBase<TParams> a, RuleNodeBase<TParams> b)
	{
		if (a is AggregateRule<TParams> aggregate)
		{
			return aggregate with { Rules = aggregate.Rules.Add(b) };
		}

		return new AggregateRule<TParams>(a, b);
	}
	public static NotRule<TParams> operator !(RuleNodeBase<TParams> a) => new NotRule<TParams>(a);

	public abstract string Describe();
	public abstract bool Evaluate(in TParams parameters);
	public abstract Expression<Func<TParams, bool>> ToExpression();
}
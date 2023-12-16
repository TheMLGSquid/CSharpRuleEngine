using System.Linq.Expressions;

namespace RuleEngine.Decisions;

public sealed record TrueRule<TParams> : RuleNodeBase<TParams>
{
	public static readonly TrueRule<TParams> Instance = new();
	private TrueRule() { }

	public override string Describe() => "Valid";

	public override bool Evaluate(in TParams parameters) => true;

	public override Expression<Func<TParams, bool>> ToExpression() => static _ => true;
}

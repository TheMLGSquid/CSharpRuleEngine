using System.Linq.Expressions;

namespace RuleEngine.Decisions;

public sealed record FalseRule<TParams> : RuleNodeBase<TParams>
{
	public static readonly FalseRule<TParams> Instance = new();
	private FalseRule() { }

	public override string Describe() => "Invalid";

	public override bool Evaluate(in TParams parameters) => false;

	public override Expression<Func<TParams, bool>> ToExpression() => static _ => false;
}

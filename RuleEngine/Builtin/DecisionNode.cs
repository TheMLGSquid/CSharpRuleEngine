using RuleEngine.Decisions;
using System.Linq.Expressions;

namespace RuleEngine.Builtin;

public sealed record DecisionNode<TParams>(IDecision<TParams> Decision)
    : RuleNodeBase<TParams>
{
    public IRuleNode<TParams> Then { get; init; } = TrueRule<TParams>.Instance;
    public IRuleNode<TParams> Else { get; init; } = TrueRule<TParams>.Instance;

    public override string Describe()
    {
        return $"[If {Decision.Describe()} Then({Then.Describe()}) Else({Else.Describe()})]";
    }

    public override bool Evaluate(in TParams parameters)
    {
        return Decision.Decide(parameters)
            ? Then.Evaluate(parameters)
            : Else.Evaluate(parameters);
    }

    public override Expression<Func<TParams, bool>> ToExpression()
    {
        var decisionExpr = FixParams(Decision.ToExpression());
        var thenExpr = FixParams(Then.ToExpression()).Body;
        var elseExpr = FixParams(Then.ToExpression()).Body;

        var conditionExpr = Expression.Condition(decisionExpr, thenExpr, elseExpr);

        return Expression.Lambda<Func<TParams, bool>>(
            conditionExpr,
            true,
            ParameterReplacer<TParams>.ParamExpr
            );
    }
}
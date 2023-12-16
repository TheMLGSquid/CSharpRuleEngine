using RuleEngine.Builtin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RuleEngine.Decisions;

public interface IDecision<TParams>
{
	string Describe();
	bool Decide(in TParams parameters);
	Expression<Func<TParams, bool>> ToExpression();
}

public static class DeicisonExtensions
{
	public static DecisionNode<TParams> Clause<TParams>(this IDecision<TParams> decision,
		IRuleNode<TParams> thenRule, IRuleNode<TParams> elseRule) => new(decision)
		{
			Then = thenRule,
			Else = elseRule
		};
	public static DecisionNode<TParams> Then<TParams>(this IDecision<TParams> decision,
		IRuleNode<TParams> thenRule, bool decisionMustBeTrue = true) => new(decision)
		{
			Then = thenRule,
			Else = decisionMustBeTrue ? FalseRule<TParams>.Instance : TrueRule<TParams>.Instance
		};
}

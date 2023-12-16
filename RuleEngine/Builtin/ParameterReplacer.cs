using System.Linq.Expressions;

namespace RuleEngine.Builtin;

internal class ParameterReplacer<TParams> : ExpressionVisitor
{
	public static readonly ParameterExpression ParamExpr
		= Expression.Parameter(typeof(TParams), "parameters");
	private static readonly ParameterReplacer<TParams> Instance = new();

	protected override Expression VisitParameter(ParameterExpression node)
	{
		return ParamExpr;
	}

	public static TExpression Replace<TExpression>(TExpression expression)
		where TExpression : Expression
	{
		return (TExpression)Instance.Visit(expression);
	}
}

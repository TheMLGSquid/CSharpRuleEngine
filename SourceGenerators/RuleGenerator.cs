using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace RuleEngine.SourceGeneration
{
	[Generator]
	public class RuleGenerator : ISourceGenerator
	{
		public void Execute(GeneratorExecutionContext context)
		{
			var compilation = context.Compilation;

			Console.WriteLine($"There are {compilation.SyntaxTrees.Count()} syntax trees");
			foreach (var syntaxTree in compilation.SyntaxTrees)
			{
				var semanticModel = compilation.GetSemanticModel(syntaxTree);
				var classNodes = syntaxTree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>();
				var interfaceNodes = syntaxTree.GetRoot().DescendantNodes().OfType<InterfaceDeclarationSyntax>();

				var symbols = classNodes.Select(node => (false, semanticModel.GetDeclaredSymbol(node)))
					.Concat(interfaceNodes.Select(node => (true, semanticModel.GetDeclaredSymbol(node))));

				foreach (var (isInterface, symbol) in symbols)
				{
					var classAttributes = symbol.GetAttributes();

					bool hasGenerateRulesAttribute = classAttributes
						.Any(ad => ad.AttributeClass.Name.Contains("GenerateRules"));
					if (hasGenerateRulesAttribute)
					{
						var generatedCode = GenerateCodeForClass(symbol, isInterface);
						context.AddSource($"{symbol.Name}_generated.cs", SourceText.From(generatedCode, Encoding.UTF8));
					}
				}

			}
		}

		public static string GenerateCodeForClass(INamedTypeSymbol classSymbol, bool isInterface)
		{
			var stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("using System;");
			stringBuilder.AppendLine("using RuleEngine;");
			stringBuilder.AppendLine("using System.Linq.Expressions;");
			stringBuilder.AppendLine($"namespace {classSymbol.ContainingNamespace}");
			stringBuilder.AppendLine("{");

			foreach (var member in classSymbol.GetMembers())
			{
				if (member is IPropertySymbol propertySymbol)
				{
					stringBuilder.AppendLine(GenerateEqualsRuleClassForProperty(classSymbol, propertySymbol, isInterface));
				}
			}

			stringBuilder.AppendLine("}");

			return stringBuilder.ToString();
		}

		private static string GenerateEqualsRuleClassForProperty(INamedTypeSymbol classSymbol, IPropertySymbol propertySymbol, bool isInterface)
		{
			string paramsType = classSymbol.Name;
			var stringBuilder = new StringBuilder();

			stringBuilder.AppendLine($"	public sealed record {propertySymbol.Name}EqualsRule({propertySymbol.Type.Name} EqualsValue)");
			stringBuilder.AppendLine($"		: RuleNodeBase<{paramsType}>");

			stringBuilder.AppendLine("	{");
			stringBuilder.AppendLine("		public override string Describe()");
			stringBuilder.AppendLine($"			=> $\"{propertySymbol.Name} == {{EqualsValue}}\";");
			stringBuilder.AppendLine("");
			stringBuilder.AppendLine($"		public override bool Evaluate(in {paramsType} parameters)");
			stringBuilder.AppendLine("		{");
			stringBuilder.AppendLine($"			return parameters.{propertySymbol.Name}.Equals(EqualsValue);");
			stringBuilder.AppendLine("		}");
			stringBuilder.AppendLine("");
			stringBuilder.AppendLine($"		public override Expression<Func<{paramsType}, bool>> ToExpression()");
			stringBuilder.AppendLine("		{");
			stringBuilder.AppendLine($"			return parameters => parameters.{propertySymbol.Name}.Equals(EqualsValue);");
			stringBuilder.AppendLine("		}");
			stringBuilder.AppendLine("	}");

			return stringBuilder.ToString();
		}

		public void Initialize(GeneratorInitializationContext context)
		{
						  //System.Diagnostics.Debugger.Launch();
		}
	}
}

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

				foreach (var classNode in classNodes)
				{
					var classSymbol = semanticModel.GetDeclaredSymbol(classNode);
					var classAttributes = classSymbol.GetAttributes();

					bool hasGenerateRulesAttribute = classAttributes
						.Any(ad => ad.AttributeClass.Name.Contains("GenerateRules"));
					if (hasGenerateRulesAttribute)
					{
						var generatedCode = GenerateCodeForClass(classSymbol);
						context.AddSource($"{classSymbol.Name}_generated.cs", SourceText.From(generatedCode, Encoding.UTF8));
					}
				}
			}
		}

		public static string GenerateCodeForClass(INamedTypeSymbol classSymbol)
		{
			var stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("using System;");
			stringBuilder.AppendLine("using RuleEngine;");
			stringBuilder.AppendLine("using System.Linq.Expressions;");
			stringBuilder.AppendLine($"namespace {classSymbol.ContainingNamespace}");
			stringBuilder.AppendLine("{");

			foreach (var member in classSymbol.GetMembers())
			{
				if (member is IPropertySymbol propertySymbol && propertySymbol.DeclaredAccessibility == Accessibility.Public)
				{
					stringBuilder.AppendLine(GenerateEqualsRuleClassForProperty(classSymbol, propertySymbol));
				}
			}

			stringBuilder.AppendLine("}");

			return stringBuilder.ToString();
		}

		private static string GenerateEqualsRuleClassForProperty(INamedTypeSymbol classSymbol, IPropertySymbol propertySymbol)
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
			stringBuilder.AppendLine("			return parameters.Equals(EqualsValue);");
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
						// System.Diagnostics.Debugger.Launch();
		}
	}
}

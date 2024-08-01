// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: MIT
using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;

namespace Remotion.Infrastructure.Analyzers.ReflectionVerifier;

public partial class AnalyzerInternal (SyntaxNodeAnalysisContext context)
{
  private readonly InvocationExpressionSyntax _node = (InvocationExpressionSyntax)context.Node;
  private readonly SemanticModel _semanticModel = context.SemanticModel;

  public Diagnostic? Analyze ()
  {
    if (_node.Expression is not MemberAccessExpressionSyntax memberAccessExpressionSyntax)
    {
      return null;
    }

    var methodName = memberAccessExpressionSyntax.Name.ToString();
    if (!Enum.TryParse<InvokingMethods>(methodName, out var kindOfMethod))
    {
      return null;
    }

    MethodSignature calledSignature;
    try
    {
      calledSignature = GetCalledSignature(kindOfMethod);
    }
    catch (NotSupportedException)
    {
      return null;
    }

    var isValid = DoesExist(calledSignature);
    Diagnostic? diagnostic = null;
    if (!isValid)
    {
      diagnostic = Diagnostic.Create(Rules.Rule, Location.Create(_node.SyntaxTree, _node.Span));
    }

    return diagnostic;
  }

  private bool DoesExist (MethodSignature signature)
  {
    var root = _node.SyntaxTree.GetRoot();
    foreach (var childNode in root.DescendantNodes())
    {
      if (childNode is BaseMethodDeclarationSyntax methodDeclarationSyntax)
      {
        string methodName;
        switch (methodDeclarationSyntax)
        {
          case MethodDeclarationSyntax method:
            methodName = method.Identifier.Text;
            break;
          case ConstructorDeclarationSyntax constructor:
            methodName = constructor.Identifier.Text;
            break;
          default:
            continue; // Skip other types of method-like declarations
        }

        var parameters = methodDeclarationSyntax.ParameterList.Parameters;
        var typesOfParameters = parameters.Select(
            param => _semanticModel.GetDeclaredSymbol(param)?.Type).ToArray();

        var foundSignature = new MethodSignature(methodName, typesOfParameters);

        if (foundSignature.Equals(signature))
        {
          return true;
        }
      }
    }

    return false;
  }
}
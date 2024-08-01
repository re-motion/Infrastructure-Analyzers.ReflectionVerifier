// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: MIT
using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;

namespace Remotion.Infrastructure.Analyzers.ReflectionVerifier;

public class AnalyzerInternal (SyntaxNodeAnalysisContext context)
{
  private readonly InvocationExpressionSyntax _node = (InvocationExpressionSyntax)context.Node;
  private readonly SemanticModel _semanticModel = context.SemanticModel;

  public Diagnostic? Analyze ()
  {
    if (_node.Expression is not MemberAccessExpressionSyntax memberAccessExpressionSyntax)
    {
      return null;
    }

    if (memberAccessExpressionSyntax.Name.ToString() is not nameof(InvokingMethods.CreateInstance))
    {
      return null;
    }

    MethodSignature calledSignature;
    try
    {
      calledSignature = GetCalledSignature(InvokingMethods.CreateInstance);
    }
    // ReSharper disable once RedundantCatchClause
    catch (NotSupportedException ex)
    {
      //return null;
      throw;
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

  private MethodSignature GetCalledSignature (InvokingMethods kind)
  {
    if (kind is InvokingMethods.CreateInstance)
    {
      var arguments = _node.ArgumentList.Arguments;
      var name = (arguments[0].Expression as TypeOfExpressionSyntax)?.Type.ToString();
      if (name is null)
      {
        throw new NotSupportedException("cannot look into variable with a roslyn analyzer");
      }

      var parameters = arguments.Select(
          arg =>
              _semanticModel.GetTypeInfo(arg.Expression).Type).Skip(1).ToArray();

      if (parameters.Any(p => p is null))
      {
        throw new Exception();
      }

      return new MethodSignature(name, parameters);
    }

    throw new NotSupportedException("");
  }
}
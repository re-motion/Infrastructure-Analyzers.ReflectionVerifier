// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
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

    if (_semanticModel.GetSymbolInfo(memberAccessExpressionSyntax).Symbol is not IMethodSymbol methodSymbol)
    {
      return null;
    }


    MethodSignature calledSignature;
    try
    {
      var signatureFinder = new SignatureFinder(context);
      calledSignature = signatureFinder.GetCalledSignature(methodSymbol);
    }
    catch (VariableException ex)
    {
      return null;
    }
    catch (NotSupportedException ex)
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
    var classSymbol = signature.ClassSymbol;
    var childNodes = classSymbol.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax().ChildNodes()
                     ?? throw new Exception("Could not get declaration of called method.");

    foreach (var possibleMethod in childNodes)
    {
      if (possibleMethod is BaseMethodDeclarationSyntax methodDeclarationSyntax)
      {
        var methodSymbol = _semanticModel.GetDeclaredSymbol(methodDeclarationSyntax);

        if (methodSymbol is null)
        {
          throw new Exception("could not get semantic model of method declaration");
        }

        var fullName = methodSymbol.ToDisplayString();

        if (fullName.Equals(signature.ToString()))
        {
          return true;
        }
      }
    }

    return false;
  }
}
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
  public readonly InvocationExpressionSyntax? InvocationExpressionNode = context.Node as InvocationExpressionSyntax;
  public readonly ObjectCreationExpressionSyntax? ObjectCreationExpressionNode = context.Node as ObjectCreationExpressionSyntax;
  public readonly SyntaxNode Node = context.Node;
  public readonly SemanticModel SemanticModel = context.SemanticModel;

  public Diagnostic? Analyze ()
  {
    var methodSymbol = GetMethodSymbol();

    if (methodSymbol is null)
    {
      return null;
    }

    MethodSignature? calledSignature;
    try
    {
      var signatureFinder = new SignatureFinder(context);
      calledSignature = signatureFinder.GetCalledSignature(methodSymbol);
    }
    catch (VariableException ex)
    {
      return null;
    }

    //not a reflection
    if (calledSignature is null)
    {
      return null;
    }

    var isValid = DoesExist(calledSignature);

    if (!isValid)
    {
      return Diagnostic.Create(Rules.Rule, Location.Create(Node.SyntaxTree, Node.Span));
    }

    return null;
  }

  private IMethodSymbol? GetMethodSymbol ()
  {
    if (InvocationExpressionNode is null)
    {
      if (Node is ObjectCreationExpressionSyntax)
      {
        return SemanticModel.GetSymbolInfo(Node).Symbol as IMethodSymbol;
      }

      return null;
    }


    if (InvocationExpressionNode.Expression is not MemberAccessExpressionSyntax memberAccessExpressionSyntax)
    {
      return null;
    }

    return SemanticModel.GetSymbolInfo(memberAccessExpressionSyntax).Symbol as IMethodSymbol;
  }
}
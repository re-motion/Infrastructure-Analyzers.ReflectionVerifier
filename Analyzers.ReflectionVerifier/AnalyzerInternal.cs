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

    var isValid = DoesExist(calledSignature.GetValueOrDefault());

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

  private bool DoesExist (MethodSignature signature)
  {
    var classSymbol = signature.OriginalDefinition;
    var childNodes = classSymbol.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax().ChildNodes()
                     ?? throw new Exception("Could not get declaration of called method.");

    foreach (var possibleMethod in childNodes)
    {
      if (possibleMethod is BaseMethodDeclarationSyntax methodDeclarationSyntax)
      {
        var methodSymbol = SemanticModel.GetDeclaredSymbol(methodDeclarationSyntax);

        if (methodSymbol is null)
        {
          throw new Exception("could not get semantic model of method declaration");
        }

        if (IsValidFor(signature, methodSymbol))
        {
          return true;
        }
      }
    }

    return false;
  }


  private bool IsValidFor (MethodSignature signature, IMethodSymbol targetMethod)
  {
    var targetSignature = MethodSignature.ParseMethodSymbol(targetMethod);

    if (!signature.NameInclusiveClass.Equals(targetSignature.NameInclusiveClass))
    {
      return false;
    }

    var argumentTypes = signature.Parameters;

    if (argumentTypes.Length != targetMethod.Parameters.Length)
    {
      return false;
    }

    for (var i = 0; i < argumentTypes.Length; i++)
    {
      var argumentType = argumentTypes[i];
      var parameterType = targetMethod.Parameters[i].Type;


      if (parameterType.TypeKind == TypeKind.TypeParameter)
      {
        var typeArguments = targetMethod.TypeArguments;

        //uses own generics
        if (typeArguments.Length > 0)
        {
          continue;
        }


        continue;
      }

      if (!IsAssignableTo(argumentType, parameterType, SemanticModel.Compilation))
      {
        return false;
      }
    }

    return true;
  }

  private bool IsAssignableTo (ITypeSymbol? sourceType, ITypeSymbol targetType, Compilation compilation)
  {
    if (sourceType is null)
    {
      return targetType.IsReferenceType;
    }

    var conversionIsValid = compilation.ClassifyConversion(sourceType, targetType).Exists;

    return sourceType.Equals(targetType, SymbolEqualityComparer.Default) || conversionIsValid;
  }
}
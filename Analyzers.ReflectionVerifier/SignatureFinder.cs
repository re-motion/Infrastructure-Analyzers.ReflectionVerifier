// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: MIT
using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Remotion.Infrastructure.Analyzers.ReflectionVerifier;

public partial class SignatureFinder (SyntaxNodeAnalysisContext context)
{
  private readonly InvocationExpressionSyntax? _invocationExpressionNode = context.Node as InvocationExpressionSyntax;
  private readonly ObjectCreationExpressionSyntax? _objectCreationExpressionNode = context.Node as ObjectCreationExpressionSyntax;
  private readonly SyntaxNode _node = context.Node;
  private readonly SemanticModel _semanticModel = context.SemanticModel;

  public MethodSignature? GetCalledSignature (IMethodSymbol methodSymbol)
  {
    var methodName = methodSymbol.OriginalDefinition.ToDisplayString();

    var kindOfMethod = s_methodNameToKind.TryGetValue(methodName, out var kind) ? kind : InvokingMethod.NotAReflection;

    return kindOfMethod switch
    {
        InvokingMethod.NotAReflection => null,
        InvokingMethod.CreateInstance => GetMethodSignatureCreateInstance(methodSymbol),
        InvokingMethod.InvokeMethod => GetMethodSignatureInvokeMethod(),
        InvokingMethod.CreateWithoutGeneric => GetMethodSignatureCreateWithoutGeneric(),
        InvokingMethod.CreateWithGeneric => GetMethodSignatureCreateWithGeneric(methodSymbol),
        InvokingMethod.LifetimeServiceNewObjectWithOutGeneric => GetMethodSignatureLifetimeServiceNewObjectWithOutGeneric(),
        InvokingMethod.DomainObjectNewObjectWithGeneric => GetMethodSignatureDomainObjectNewObjectWithGeneric(methodSymbol),
        InvokingMethod.MockGeneric => GetMethodSignatureMockGeneric(methodSymbol),
        InvokingMethod.MockSetup => GetMethodSignatureMockSetup(),
        _ => throw new NotSupportedException("Not supporting this kind of method")
    };
  }
}
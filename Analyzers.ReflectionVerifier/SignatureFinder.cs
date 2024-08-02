// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Remotion.Infrastructure.Analyzers.ReflectionVerifier;

public partial class SignatureFinder (SyntaxNodeAnalysisContext context)
{
  private readonly InvocationExpressionSyntax _node = (InvocationExpressionSyntax)context.Node;
  private readonly SemanticModel _semanticModel = context.SemanticModel;

  public MethodSignature GetCalledSignature (IMethodSymbol methodSymbol)
  {
    var methodName = GetMethodName(methodSymbol);

    var kindOfMethod = GetMethodKindByName(methodName);

    return kindOfMethod switch
    {
        InvokingMethod.CreateInstance => GetMethodSignatureCreateInstance(),
        InvokingMethod.InvokeMethod => GetMethodSignatureInvokeMethod(),
        InvokingMethod.CreateWithoutGeneric => GetMethodSignatureCreateWithoutGeneric(),
        InvokingMethod.CreateWithGeneric => GetMethodSignatureCreateWithGeneric(methodSymbol),
        _ => throw new NotSupportedException("not supporting this kind of method")
    };
  }

  private InvokingMethod GetMethodKindByName (string methodName)
  {
    InvokingMethod result;
    switch (methodName)
    {
      case "System.Activator.CreateInstance":
        result = InvokingMethod.CreateInstance;
        break;
      case "Remotion.Development.UnitTesting.PrivateInvoke.InvokePublicMethod":
      case "Remotion.Development.UnitTesting.PrivateInvoke.InvokeNonPublicMethod":
      case "Remotion.Development.UnitTesting.PrivateInvoke.InvokePublicStaticMethod":
      case "Remotion.Development.UnitTesting.PrivateInvoke.InvokeNonPublicStaticMethod":
        result = InvokingMethod.InvokeMethod;
        break;
      case "Remotion.Mixins.ObjectFactory.Create":
        result = InvokingMethod.CreateWithoutGeneric;
        break;
      case "Remotion.Mixins.ObjectFactory.Create<>":
        result = InvokingMethod.CreateWithGeneric;
        break;
      default:
        throw new NotSupportedException("This method is currently not supported.");
    }

    return result;
  }

  private static string GetMethodName (IMethodSymbol methodSymbol)
  {
    var arr = methodSymbol.TypeArguments.ToArray();
    List<string> stringArrayTypeParams = [];
    stringArrayTypeParams.AddRange(arr.Select(typeParameterSymbol => typeParameterSymbol.ToString()));

    var methodName = $"{methodSymbol.ContainingNamespace}.{methodSymbol.ContainingType.Name}.{methodSymbol.Name}";
    if (stringArrayTypeParams.Count > 0)
    {
      methodName += $"<>"; //{string.Join(", ", stringArrayTypeParams)}
    }

    return methodName;
  }
}
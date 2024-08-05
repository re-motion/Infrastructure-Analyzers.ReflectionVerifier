// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Remotion.Infrastructure.Analyzers.ReflectionVerifier;

public partial class SignatureFinder (SyntaxNodeAnalysisContext context)
{
  private readonly InvocationExpressionSyntax _node = (InvocationExpressionSyntax)context.Node;
  private readonly SemanticModel _semanticModel = context.SemanticModel;

  public MethodSignature? GetCalledSignature (IMethodSymbol methodSymbol)
  {
    var kindOfMethod = GetMethodKindByName(GetMethodName(methodSymbol));

    return kindOfMethod switch
    {
        InvokingMethod.NotAReflection => null,
        InvokingMethod.CreateInstance => GetMethodSignatureCreateInstance(),
        InvokingMethod.InvokeMethod => GetMethodSignatureInvokeMethod(),
        InvokingMethod.CreateWithoutGeneric => GetMethodSignatureCreateWithoutGeneric(),
        InvokingMethod.CreateWithGeneric => GetMethodSignatureCreateWithGeneric(methodSymbol),
        InvokingMethod.LifetimeServiceNewObjectWithOutGeneric => GetMethodSignatureLifetimeServiceNewObjectWithOutGeneric(),
        InvokingMethod.DomainObjectNewObjectWithGeneric => GetMethodSignatureDomainObjectNewObjectWithGeneric(methodSymbol),
        InvokingMethod.MockGeneric => GetMethodSignatureMockGeneric(methodSymbol),
        InvokingMethod.MockSetup => GetMethodSignatureMockSetup(),
        _ => throw new NotSupportedException("not supporting this kind of method")
    };
  }

  private InvokingMethod GetMethodKindByName (string methodName)
  {
    var result = methodName switch
    {
        "System.Activator.CreateInstance"
            => InvokingMethod.CreateInstance,
        "Remotion.Development.UnitTesting.PrivateInvoke.InvokePublicMethod"
            or "Remotion.Development.UnitTesting.PrivateInvoke.InvokeNonPublicMethod"
            or "Remotion.Development.UnitTesting.PrivateInvoke.InvokePublicStaticMethod"
            or "Remotion.Development.UnitTesting.PrivateInvoke.InvokeNonPublicStaticMethod"
            => InvokingMethod.InvokeMethod,
        "Remotion.Mixins.ObjectFactory.Create"
            => InvokingMethod.CreateWithoutGeneric,
        "Remotion.Mixins.ObjectFactory.Create<>"
            => InvokingMethod.CreateWithGeneric,
        "Remotion.Data.DomainObjects.DomainImplementation.LifetimeService.NewObject"
            => InvokingMethod.LifetimeServiceNewObjectWithOutGeneric,
        "Remotion.Data.DomainObjects.DomainObject.NewObject<>"
            => InvokingMethod.DomainObjectNewObjectWithGeneric,
        "Moq.Mock<>"
            => InvokingMethod.MockGeneric,
        "Moq.Protected.Setup"
            => InvokingMethod.MockSetup,

        _ => InvokingMethod.NotAReflection
    };

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
      methodName += "<>";
    }

    return methodName;
  }
}
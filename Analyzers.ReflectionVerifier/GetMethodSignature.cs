// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Remotion.Infrastructure.Analyzers.ReflectionVerifier;

public partial class SignatureFinder
{
  #region InvocationExpressionSyntax

  private MethodSignature GetMethodSignatureCreateInstance ()
  {
    var arguments = _invocationExpressionNode!.ArgumentList.Arguments.ToArray();
    var typeSymbol = GetTypeSymbolTypeOfExpression(arguments[0], out var genericsMap);
    var name = GetFullName(typeSymbol);
    var parameters = GetParameters(arguments.Skip(1).ToArray());

    return new MethodSignature(name, typeSymbol.OriginalDefinition, parameters, genericsMap);
  }

  private MethodSignature GetMethodSignatureInvokeMethod ()
  {
    var arguments = _invocationExpressionNode!.ArgumentList.Arguments.ToArray();

    var typeSymbol = IsStatic() ? GetTypeSymbolTypeOfExpression(arguments[0], out var genericsMap) : GetTypeSymbolFromVariable(arguments[0], out genericsMap);

    var parameters = GetParameters(arguments.Skip(2).ToArray());

    if (!(arguments[1].Expression as LiteralExpressionSyntax).IsKind(SyntaxKind.StringLiteralExpression))
    {
      throw new VariableException("Variable instead of literal string");
    }

    var name = typeSymbol.OriginalDefinition.ToDisplayString();
    name += "." + (arguments[1].Expression as LiteralExpressionSyntax)!.ToString()
        .Replace("\"", ""); // "Method" -> Method

    return new MethodSignature(name, typeSymbol.OriginalDefinition, parameters, genericsMap);
  }

  private MethodSignature GetMethodSignatureCreateWithGeneric (IMethodSymbol methodSymbol)
  {
    var name = GetFullNameGeneric(methodSymbol, out var originalDefinition, out var genericsMap);
    var arguments = _invocationExpressionNode!.ArgumentList.Arguments.ToArray();

    if (arguments.Length > 1)
    {
      throw new ArgumentException("ObjectFactory.Create<> or DomainObject.NewObject<> called with more than one argument.");
    }

    var paramListArgs = arguments.Length == 1 ? GetParamListArgs(arguments[0]) : [];
    var parameters = GetParameters(paramListArgs);

    return new MethodSignature(name, originalDefinition, parameters, genericsMap);
  }

  private MethodSignature GetMethodSignatureCreateWithoutGeneric ()
  {
    var arguments = _invocationExpressionNode!.ArgumentList.Arguments.ToArray();
    var typeSymbol = GetTypeSymbolTypeOfExpression(arguments[0], out var genericsMap);
    var fullName = GetFullName(typeSymbol);

    if (arguments.Length > 2)
    {
      throw new ArgumentException("ObjectFactory.Create called with more than two arguments.");
    }

    var paramListArgs = arguments.Length == 2 ? GetParamListArgs(arguments[1]) : [];
    var parameters = GetParameters(paramListArgs);

    return new MethodSignature(fullName, typeSymbol.OriginalDefinition, parameters, genericsMap);
  }

  private MethodSignature GetMethodSignatureLifetimeServiceNewObjectWithOutGeneric ()
  {
    var arguments = _invocationExpressionNode!.ArgumentList.Arguments.Skip(1).ToArray();
    var typeSymbol = GetTypeSymbolTypeOfExpression(arguments[0], out var genericsMap);
    var fullName = GetFullName(typeSymbol);

    if (arguments.Length > 2)
    {
      throw new ArgumentException("LifetimeService.NewObject called with more than three arguments.");
    }

    var paramListArgs = arguments.Length == 2 ? GetParamListArgs(arguments[1]) : [];
    var parameters = GetParameters(paramListArgs);

    return new MethodSignature(fullName, typeSymbol.OriginalDefinition, parameters, genericsMap);
  }

  private MethodSignature GetMethodSignatureDomainObjectNewObjectWithGeneric (IMethodSymbol methodSymbol)
  {
    // has same arguments as ObjectFactory.Create<>
    return GetMethodSignatureCreateWithGeneric(methodSymbol);
  }

  private MethodSignature GetMethodSignatureMockSetup ()
  {
    var arguments = _invocationExpressionNode!.ArgumentList.Arguments.ToArray();
    var parameters = GetParameters(arguments.Skip(1).ToArray());

    var methodSymbol = _semanticModel.GetSymbolInfo(_invocationExpressionNode!).Symbol as IMethodSymbol
                       ?? throw new Exception("Could not get semantic model of invocation expression");
    var returnType = methodSymbol.ReturnType;
    var tMockArr = (returnType as INamedTypeSymbol)?.TypeArguments.ToArray();

    if (tMockArr is null)
    {
      throw new VariableException("Could not get Return Type of Setup Method.");
    }

    if (tMockArr.Length != 1)
    {
      throw new Exception("Not a Setup method.");
    }

    var typeSymbol = tMockArr[0];

    var fullName = typeSymbol.OriginalDefinition.ToDisplayString() + "." + arguments[0].ToString()
        .Replace("\"", ""); // "Method" -> Method

    var genericsMap = new Dictionary<string, ITypeSymbol?>();

    if (typeSymbol is INamedTypeSymbol { IsGenericType: true } namedTypeSymbol)
    {
      var typeParameters = namedTypeSymbol.TypeParameters;
      var typeArguments = namedTypeSymbol.TypeArguments;
      for (var i = 0; i < typeParameters.Length; i++)
      {
        genericsMap[typeParameters[i].Name] = typeArguments[i];
      }
    }

    return new MethodSignature(fullName, typeSymbol.OriginalDefinition, parameters, genericsMap);
  }

  #endregion


  #region ObjectCreationSyntaxExpressions

  private MethodSignature GetMethodSignatureMockGeneric (IMethodSymbol methodSymbol)
  {
    var name = GetFullNameGeneric(methodSymbol, out var originalDefinition, out var genericsMap);
    var arguments = _objectCreationExpressionNode!.ArgumentList?.Arguments.ToArray();

    if (arguments is null)
    {
      throw new Exception("could not get arguments of ObjectCreationSyntax");
    }

    var parameters = GetParameters(arguments);

    return new MethodSignature(name, originalDefinition, parameters, genericsMap);
  }

  #endregion
}
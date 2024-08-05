// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: MIT
using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Remotion.Infrastructure.Analyzers.ReflectionVerifier;

public partial class SignatureFinder
{
  private MethodSignature GetMethodSignatureCreateInstance ()
  {
    var arguments = _node.ArgumentList.Arguments.ToArray();
    var typeSymbol = GetTypeSymbol(arguments);
    var name = GetFullName(typeSymbol);
    var parameters = GetParameters(arguments.Skip(1).ToArray());

    return new MethodSignature(name, typeSymbol.OriginalDefinition, parameters);
  }

  private MethodSignature GetMethodSignatureInvokeMethod ()
  {
    var arguments = _node.ArgumentList.Arguments.ToArray();
    var typeSymbol = GetTypeSymbol(arguments);
    var parameters = GetParameters(arguments.Skip(2).ToArray());

    if (!(arguments[1].Expression as LiteralExpressionSyntax).IsKind(SyntaxKind.StringLiteralExpression))
    {
      throw new VariableException("Variable instead of literal string");
    }

    var name = typeSymbol.ToDisplayString();
    name += "." + (arguments[1].Expression as LiteralExpressionSyntax)!.ToString()
        .Replace("\"", ""); // "Method" -> Method

    return new MethodSignature(name, typeSymbol.OriginalDefinition, parameters);
  }

  private MethodSignature GetMethodSignatureCreateWithGeneric (IMethodSymbol methodSymbol)
  {
    var name = GetFullNameGeneric(methodSymbol, out var classSymbol);
    var arguments = _node.ArgumentList.Arguments.ToArray();

    if (arguments.Length > 1)
    {
      throw new ArgumentException("ObjectFactory.Create<> or DomainObject.NewObject<> called with more than one argument.");
    }

    var paramListArgs = arguments.Length == 1 ? GetParamListArgs(arguments[0]) : [];
    var parameters = GetParameters(paramListArgs);

    return new MethodSignature(name, classSymbol, parameters);
  }

  private MethodSignature GetMethodSignatureCreateWithoutGeneric ()
  {
    var arguments = _node.ArgumentList.Arguments.ToArray();
    var typeSymbol = GetTypeSymbol(arguments);
    var fullName = GetFullName(typeSymbol);

    if (arguments.Length > 2)
    {
      throw new ArgumentException("ObjectFactory.Create called with more than two arguments.");
    }

    var paramListArgs = arguments.Length == 2 ? GetParamListArgs(arguments[1]) : [];
    var parameters = GetParameters(paramListArgs);

    return new MethodSignature(fullName, typeSymbol.OriginalDefinition, parameters);
  }

  private MethodSignature GetMethodSignatureLifetimeServiceNewObjectWithOutGeneric ()
  {
    var arguments = _node.ArgumentList.Arguments.Skip(1).ToArray();
    var typeSymbol = GetTypeSymbol(arguments);
    var fullName = GetFullName(typeSymbol);

    if (arguments.Length > 2)
    {
      throw new ArgumentException("LifetimeService.NewObject called with more than three arguments.");
    }

    var paramListArgs = arguments.Length == 2 ? GetParamListArgs(arguments[1]) : [];
    var parameters = GetParameters(paramListArgs);

    return new MethodSignature(fullName, typeSymbol.OriginalDefinition, parameters);
  }

  private MethodSignature GetMethodSignatureDomainObjectNewObjectWithGeneric (IMethodSymbol methodSymbol)
  {
    // has same arguments as ObjectFactory.Create<>
    return GetMethodSignatureCreateWithGeneric(methodSymbol);
  }

  private MethodSignature GetMethodSignatureMockGeneric (IMethodSymbol methodSymbol)
  {
    throw new NotImplementedException(); // is not an InvocationExpressionSyntax but an ObjectCreationExpressionSyntax
  }

  private MethodSignature GetMethodSignatureMockSetup ()
  {
    throw new NotImplementedException(); // not possible because namespace is not given in the params
  }
}
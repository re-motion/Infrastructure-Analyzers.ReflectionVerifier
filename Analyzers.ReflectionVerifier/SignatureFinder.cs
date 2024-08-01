// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: MIT
using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Remotion.Infrastructure.Analyzers.ReflectionVerifier;

public partial class AnalyzerInternal
{
  private MethodSignature GetCalledSignature (InvokingMethods kind)
  {
    return kind switch
    {
        InvokingMethods.CreateInstance => GetMethodSignatureCreateInstance(),
        InvokingMethods.InvokePublicMethod or InvokingMethods.InvokeNonPublicMethod => GetMethodSignatureInvokeMethod(),
        _ => throw new NotSupportedException("")
    };
  }

  private MethodSignature GetMethodSignatureInvokeMethod ()
  {
    var arguments = _node.ArgumentList.Arguments.Skip(1).ToArray();
    var isLiteralStringExpression = (arguments[0].Expression as LiteralExpressionSyntax).IsKind(SyntaxKind.StringLiteralExpression);
    if (!isLiteralStringExpression)
    {
      throw new NotSupportedException("cannot look into variable with a roslyn analyzer");
    }

    var name = (arguments[0].Expression as LiteralExpressionSyntax)!.ToString();
    name = name.Replace("\"", ""); // "Method" -> Method


    var parameters = GetParameters(arguments.Skip(1).ToArray());

    return new MethodSignature(name, parameters);
  }

  private MethodSignature GetMethodSignatureCreateInstance ()
  {
    var arguments = _node.ArgumentList.Arguments;
    var name = (arguments[0].Expression as TypeOfExpressionSyntax)?.Type.ToString();
    if (name is null)
    {
      throw new NotSupportedException("cannot look into variable with a roslyn analyzer");
    }

    var parameters = GetParameters(arguments.Skip(1).ToArray());

    return new MethodSignature(name, parameters);
  }

  private ITypeSymbol[] GetParameters (ArgumentSyntax[] arguments)
  {
    var parameters = arguments.Select(
        arg =>
            _semanticModel.GetTypeInfo(arg.Expression).Type).ToArray();

    if (parameters.Any(p => p is null))
    {
      throw new Exception("parameter type is null for some reason");
    }

    return parameters!;
  }
}
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
  private MethodSignature GetMethodSignatureCreateWithGeneric (IMethodSymbol methodSymbol)
  {
    var name = $"{GetTypeParam(methodSymbol, out var classSymbol)}";
    name += name.Substring(name.LastIndexOf(".", StringComparison.Ordinal));

    var arguments = _node.ArgumentList.Arguments.ToArray();

    if (arguments.Length > 1)
    {
      throw new ArgumentException("ObjectFactory.Create called with more than two arguments.");
    }

    var paramListArgs = arguments.Length == 1 ? GetParamListArgs(arguments[0]) : [];

    var parameters = GetParameters(paramListArgs);

    return new MethodSignature(name, classSymbol, parameters);
  }

  private MethodSignature GetMethodSignatureCreateWithoutGeneric ()
  {
    var arguments = _node.ArgumentList.Arguments.ToArray();


    var typeSyntax = (arguments[0].Expression as TypeOfExpressionSyntax)?.Type
                     ?? throw new VariableException("Variable instead of typeof([Class]).");

    var typeSymbol = _semanticModel.GetSymbolInfo(typeSyntax).Symbol as ITypeSymbol
                     ?? throw new VariableException("Cannot resolve type symbol.");

    var name = typeSymbol.ToDisplayString();

    var fullName = name + name.Substring(name.LastIndexOf(".", StringComparison.Ordinal));

    arguments = arguments.Skip(1).ToArray();

    if (arguments.Length > 1)
    {
      throw new ArgumentException("ObjectFactory.Create called with more than two arguments.");
    }

    var paramListArgs = arguments.Length == 1 ? GetParamListArgs(arguments[0]) : [];

    var parameters = GetParameters(paramListArgs);

    var classSymbol = typeSymbol.OriginalDefinition;

    return new MethodSignature(fullName, classSymbol, parameters);
  }

  private MethodSignature GetMethodSignatureInvokeMethod ()
  {
    var arguments = _node.ArgumentList.Arguments.ToArray();


    var typeSyntax = (arguments[0].Expression as TypeOfExpressionSyntax)?.Type
                     ?? throw new VariableException("Variable instead of typeof([Class]).");

    var typeSymbol = _semanticModel.GetSymbolInfo(typeSyntax).Symbol as ITypeSymbol
                     ?? throw new VariableException("Cannot resolve type symbol.");

    var name = typeSymbol.ToDisplayString();

    arguments = arguments.Skip(1).ToArray();

    name += "." + (arguments[0].Expression as LiteralExpressionSyntax)!.ToString()
        .Replace("\"", ""); // "Method" -> Method


    var isLiteralStringExpression = (arguments[0].Expression as LiteralExpressionSyntax).IsKind(SyntaxKind.StringLiteralExpression);
    if (!isLiteralStringExpression)
    {
      throw new VariableException("Variable instead of literal string");
    }


    var parameters = GetParameters(arguments.Skip(1).ToArray());

    var classSymbol = typeSymbol.OriginalDefinition;

    return new MethodSignature(name, classSymbol, parameters);
  }

  private MethodSignature GetMethodSignatureCreateInstance ()
  {
    var arguments = _node.ArgumentList.Arguments;
    var typeSyntax = (arguments[0].Expression as TypeOfExpressionSyntax)?.Type
                     ?? throw new VariableException("Variable instead of typeof([Class]).");

    var typeSymbol = _semanticModel.GetSymbolInfo(typeSyntax).Symbol as ITypeSymbol
                     ?? throw new VariableException("Cannot resolve type symbol.");

    var name = typeSymbol.ToDisplayString();
    if (name is null)
    {
      throw new VariableException("Variable instead of literal string");
    }

    name += name.Substring(name.LastIndexOf(".", StringComparison.Ordinal));

    var parameters = GetParameters(arguments.Skip(1).ToArray());


    var classSymbol = typeSymbol.OriginalDefinition;


    return new MethodSignature(name, classSymbol, parameters);
  }

  private ITypeSymbol[] GetParameters (ArgumentSyntax[] arguments)
  {
    if (arguments.Length == 0)
    {
      return [];
    }

    var parameters = arguments.Select(
        arg =>
            _semanticModel.GetTypeInfo(arg.Expression).Type).ToArray();

    if (parameters.Any(p => p is null))
    {
      throw new Exception("Parameter type is null for some reason.");
    }

    return parameters!;
  }

  private static ArgumentSyntax[] GetParamListArgs (ArgumentSyntax argument)
  {
    ArgumentSyntax[] paramListArgs;

    var invocationExpressionSyntaxParamList = argument.Expression as InvocationExpressionSyntax;
    var memberAccessExpressionSyntaxParamList = invocationExpressionSyntaxParamList?.Expression as MemberAccessExpressionSyntax;
    if (memberAccessExpressionSyntaxParamList is null)
    {
      memberAccessExpressionSyntaxParamList = argument.Expression as MemberAccessExpressionSyntax;
      if (memberAccessExpressionSyntaxParamList is null)
      {
        throw new ArgumentException("ObjectFactory.Create called with an Argument which is not ParamList.");
      }
    }

    var paramListAccessName = memberAccessExpressionSyntaxParamList.Name.ToString();

    if (paramListAccessName.Equals("Empty"))
    {
      paramListArgs = [];
    }
    else if (paramListAccessName.Equals("Create"))
    {
      paramListArgs = invocationExpressionSyntaxParamList?.ArgumentList.Arguments.ToArray()
                      ?? throw new ArgumentException("ObjectFactory.Create called with ParamList.Create as a Field (ParamList has Create field ???)");
    }
    else
    {
      throw new ArgumentException($"ObjectFactory.Create called with Argument ParamList.{paramListAccessName}, should only be called with ParamList.Empty or ParamList.Create.");
    }

    return paramListArgs;
  }

  private static string GetTypeParam (IMethodSymbol methodSymbol, out ITypeSymbol classSymbol)
  {
    var typeSymbols = methodSymbol.TypeArguments.ToArray();

    if (typeSymbols.Length != 1)
    {
      if (typeSymbols.Length > 1)
      {
        throw new ArgumentException("Method has more than one type parameter.");
      }
      else
      {
        throw new ArgumentException("Method does not have a type parameter.");
      }
    }

    var typeSymbol = typeSymbols[0];
    var stringArrayTypeParams = typeSymbol.ToString();

    classSymbol = typeSymbol.OriginalDefinition;
    return $"{stringArrayTypeParams}";
  }
}
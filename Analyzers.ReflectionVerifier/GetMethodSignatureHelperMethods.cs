// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: MIT
using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Remotion.Infrastructure.Analyzers.ReflectionVerifier;

public partial class SignatureFinder
{
  private ITypeSymbol GetTypeSymbolTypeOfExpression (ArgumentSyntax[] arguments)
  {
    var typeSyntax = (arguments[0].Expression as TypeOfExpressionSyntax)?.Type
                     ?? throw new VariableException("Variable instead of typeof([Class]).");

    var typeSymbol = _semanticModel.GetSymbolInfo(typeSyntax).Symbol as ITypeSymbol
                     ?? throw new VariableException("Cannot resolve type symbol.");
    return typeSymbol;
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

  private static string GetTypeParam (IMethodSymbol methodSymbol, out ITypeSymbol originalDefinition)
  {
    var methodKind = methodSymbol.MethodKind;
    ITypeSymbol[] typeSymbols;
    if (methodKind is MethodKind.Constructor)
    {
      typeSymbols = methodSymbol.ContainingType.TypeArguments.ToArray();
    }
    else
    {
      typeSymbols = methodSymbol.TypeArguments.ToArray();
    }

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
    originalDefinition = typeSymbol.OriginalDefinition;
    var typeParam = originalDefinition.ToString();

    return $"{typeParam}";
  }


  private static string GetFullNameGeneric (IMethodSymbol methodSymbol, out ITypeSymbol originalDefinition)
  {
    var name = $"{GetTypeParam(methodSymbol, out originalDefinition)}";
    var lastIndexOfPoint = name.LastIndexOf(".", StringComparison.Ordinal);
    if (lastIndexOfPoint == -1)
    {
      throw new Exception("Namespace of type does not have a . .");
    }

    var firstIndexOfLessThan = name.IndexOf("<", StringComparison.Ordinal);

    if (firstIndexOfLessThan == -1)
    {
      firstIndexOfLessThan = name.Length;
    }

    name += name.Substring(lastIndexOfPoint, firstIndexOfLessThan - lastIndexOfPoint);
    return name;
  }


  private static string GetFullName (ITypeSymbol typeSymbol)
  {
    var name = typeSymbol.OriginalDefinition.ToDisplayString();
    var fullName = name + name.Substring(name.LastIndexOf(".", StringComparison.Ordinal));
    return fullName;
  }
}
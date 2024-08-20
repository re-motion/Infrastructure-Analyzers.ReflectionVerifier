// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Remotion.Infrastructure.Analyzers.ReflectionVerifier;

public partial class SignatureFinder
{
  private ITypeSymbol GetTypeSymbolTypeOfExpression (ArgumentSyntax argument, out Dictionary<string, ITypeSymbol?> genericsMap)
  {
    
    var typeSyntax = (argument.Expression as TypeOfExpressionSyntax)?.Type
                     ?? throw new VariableException("Variable instead of typeof([Class]).");

    var typeSymbol = _semanticModel.GetSymbolInfo(typeSyntax).Symbol as ITypeSymbol
                     ?? throw new VariableException("Cannot resolve type symbol.");

    genericsMap = new Dictionary<string, ITypeSymbol?>();

    if (_semanticModel.GetTypeInfo(typeSyntax).Type is INamedTypeSymbol namedTypeSymbol)
    {
      var typeParameters = namedTypeSymbol.TypeParameters;
      var typeArguments = namedTypeSymbol.TypeArguments;
      for (var i = 0; i < typeParameters.Length; i++)
      {
        genericsMap[typeParameters[i].Name] = typeArguments[i];
      }
    }

    return typeSymbol;
  }
  
  private ITypeSymbol GetTypeSymbolFromVariable(
      ArgumentSyntax argument, 
      out Dictionary<string, ITypeSymbol?> genericsMap)
  {
    // Ensure we have an expression
    if (argument.Expression == null)
    {
      throw new VariableException("Argument expression is null.");
    }

    // Get the type information from the semantic model
    var typeInfo = _semanticModel.GetTypeInfo(argument.Expression);
    var typeSymbol = typeInfo.Type 
                     ?? throw new VariableException("Cannot resolve type symbol from the variable.");

    genericsMap = new Dictionary<string, ITypeSymbol?>();

    // If the type is a named type (possibly generic)
    if (typeSymbol is INamedTypeSymbol namedTypeSymbol)
    {
      var typeParameters = namedTypeSymbol.TypeParameters;
      var typeArguments = namedTypeSymbol.TypeArguments;
      for (var i = 0; i < typeParameters.Length; i++)
      {
        genericsMap[typeParameters[i].Name] = typeArguments[i];
      }
    }

    return typeSymbol;
  }
  
  
  private bool IsStatic ()
  {
    var expression = _invocationExpressionNode!.Expression;

    // Check if it's a member access expression (e.g., ClassName.MethodName)
    if (expression is MemberAccessExpressionSyntax memberAccessExpression)
    {
      // Extract the method name and check if it contains "Static"
      var methodName = memberAccessExpression.Name.Identifier.Text;
      return methodName.Contains("Static");
    }
    // Check if it's a simple identifier (e.g., MethodName)
    else if (expression is IdentifierNameSyntax identifierName)
    {
      // Extract the method name and check if it contains "Static"
      var methodName = identifierName.Identifier.Text;
      return methodName.Contains("Static");
    }

    // Return false if neither case applies
    return false;
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


  private static string GetFullNameGeneric (IMethodSymbol methodSymbol, out ITypeSymbol originalDefinition, out Dictionary<string, ITypeSymbol?> genericMap)
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


    genericMap = new Dictionary<string, ITypeSymbol?>();
    if (originalDefinition is INamedTypeSymbol { IsGenericType: true } namedTypeSymbol)
    {
      var typeParameters = namedTypeSymbol.TypeParameters;
      var typeArguments = namedTypeSymbol.TypeArguments;
      for (var i = 0; i < typeParameters.Length; i++)
      {
        genericMap[typeParameters[i].Name] = typeArguments[i];
      }
    }

    return name;
  }


  private static string GetFullName (ITypeSymbol typeSymbol)
  {
    var name = typeSymbol.OriginalDefinition.ToDisplayString();
    var fullName = name + name.Substring(name.LastIndexOf(".", StringComparison.Ordinal));
    return fullName;
  }
}
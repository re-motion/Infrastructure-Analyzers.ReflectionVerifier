// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: MIT
using System;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using Exception = System.Exception;

namespace Remotion.Infrastructure.Analyzers.ReflectionVerifier;

public partial class AnalyzerInternal
{
  private bool DoesExist (MethodSignature signature)
  {
    var classSymbol = signature.OriginalClassDefinition;

    var metadataName = GetFullMetadataName(classSymbol);

    var compilation = context.SemanticModel.Compilation;
    compilation = compilation.WithOptions(compilation.Options.WithMetadataImportOptions(MetadataImportOptions.All));
    //TODO: get all members not just public and protected (line above does not work)

    var namedTypeSymbol = compilation.GetTypeByMetadataName(metadataName)
                          ?? throw new Exception("Could not get INamedType of originalDefinition");

    var members = namedTypeSymbol.GetMembers();

    foreach (var possibleMethod in members)
    {
      if (possibleMethod is IMethodSymbol methodSymbol)
      {
        if (IsValidFor(signature, methodSymbol))
        {
          return true;
        }
      }
    }

    return false;
  }

  public static string GetFullMetadataName (ISymbol s)
  {
    if (IsRootNamespace(s))
    {
      return string.Empty;
    }

    var sb = new StringBuilder(s.MetadataName);
    var last = s;

    s = s.ContainingSymbol;

    while (!IsRootNamespace(s))
    {
      if (s is ITypeSymbol && last is ITypeSymbol)
      {
        sb.Insert(0, '+');
      }
      else
      {
        sb.Insert(0, '.');
      }

      sb.Insert(0, s.OriginalDefinition.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
      //sb.Insert(0, s.MetadataName);
      s = s.ContainingSymbol;
    }

    return sb.ToString();

    bool IsRootNamespace (ISymbol symbol)
    {
      INamespaceSymbol? namespaceSymbol;
      return ((namespaceSymbol = symbol as INamespaceSymbol) != null) && namespaceSymbol.IsGlobalNamespace;
    }
  }


  private bool IsValidFor (MethodSignature signature, IMethodSymbol targetMethod)
  {
    var name = targetMethod.MethodKind is MethodKind.Constructor
        ? $"{targetMethod.ContainingType.ToDisplayString()}.{targetMethod.ContainingType.Name}"
        : $"{targetMethod.ContainingType.ToDisplayString()}.{targetMethod.Name}";

    if (!signature.NameInclusiveClass.Equals(name))
    {
      return false;
    }

    var argumentTypes = signature.Parameters;

    if (argumentTypes.Length != targetMethod.Parameters.Length)
    {
      return false;
    }

    //describes the TypeParameterConstraintClauses (the conditions, the generics have to meet)
    var genericsMap = signature.GenericsMap;

    for (var i = 0; i < argumentTypes.Length; i++)
    {
      var argumentType = argumentTypes[i];
      var parameterType = targetMethod.Parameters[i].Type;


      if (parameterType.TypeKind == TypeKind.TypeParameter)
      {
        //uses own generics, which the methods that are covered are not supporting -> anything is ok, should not be possible
        if (targetMethod.TypeArguments.Length > 0)
        {
          continue;
        }

        //replace parameter type with the type mentioned in the TypeParameterConstraintClause (= z. B.: where T is Test)
        parameterType = genericsMap[parameterType.ToString()];
      }

      if (!IsAssignableTo(argumentType, parameterType))
      {
        return false;
      }
    }

    return true;
  }

  private bool IsAssignableTo (ITypeSymbol? sourceType, ITypeSymbol? targetType)
  {
    var compilation = SemanticModel.Compilation;

    if (targetType is null)
    {
      return true; // in the case of a generic parameter without a constraint clause
    }

    if (sourceType is null)
    {
      return targetType.IsReferenceType;
    }

    var conversionIsValid = compilation.ClassifyConversion(sourceType, targetType).Exists;

    return sourceType.Equals(targetType, SymbolEqualityComparer.Default) || conversionIsValid;
  }
}
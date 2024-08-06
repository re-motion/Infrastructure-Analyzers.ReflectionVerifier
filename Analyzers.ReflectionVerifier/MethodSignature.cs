// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: MIT
using System;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Remotion.Infrastructure.Analyzers.ReflectionVerifier;

public readonly struct MethodSignature (string nameInclusiveClass, ITypeSymbol originalDefinition, ITypeSymbol?[] parameters)
{
  public ITypeSymbol OriginalDefinition { get; } = originalDefinition;
  public string NameInclusiveClass { get; } = nameInclusiveClass;
  public ITypeSymbol?[] Parameters { get; } = parameters;


  public static MethodSignature ParseMethodSymbol (IMethodSymbol methodSymbol)
  {
    var name = GetClassName(methodSymbol);
    var definition = GetTypeSymbol(methodSymbol);
    var parametersLocal = GetParameters(methodSymbol);
    return new MethodSignature(name, definition, parametersLocal);
  }

  private static ITypeSymbol?[] GetParameters (IMethodSymbol methodSymbol)
  {
    return methodSymbol.Parameters.Select(p => p.Type).ToArray();
  }

  private static ITypeSymbol GetTypeSymbol (IMethodSymbol methodSymbol)
  {
    return methodSymbol.ContainingType;
  }

  private static string GetClassName (IMethodSymbol methodSymbol)
  {
    if (methodSymbol.MethodKind is MethodKind.Constructor)
    {
      return $"{methodSymbol.ContainingType.ToDisplayString()}.{methodSymbol.ContainingType.Name}";
    }

    return $"{methodSymbol.ContainingType.ToDisplayString()}.{methodSymbol.Name}";
  }

  public static bool operator == (MethodSignature left, MethodSignature right)
  {
    return left.Equals(right);
  }

  public static bool operator != (MethodSignature left, MethodSignature right)
  {
    return !(left == right);
  }

  public override bool Equals (object? o)
  {
    return o is MethodSignature other && Equals(other);
  }

  private bool Equals (MethodSignature other)
  {
    return NameInclusiveClass.Equals(other.NameInclusiveClass) &&
           Parameters.SequenceEqual(other.Parameters, SymbolEqualityComparer.Default) &&
           SymbolEqualityComparer.Default.Equals(OriginalDefinition, other.OriginalDefinition);
  }

  public override int GetHashCode ()
  {
    return NameInclusiveClass.GetHashCode() ^ Parameters.GetHashCode() ^ OriginalDefinition.GetHashCode();
  }

  public override string ToString ()
  {
    return $"{NameInclusiveClass}({string.Join(", ", Parameters.Select(param => param.ToString()))})";
  }
}
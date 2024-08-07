// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: MIT
using System;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Remotion.Infrastructure.Analyzers.ReflectionVerifier;

public class MethodSignature (string nameInclusiveClass, ITypeSymbol originalDefinition, ITypeSymbol?[] parameters)
{
  public ITypeSymbol OriginalDefinition { get; } = originalDefinition;
  public string NameInclusiveClass { get; } = nameInclusiveClass;
  public ITypeSymbol?[] Parameters { get; } = parameters;


  public static MethodSignature ParseMethodSymbol (IMethodSymbol methodSymbol)
  {
    var name = methodSymbol.MethodKind is MethodKind.Constructor
        ? $"{methodSymbol.ContainingType.ToDisplayString()}.{methodSymbol.ContainingType.Name}"
        : $"{methodSymbol.ContainingType.ToDisplayString()}.{methodSymbol.Name}";

    var definition = methodSymbol.ContainingType;

    var parametersLocal = methodSymbol.Parameters.Select(p => p.Type).ToArray();


    return new MethodSignature(name, definition, parametersLocal);
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
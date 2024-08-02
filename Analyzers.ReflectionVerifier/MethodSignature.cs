// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: MIT
using System;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Remotion.Infrastructure.Analyzers.ReflectionVerifier;

public readonly struct MethodSignature (string nameInclusiveClass, ITypeSymbol classSymbol, ITypeSymbol?[] parameters)
{
  public ITypeSymbol ClassSymbol { get; } = classSymbol;
  public string NameInclusiveClass { get; } = nameInclusiveClass;
  public ITypeSymbol?[] Parameters { get; } = parameters;

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
    return NameInclusiveClass == other.NameInclusiveClass && Parameters.SequenceEqual(other.Parameters) && ClassSymbol.Equals(other.ClassSymbol);
  }

  public override int GetHashCode ()
  {
    return NameInclusiveClass.GetHashCode() ^ Parameters.GetHashCode() ^ ClassSymbol.GetHashCode();
  }

  public override string ToString ()
  {
    return $"{NameInclusiveClass}({string.Join(", ", Parameters.Select(param => param.ToString()))})";
  }
}
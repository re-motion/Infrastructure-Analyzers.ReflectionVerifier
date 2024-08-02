// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: MIT
using System;

namespace Remotion.Infrastructure.Analyzers.ReflectionVerifier;

/// <summary>
/// Is used to describe a case where a variable is used in a Reflection, which is not possible to analyze before runtime.
/// </summary>
[Serializable]
internal class VariableException : Exception
{
  public VariableException ()
  {
  }

  public VariableException (string name)
      : base($"Cannot Look into Variable: {name}")
  {
  }
}
// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: MIT

using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

namespace Remotion.Infrastructure.Analyzers.ReflectionVerifier.UnitTests;

public static class CSharpAnalyzerVerifier<TAnalyzer>
  where TAnalyzer : DiagnosticAnalyzer, new()
{
  public static DiagnosticResult Diagnostic(DiagnosticDescriptor desc)
  {
    return CSharpAnalyzerVerifier<TAnalyzer, DefaultVerifier>.Diagnostic(desc);
  }

  public static Task VerifyAnalyzerAsync(string source, params DiagnosticResult[] expected)
  {

    var test = new Test
    {
      TestCode = source,

      SolutionTransforms =
      {
        (solution, id) =>
        {
          var project = solution.GetProject(id)!;
          return project.Solution;
        }
      }
    };
    test.ExpectedDiagnostics.AddRange(expected);

    return test.RunAsync();
  }

  private class Test : CSharpAnalyzerTest<TAnalyzer, DefaultVerifier>;
}
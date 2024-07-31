// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: MIT

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace Remotion.Infrastructure.Analyzers.ReflectionVerifier.UnitTests;

public class ReflectionTest
{
  [Fact]
  public async Task Test()
  {
    const string text =
      @"
//reflection case
      ";
    var expected = DiagnosticResult.EmptyDiagnosticResults;
    await CSharpAnalyzerVerifier<ReflectionAnalyzer>.VerifyAnalyzerAsync(text, expected);
	//or
	/*
	var expected = CSharpAnalyzerVerifier<ReflectionAnalyzer>.Diagnostic(ReflectionAnalyzer.Rule)
        .WithLocation(14, 26)
        .WithArguments("Test");
    await CSharpAnalyzerVerifier<ReflectionAnalyzer>.VerifyAnalyzerAsync(text, expected);
	*/
  }
}
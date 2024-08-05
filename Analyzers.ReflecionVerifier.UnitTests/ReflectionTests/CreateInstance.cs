// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: MIT

using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace Remotion.Infrastructure.Analyzers.ReflectionVerifier.UnitTests.ReflectionTests;

public class ReflectionTestCreateInstance
{
  [Fact]
  public async Task ReflectionCallCorrect ()
  {
    const string text =
        @"
using System;
using System.Reflection;

namespace ConsoleApp1;

public class Test
{
  public void TestMethod (int a)
  {
  }

  public Test (string a, int b)
  {
  }

  public void Test2 ()
  {
    Activator.CreateInstance(typeof(Test), ""foo"", 42);
    var x = new Test(""foo"", 42);
  }
}
      ";
    var expected = DiagnosticResult.EmptyDiagnosticResults;
    await CSharpAnalyzerVerifier<ReflectionAnalyzer>.VerifyAnalyzerAsync(text, expected);
  }

  [Fact]
  public async Task ReflectionCallWrong ()
  {
    const string text =
        @"
using System;
using System.Reflection;

namespace ConsoleApp1;

public class Test
{
  public Test (string a, int b)
  {
  }

  public void TestMethod (int a)
  {
  }

  public void Test2 ()
  {
    Activator.CreateInstance(typeof(Test), ""foo"", ""42"");
    var x = new Test(""foo"", 42);
  }
}
      ";

    var expected = CSharpAnalyzerVerifier<ReflectionAnalyzer>.Diagnostic(Rules.Rule)
        .WithLocation(19, 5)
        .WithArguments("Test");
    await CSharpAnalyzerVerifier<ReflectionAnalyzer>.VerifyAnalyzerAsync(text, expected);
  }
}
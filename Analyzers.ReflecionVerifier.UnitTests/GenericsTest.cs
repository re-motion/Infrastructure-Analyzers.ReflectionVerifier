// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: MIT
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace Remotion.Infrastructure.Analyzers.ReflectionVerifier.UnitTests;

public class GenericsTest
{
  [Fact]
  public async Task GenericsTest_ReflectionCallCorrect ()
  {
    const string text =
        """
        using System;
        using Moq.Protected;

        namespace ConsoleApp1;

        public class Test<T>
        {
          public Test (string a, int b)
          {
          }
        
          public void TestMethod (T a)
          {
          }
        
          public void Test3 ()
          {
            var v = new Moq.Mock<Test<string>>("foo", 3);
            v.Protected().Setup("TestMethod", "foo");
          }
        }
        """;
    var expected = DiagnosticResult.EmptyDiagnosticResults;
    await CSharpAnalyzerVerifier<ReflectionAnalyzer>.VerifyAnalyzerAsync(text, expected);
  }
  [Fact]
  public async Task GenericsTest_ReflectionCallWrong ()
  {
    const string text =
        """
        using System;
        using Moq.Protected;
        
        namespace ConsoleApp1;
        
        public class Test<T>
        {
          public Test (string a, int b)
          {
          }
        
          public void TestMethod (T a)
          {
          }
        
          public void Test3 ()
          {
            var v = new Moq.Mock<Test<string>>("foo", 3);
            v.Protected().Setup("TestMethod", 42);
          }
        }   
        """;

    var expected = CSharpAnalyzerVerifier<ReflectionAnalyzer>.Diagnostic(Rules.Rule)
        .WithLocation(21, 7)
        .WithArguments("Test");
    await CSharpAnalyzerVerifier<ReflectionAnalyzer>.VerifyAnalyzerAsync(text, expected);
  }
}
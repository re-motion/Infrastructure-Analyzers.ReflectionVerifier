// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: MIT
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace Remotion.Infrastructure.Analyzers.ReflectionVerifier.UnitTests.ReflectionTests;

public class MoqSetup
{
  [Fact]
  public async Task ReflectionCallCorrect ()
  {
    const string text =
        """
        using System;
        using Moq.Protected;

        namespace ConsoleApp1;

        public class Test
        {
          public Test (string a, int b)
          {
          }
        
          public void TestMethod (int a)
          {
          }
        
          public void Test3 ()
          {
            var v = new Moq.Mock<Test>("foo", 3);
            v.Protected().Setup("TestMethod", 3);
          }
        } 
        """;
    var expected = DiagnosticResult.EmptyDiagnosticResults;
    await CSharpAnalyzerVerifier<ReflectionAnalyzer>.VerifyAnalyzerAsync(text, expected);
  }

  [Fact]
  public async Task ReflectionCallWrong ()
  {
    const string text =
        """
        using System;
        using Moq.Protected;

        namespace ConsoleApp1;

        public class Test
        {
          public Test (string a, int b)
          {
          }
        
          public void TestMethod (int a)
          {
          }
        
          public void Test3 ()
          {
            var v = new Moq.Mock<Test>("foo", 3);
            v.Protected().Setup("TestMethod", "foo", 3, 7);
          }
        }  
        """;

    var expected = CSharpAnalyzerVerifier<ReflectionAnalyzer>.Diagnostic(Rules.Rule)
        .WithLocation(19, 5)
        .WithArguments("Test");
    await CSharpAnalyzerVerifier<ReflectionAnalyzer>.VerifyAnalyzerAsync(text, expected);
  }
}
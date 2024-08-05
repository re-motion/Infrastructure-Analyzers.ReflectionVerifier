// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: MIT
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace Remotion.Infrastructure.Analyzers.ReflectionVerifier.UnitTests.ReflectionTests;

public class MoqMock
{
  [Fact]
  public async Task ReflectionCallCorrect ()
  {
    const string text =
        """
        using System;

        namespace ConsoleApp1;

        public class Test
        {
          public Test (string a, int b)
          {
          }
        
          public void TestMethod (string a, int b)
          {
          }
        
          public void Test2 ()
          {
            var v = new Moq.Mock<Test>("foo", 3);
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

        namespace ConsoleApp1;

        public class Test
        {
          public Test ()
          {
          }
        
          public void TestMethod (string a, int b)
          {
          }
        
          public void Test2 ()
          {
            var v = new Moq.Mock<Test>("foo", 3, 5);
          }
        }    
        """;

    var expected = CSharpAnalyzerVerifier<ReflectionAnalyzer>.Diagnostic(Rules.Rule)
        .WithLocation(17, 13)
        .WithArguments("Test");
    await CSharpAnalyzerVerifier<ReflectionAnalyzer>.VerifyAnalyzerAsync(text, expected);
  }
}
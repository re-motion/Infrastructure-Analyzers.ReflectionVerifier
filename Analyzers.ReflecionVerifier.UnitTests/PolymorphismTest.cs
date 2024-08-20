// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: MIT
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace Remotion.Infrastructure.Analyzers.ReflectionVerifier.UnitTests;

public class PolymorphismTest
{
  [Fact]
  public async Task PolymorphismTest_ReflectionCallCorrect ()
  {
    const string text =
        """
        using System;
        using System.Reflection;
        using Remotion.Development.UnitTesting;

        namespace ConsoleApp1;

        public class Test
        {
          public Test (string a, int b)
          {
          }
        
          public void TestMethod<T> (T a)
          {
          }
        
          public void Test3 ()
          {
            var test = new Test("", 4);
            PrivateInvoke.InvokePublicMethod(test, "TestMethod", 42);
          }
        }
              
        """;
    var expected = DiagnosticResult.EmptyDiagnosticResults;
    await CSharpAnalyzerVerifier<ReflectionAnalyzer>.VerifyAnalyzerAsync(text, expected);
  }
}
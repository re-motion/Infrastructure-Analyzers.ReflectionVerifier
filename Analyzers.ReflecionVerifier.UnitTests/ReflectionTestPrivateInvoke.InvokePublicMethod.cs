// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: MIT

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace Remotion.Infrastructure.Analyzers.ReflectionVerifier.UnitTests;

public class ReflectionTestPrivateInvoke_InvokePublicMethod
{
  [Fact]
  public async Task ReflectionCallCorrect ()
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
        
          public void TestMethod (int a)
          {
          }
        
          public void Test3 ()
          {
            
            PrivateInvoke.InvokePublicMethod(typeof(Test), "TestMethod", 42);
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
        using System.Reflection;
        using Remotion.Development.UnitTesting;

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
            
            PrivateInvoke.InvokePublicMethod(typeof(Test), "TestMethod", 42, "foo");
          }
        }
              
        """;

    var expected = CSharpAnalyzerVerifier<ReflectionAnalyzer>.Diagnostic(Rules.Rule)
        .WithLocation(20, 5)
        .WithArguments("Test");
    await CSharpAnalyzerVerifier<ReflectionAnalyzer>.VerifyAnalyzerAsync(text, expected);
  }
}
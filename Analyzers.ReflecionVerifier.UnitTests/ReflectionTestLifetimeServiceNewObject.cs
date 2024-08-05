// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: MIT
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace Remotion.Infrastructure.Analyzers.ReflectionVerifier.UnitTests;

public class ReflectionTestLifetimeServiceNewObject
{
  [Fact]
  public async Task ParamListEmpty_ReflectionCallCorrect ()
  {
    const string text =
        """

        using System;
        using System.Reflection;
        using Remotion.Data.DomainObjects.DomainImplementation;
        using Remotion.TypePipe;

        namespace ConsoleApp1;

        public class Test
        {
          public void TestMethod (int a)
          {
          }
        
          public Test ()
          {
          }
        
          public void Test2 ()
          {
            LifetimeService.NewObject(null, typeof(Test), ParamList.Empty);
          }
        }
              
        """;
    var expected = DiagnosticResult.EmptyDiagnosticResults;
    await CSharpAnalyzerVerifier<ReflectionAnalyzer>.VerifyAnalyzerAsync(text, expected);
  }

  [Fact]
  public async Task ParamListEmpty_ReflectionCallWrong ()
  {
    const string text =
        """

        using System;
        using System.Reflection;
        using Remotion.Data.DomainObjects.DomainImplementation;
        using Remotion.TypePipe;

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
            LifetimeService.NewObject(null, typeof(Test), ParamList.Empty);
          }
        }
              
        """;

    var expected = CSharpAnalyzerVerifier<ReflectionAnalyzer>.Diagnostic(Rules.Rule)
        .WithLocation(21, 5)
        .WithArguments("Test");
    await CSharpAnalyzerVerifier<ReflectionAnalyzer>.VerifyAnalyzerAsync(text, expected);
  }

  [Fact]
  public async Task ParamListCreate_ReflectionCallCorrect ()
  {
    const string text =
        """

        using System;
        using System.Reflection;
        using Remotion.Data.DomainObjects.DomainImplementation;
        using Remotion.TypePipe;

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
            LifetimeService.NewObject(null, typeof(Test), ParamList.Create("foo", 42));
          }
        }
              
        """;
    var expected = DiagnosticResult.EmptyDiagnosticResults;
    await CSharpAnalyzerVerifier<ReflectionAnalyzer>.VerifyAnalyzerAsync(text, expected);
  }

  [Fact]
  public async Task ParamListCreate_ReflectionCallWrong ()
  {
    const string text =
        """

        using System;
        using System.Reflection;
        using Remotion.Data.DomainObjects.DomainImplementation;
        using Remotion.TypePipe;

        namespace ConsoleApp1;

        public class Test
        {
          public Test ()
          {
          }
        
          public void TestMethod (int a)
          {
          }
        
          public void Test2 ()
          {
            LifetimeService.NewObject(null, typeof(Test), ParamList.Create("foo", 42));
          }
        }
              
        """;

    var expected = CSharpAnalyzerVerifier<ReflectionAnalyzer>.Diagnostic(Rules.Rule)
        .WithLocation(21, 5)
        .WithArguments("Test");
    await CSharpAnalyzerVerifier<ReflectionAnalyzer>.VerifyAnalyzerAsync(text, expected);
  }
}
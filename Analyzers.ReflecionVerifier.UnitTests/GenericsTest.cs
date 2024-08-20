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

  [Fact(Skip = "not implemented")]
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

  [Fact]
  public async Task GenericsTest_ParameterNotMatchingWhereCondition ()
  {
    const string text =
        """
        using System;
        using Remotion.Development.UnitTesting;

        namespace ConsoleApp1;

        public class Test<T, T2> where T : DerivedTest where T2 : class
        {
          public Test (string a, int b)
          {
          }
        
          public void TestMethod (T a)
          {
          }
        
          public static void Main (string[] args)
          {
          var test = new Test<T, T2>("asdf", 4);
            PrivateInvoke.InvokePublicMethod(test, "TestMethod", "foo");
          }
        }
        public class Test2
        {
        }
        public class DerivedTest : Test2
        {
        }
        public class SecondDerivedTest : DerivedTest
        {
        }
        """;
    var expected = CSharpAnalyzerVerifier<ReflectionAnalyzer>.Diagnostic(Rules.Rule)
        .WithLocation(19, 5)
        .WithArguments("Test");
    await CSharpAnalyzerVerifier<ReflectionAnalyzer>.VerifyAnalyzerAsync(text, expected);
  }

  [Fact]
  public async Task GenericsTest_ParameterMatchingWhereCondition ()
  {
    const string text =
        """
        using System;
        using Remotion.Development.UnitTesting;

        namespace ConsoleApp1;

        public class Test<T, T2> where T : DerivedTest where T2 : class
        {
          public Test (string a, int b)
          {
          }
        
          public void TestMethod (T a)
          {
          }
        
          public static void Main (string[] args)
          {
            var test = new Test<T, T2>("asdf", 4);
            PrivateInvoke.InvokePublicMethod(test, "TestMethod", new DerivedTest());
          }
        }
        public class Test2
        {
        }
        public class DerivedTest : Test2
        {
        }
        public class SecondDerivedTest : DerivedTest
        {
        }
        """;
    var expected = DiagnosticResult.EmptyDiagnosticResults;
    await CSharpAnalyzerVerifier<ReflectionAnalyzer>.VerifyAnalyzerAsync(text, expected);
  }

  [Fact]
  public async Task GenericsTest_ParameterMatchingWhereConditionDerived ()
  {
    const string text =
        """
        using System;
        using Remotion.Development.UnitTesting;

        namespace ConsoleApp1;

        public class Test<T, T2> where T : DerivedTest where T2 : class
        {
          public Test (string a, int b)
          {
          }
        
          public void TestMethod (T a)
          {
          }
        
          public static void Main (string[] args)
          {
            var test = new Test<T, T2>("asdf", 4);
            PrivateInvoke.InvokePublicMethod(test, "TestMethod", new DerivedTest());
          }
        }
        public class Test2
        {
        }
        public class DerivedTest : Test2
        {
        }
        public class SecondDerivedTest : DerivedTest
        {
        }
        """;
    var expected = DiagnosticResult.EmptyDiagnosticResults;
    await CSharpAnalyzerVerifier<ReflectionAnalyzer>.VerifyAnalyzerAsync(text, expected);
  }
}
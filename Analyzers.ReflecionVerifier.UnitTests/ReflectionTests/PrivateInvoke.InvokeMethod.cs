// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: MIT

using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace Remotion.Infrastructure.Analyzers.ReflectionVerifier.UnitTests.ReflectionTests;

public class ReflectionTestPrivateInvokeInvokeMethod
{
  [Fact]
  public async Task Public_ReflectionCallCorrect ()
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
            
            var test = new Test("asdf", 3);
            PrivateInvoke.InvokePublicMethod(test, "TestMethod", 42);
          }
        }
              
        """;
    var expected = DiagnosticResult.EmptyDiagnosticResults;
    await CSharpAnalyzerVerifier<ReflectionAnalyzer>.VerifyAnalyzerAsync(text, expected);
  }

  [Fact]
  public async Task Public_ReflectionCallWrong ()
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
            
            var test = new Test("asdf", 3);
            PrivateInvoke.InvokePublicMethod(test, "TestMethod", 42, "foo");
          }
        }
              
        """;

    var expected = CSharpAnalyzerVerifier<ReflectionAnalyzer>.Diagnostic(Rules.Rule)
        .WithLocation(21, 5)
        .WithArguments("Test");
    await CSharpAnalyzerVerifier<ReflectionAnalyzer>.VerifyAnalyzerAsync(text, expected);
  }


  [Fact]
  public async Task Private_ReflectionCallCorrect ()
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
        
          private void TestMethod (int a)
          {
          }
        
          public void Test3 ()
          {
            
            var test = new Test("asdf", 3);
            PrivateInvoke.InvokeNonPublicMethod(test, "TestMethod", 42);
          }
        }
              
        """;
    var expected = DiagnosticResult.EmptyDiagnosticResults;
    await CSharpAnalyzerVerifier<ReflectionAnalyzer>.VerifyAnalyzerAsync(text, expected);
  }

  [Fact]
  public async Task Private_ReflectionCallWrong ()
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
        
          private void TestMethod (int a)
          {
          }
        
          public void Test3 ()
          {
            
            var test = new Test("asdf", 3);
            PrivateInvoke.InvokeNonPublicMethod(test, "TestMethod", 42, 3);
          }
        }
              
        """;

    var expected = CSharpAnalyzerVerifier<ReflectionAnalyzer>.Diagnostic(Rules.Rule)
        .WithLocation(21, 5)
        .WithArguments("Test");
    await CSharpAnalyzerVerifier<ReflectionAnalyzer>.VerifyAnalyzerAsync(text, expected);
  }

  //------------------------------------------------------------------------------------
  //static
  //------------------------------------------------------------------------------------

  [Fact]
  public async Task Public_Static_ReflectionCallCorrect ()
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
        
          public static void TestMethod (int a)
          {
          }
        
          public void Test3 ()
          {
            
            PrivateInvoke.InvokePublicStaticMethod(typeof(Test), "TestMethod", 42);
          }
        }
              
        """;
    var expected = DiagnosticResult.EmptyDiagnosticResults;
    await CSharpAnalyzerVerifier<ReflectionAnalyzer>.VerifyAnalyzerAsync(text, expected);
  }

  [Fact]
  public async Task Public_Static_ReflectionCallWrong ()
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
        
          public static void TestMethod (int a)
          {
          }
        
          public void Test3 ()
          {
            
            PrivateInvoke.InvokePublicStaticMethod(typeof(Test), "TestMethod", 42, "foo");
          }
        }
              
        """;

    var expected = CSharpAnalyzerVerifier<ReflectionAnalyzer>.Diagnostic(Rules.Rule)
        .WithLocation(20, 5)
        .WithArguments("Test");
    await CSharpAnalyzerVerifier<ReflectionAnalyzer>.VerifyAnalyzerAsync(text, expected);
  }


  [Fact]
  public async Task Private_Static_ReflectionCallCorrect ()
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
        
          private static void TestMethod (int a)
          {
          }
        
          public void Test3 ()
          {
            
            PrivateInvoke.InvokeNonPublicStaticMethod(typeof(Test), "TestMethod", 42);
          }
        }
              
        """;
    var expected = DiagnosticResult.EmptyDiagnosticResults;
    await CSharpAnalyzerVerifier<ReflectionAnalyzer>.VerifyAnalyzerAsync(text, expected);
  }

  [Fact]
  public async Task Private_Static_ReflectionCallWrong ()
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
        
          private static void TestMethod (int a)
          {
          }
        
          public void Test3 ()
          {
            
            PrivateInvoke.InvokeNonPublicStaticMethod(typeof(Test), "TestMethod", 42, 3);
          }
        }
              
        """;

    var expected = CSharpAnalyzerVerifier<ReflectionAnalyzer>.Diagnostic(Rules.Rule)
        .WithLocation(20, 5)
        .WithArguments("Test");
    await CSharpAnalyzerVerifier<ReflectionAnalyzer>.VerifyAnalyzerAsync(text, expected);
  }
}
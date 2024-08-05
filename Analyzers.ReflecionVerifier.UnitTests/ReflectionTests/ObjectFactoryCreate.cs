// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: MIT
using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace Remotion.Infrastructure.Analyzers.ReflectionVerifier.UnitTests.ReflectionTests;

public class ReflectionTestObjectFactoryCreate
{
  [Fact]
  public async Task Create_SpecificEmptyParamList_ReflectionCallCorrect ()
  {
    const string text =
        """

        using System;

        using Remotion.Mixins;
        using Remotion.TypePipe;

        namespace ConsoleApp1
        {
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
              ObjectFactory.Create(typeof(Test), ParamList.Empty);
            }
          }
        }
        
        
              
        """;
    var expected = DiagnosticResult.EmptyDiagnosticResults;
    await CSharpAnalyzerVerifier<ReflectionAnalyzer>.VerifyAnalyzerAsync(text, expected);
  }

  [Fact]
  public async Task Create_SpecificEmptyParamList_ReflectionCallWrong ()
  {
    const string text =
        """

        using System;

        using Remotion.Mixins;
        using Remotion.TypePipe;

        namespace ConsoleApp1
        {
          public class Test
          {
            public Test (string a)
            {
            }
        
            public void TestMethod (string a, int b)
            {
            }
        
            public void Test2 ()
            {
              ObjectFactory.Create(typeof(Test), ParamList.Empty);
            }
          }
        }
              
        """;

    var expected = CSharpAnalyzerVerifier<ReflectionAnalyzer>.Diagnostic(Rules.Rule)
        .WithLocation(21, 7)
        .WithArguments("Test");
    await CSharpAnalyzerVerifier<ReflectionAnalyzer>.VerifyAnalyzerAsync(text, expected);
  }

  [Fact]
  public async Task Create_ImplicitEmptyParamList_ReflectionCallCorrect ()
  {
    const string text =
        """

        using System;

        using Remotion.Mixins;
        using Remotion.TypePipe;

        namespace ConsoleApp1
        {
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
              ObjectFactory.Create(typeof(Test));
            }
          }
        }
        
        
              
        """;
    var expected = DiagnosticResult.EmptyDiagnosticResults;
    await CSharpAnalyzerVerifier<ReflectionAnalyzer>.VerifyAnalyzerAsync(text, expected);
  }

  [Fact]
  public async Task Create_ImplicitEmptyParamList_ReflectionCallWrong ()
  {
    const string text =
        """

        using System;

        using Remotion.Mixins;
        using Remotion.TypePipe;

        namespace ConsoleApp1
        {
          public class Test
          {
            public Test (string a)
            {
            }
        
            public void TestMethod (string a, int b)
            {
            }
        
            public void Test2 ()
            {
              ObjectFactory.Create(typeof(Test));
            }
          }
        }
              
        """;

    var expected = CSharpAnalyzerVerifier<ReflectionAnalyzer>.Diagnostic(Rules.Rule)
        .WithLocation(21, 7)
        .WithArguments("Test");
    await CSharpAnalyzerVerifier<ReflectionAnalyzer>.VerifyAnalyzerAsync(text, expected);
  }

  [Fact]
  public async Task Create_CreateParamList_ReflectionCallCorrect ()
  {
    const string text =
        """

        using System;

        using Remotion.Mixins;
        using Remotion.TypePipe;

        namespace ConsoleApp1
        {
          public class Test
          {
            public Test (string a)
            {
            }
        
            public void TestMethod (string a, int b)
            {
            }
        
            public void Test2 ()
            {
              ObjectFactory.Create(typeof(Test), ParamList.Create("foo"));
            }
          }
        }
        
        
              
        """;
    var expected = DiagnosticResult.EmptyDiagnosticResults;
    await CSharpAnalyzerVerifier<ReflectionAnalyzer>.VerifyAnalyzerAsync(text, expected);
  }

  [Fact]
  public async Task Create_CreateParamList_ReflectionCallWrong ()
  {
    const string text =
        """

        using System;

        using Remotion.Mixins;
        using Remotion.TypePipe;

        namespace ConsoleApp1
        {
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
              ObjectFactory.Create(typeof(Test), ParamList.Create("foo"));
            }
          }
        }
              
        """;

    var expected = CSharpAnalyzerVerifier<ReflectionAnalyzer>.Diagnostic(Rules.Rule)
        .WithLocation(21, 7)
        .WithArguments("Test");
    await CSharpAnalyzerVerifier<ReflectionAnalyzer>.VerifyAnalyzerAsync(text, expected);
  }

  //-------------------------------------------------------
  //with genericArgument
  //-------------------------------------------------------

  [Fact]
  public async Task Create_SpecificEmptyParamList_WithGeneric_ReflectionCallCorrect ()
  {
    const string text =
        """

        using System;

        using Remotion.Mixins;
        using Remotion.TypePipe;

        namespace ConsoleApp1
        {
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
              ObjectFactory.Create<Test>(ParamList.Empty);
            }
          }
        }
              
        """;
    var expected = DiagnosticResult.EmptyDiagnosticResults;
    await CSharpAnalyzerVerifier<ReflectionAnalyzer>.VerifyAnalyzerAsync(text, expected);
  }

  [Fact]
  public async Task Create_SpecificEmptyParamList_WithGeneric_ReflectionCallWrong ()
  {
    const string text =
        """

        using System;

        using Remotion.Mixins;
        using Remotion.TypePipe;

        namespace ConsoleApp1
        {
          public class Test
          {
            public Test (string a)
            {
            }
        
            public void TestMethod (string a, int b)
            {
            }
        
            public void Test2 ()
            {
              ObjectFactory.Create<Test>(ParamList.Empty);
            }
          }
        }
              
        """;

    var expected = CSharpAnalyzerVerifier<ReflectionAnalyzer>.Diagnostic(Rules.Rule)
        .WithLocation(21, 7)
        .WithArguments("Test");
    await CSharpAnalyzerVerifier<ReflectionAnalyzer>.VerifyAnalyzerAsync(text, expected);
  }

  [Fact]
  public async Task Create_ImplicitEmptyParamList_WithGeneric_ReflectionCallCorrect ()
  {
    const string text =
        """

        using System;

        using Remotion.Mixins;
        using Remotion.TypePipe;

        namespace ConsoleApp1
        {
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
              ObjectFactory.Create<Test>();
            }
          }
        }
              
        """;
    var expected = DiagnosticResult.EmptyDiagnosticResults;
    await CSharpAnalyzerVerifier<ReflectionAnalyzer>.VerifyAnalyzerAsync(text, expected);
  }

  [Fact]
  public async Task Create_ImplicitEmptyParamList_WithGeneric_ReflectionCallWrong ()
  {
    const string text =
        """

        using System;

        using Remotion.Mixins;
        using Remotion.TypePipe;

        namespace ConsoleApp1
        {
          public class Test
          {
            public Test (string a)
            {
            }
        
            public void TestMethod (string a, int b)
            {
            }
        
            public void Test2 ()
            {
              ObjectFactory.Create<Test>();
            }
          }
        }
              
        """;

    var expected = CSharpAnalyzerVerifier<ReflectionAnalyzer>.Diagnostic(Rules.Rule)
        .WithLocation(21, 7)
        .WithArguments("Test");
    await CSharpAnalyzerVerifier<ReflectionAnalyzer>.VerifyAnalyzerAsync(text, expected);
  }

  [Fact]
  public async Task Create_CreateParamList_WithGeneric_ReflectionCallCorrect ()
  {
    const string text =
        """

        using System;

        using Remotion.Mixins;
        using Remotion.TypePipe;

        namespace ConsoleApp1
        {
          public class Test
          {
            public Test (string a)
            {
            }
        
            public void TestMethod (string a, int b)
            {
            }
        
            public void Test2 ()
            {
              ObjectFactory.Create<Test>(ParamList.Create("foo"));
            }
          }
        }
              
        """;
    var expected = DiagnosticResult.EmptyDiagnosticResults;
    await CSharpAnalyzerVerifier<ReflectionAnalyzer>.VerifyAnalyzerAsync(text, expected);
  }

  [Fact]
  public async Task Create_CreateParamList_WithGeneric_ReflectionCallWrong ()
  {
    const string text =
        """

        using System;

        using Remotion.Mixins;
        using Remotion.TypePipe;

        namespace ConsoleApp1
        {
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
              ObjectFactory.Create<Test>(ParamList.Create("foo"));
            }
          }
        }
              
        """;

    var expected = CSharpAnalyzerVerifier<ReflectionAnalyzer>.Diagnostic(Rules.Rule)
        .WithLocation(21, 7)
        .WithArguments("Test");
    await CSharpAnalyzerVerifier<ReflectionAnalyzer>.VerifyAnalyzerAsync(text, expected);
  }
}
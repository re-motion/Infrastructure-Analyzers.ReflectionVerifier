// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: MIT
using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace Remotion.Infrastructure.Analyzers.ReflectionVerifier.UnitTests.ReflectionTests;

public class ReflectionTestDomainObjectNewObject
{
  [Fact]
  public async Task NewObject_SpecificEmptyParamList_WithGeneric_ReflectionCallCorrect ()
  {
    const string text =
        """

        using System;
        using Remotion.Data.DomainObjects;
        using Remotion.Data.DomainObjects.DomainImplementation;
        using Remotion.Mixins;
        using Remotion.TypePipe;

        namespace ConsoleApp1
        {
          public class Test : DomainObject
          {
            public Test ()
            {
            }
        
            public void TestMethod (string a, int b)
            {
            }
        
            public void Test2 ()
            {
              DomainObject.NewObject<Test>(ParamList.Empty);
            }
          }
        }
              
        """;
    var expected = DiagnosticResult.EmptyDiagnosticResults;
    await CSharpAnalyzerVerifier<ReflectionAnalyzer>.VerifyAnalyzerAsync(text, expected);
  }

  [Fact]
  public async Task NewObject_SpecificEmptyParamList_WithGeneric_ReflectionCallWrong ()
  {
    const string text =
        """

        using System;
        using Remotion.Data.DomainObjects;
        using Remotion.Data.DomainObjects.DomainImplementation;
        using Remotion.Mixins;
        using Remotion.TypePipe;

        namespace ConsoleApp1
        {
          public class Test : DomainObject
          {
            public Test (string a)
            {
            }
        
            public void TestMethod (string a, int b)
            {
            }
        
            public void Test2 ()
            {
              DomainObject.NewObject<Test>(ParamList.Empty);
            }
          }
        }
              
        """;

    var expected = CSharpAnalyzerVerifier<ReflectionAnalyzer>.Diagnostic(Rules.Rule)
        .WithLocation(22, 7)
        .WithArguments("Test");
    await CSharpAnalyzerVerifier<ReflectionAnalyzer>.VerifyAnalyzerAsync(text, expected);
  }

  [Fact]
  public async Task NewObject_ImplicitEmptyParamList_WithGeneric_ReflectionCallCorrect ()
  {
    const string text =
        """

        using System;
        using Remotion.Data.DomainObjects;
        using Remotion.Data.DomainObjects.DomainImplementation;
        using Remotion.Mixins;
        using Remotion.TypePipe;

        namespace ConsoleApp1
        {
          public class Test : DomainObject
          {
            public Test ()
            {
            }
        
            public void TestMethod (string a, int b)
            {
            }
        
            public void Test2 ()
            {
              DomainObject.NewObject<Test>();
            }
          }
        }
              
        """;
    var expected = DiagnosticResult.EmptyDiagnosticResults;
    await CSharpAnalyzerVerifier<ReflectionAnalyzer>.VerifyAnalyzerAsync(text, expected);
  }

  [Fact]
  public async Task NewObject_ImplicitEmptyParamList_WithGeneric_ReflectionCallWrong ()
  {
    const string text =
        """

        using System;
        using Remotion.Data.DomainObjects;
        using Remotion.Data.DomainObjects.DomainImplementation;
        using Remotion.Mixins;
        using Remotion.TypePipe;

        namespace ConsoleApp1
        {
          public class Test : DomainObject
          {
            public Test (string a)
            {
            }
        
            public void TestMethod (string a, int b)
            {
            }
        
            public void Test2 ()
            {
              DomainObject.NewObject<Test>();
            }
          }
        }
              
        """;

    var expected = CSharpAnalyzerVerifier<ReflectionAnalyzer>.Diagnostic(Rules.Rule)
        .WithLocation(22, 7)
        .WithArguments("Test");
    await CSharpAnalyzerVerifier<ReflectionAnalyzer>.VerifyAnalyzerAsync(text, expected);
  }

  [Fact]
  public async Task NewObject_CreateParamList_WithGeneric_ReflectionCallCorrect ()
  {
    const string text =
        """

        using System;
        using Remotion.Data.DomainObjects;
        using Remotion.Data.DomainObjects.DomainImplementation;
        using Remotion.Mixins;
        using Remotion.TypePipe;

        namespace ConsoleApp1
        {
          public class Test : DomainObject
          {
            public Test (string a)
            {
            }
        
            public void TestMethod (string a, int b)
            {
            }
        
            public void Test2 ()
            {
              DomainObject.NewObject<Test>(ParamList.Create("foo"));
            }
          }
        }
              
        """;
    var expected = DiagnosticResult.EmptyDiagnosticResults;
    await CSharpAnalyzerVerifier<ReflectionAnalyzer>.VerifyAnalyzerAsync(text, expected);
  }

  [Fact]
  public async Task NewObject_CreateParamList_WithGeneric_ReflectionCallWrong ()
  {
    const string text =
        """

        using System;
        using Remotion.Data.DomainObjects;
        using Remotion.Data.DomainObjects.DomainImplementation;
        using Remotion.Mixins;
        using Remotion.TypePipe;

        namespace ConsoleApp1
        {
          public class Test : DomainObject
          {
            public Test (string a, int b)
            {
            }
        
            public void TestMethod (string a, int b)
            {
            }
        
            public void Test2 ()
            {
              DomainObject.NewObject<Test>(ParamList.Create("foo"));
            }
          }
        }
              
        """;

    var expected = CSharpAnalyzerVerifier<ReflectionAnalyzer>.Diagnostic(Rules.Rule)
        .WithLocation(22, 7)
        .WithArguments("Test");
    await CSharpAnalyzerVerifier<ReflectionAnalyzer>.VerifyAnalyzerAsync(text, expected);
  }
}
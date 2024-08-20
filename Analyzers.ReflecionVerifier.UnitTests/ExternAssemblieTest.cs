// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: MIT
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace Remotion.Infrastructure.Analyzers.ReflectionVerifier.UnitTests;

public class ExternAssembliesTest
{
  [Fact]
  public async Task ExternAssembliesTest_Public ()
  {
    const string text =
        """
        using System;
        using Remotion.Development.UnitTesting;
        namespace Test;

        public class Test
        {
          public static void Main (string[] args)
          {
            var sortedList = new System.Collections.Stack();
            PrivateInvoke.InvokeNonPublicMethod(sortedList, "Clone");
          }
        }
        """;
    var expected = DiagnosticResult.EmptyDiagnosticResults;
    await CSharpAnalyzerVerifier<ReflectionAnalyzer>.VerifyAnalyzerAsync(text, expected);
  }

  [Fact]
  public async Task ExternAssembliesTest_Private ()
  {
    const string text =
        """
        using System;
        using Remotion.Development.UnitTesting;
        namespace Test;

        public class Test
        {
          public static void Main (string[] args)
          {
            var sortedList = new System.Collections.SortedList();
            PrivateInvoke.InvokeNonPublicMethod(sortedList, "EnsureCapacity", 5);
          }
        }
        """;
    var expected = DiagnosticResult.EmptyDiagnosticResults;
    await CSharpAnalyzerVerifier<ReflectionAnalyzer>.VerifyAnalyzerAsync(text, expected);
  }
}
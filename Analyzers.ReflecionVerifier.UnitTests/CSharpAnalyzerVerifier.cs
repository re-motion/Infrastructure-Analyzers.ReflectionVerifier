// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: MIT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.TypePipe;

namespace Remotion.Infrastructure.Analyzers.ReflectionVerifier.UnitTests;

public static class CSharpAnalyzerVerifier<TAnalyzer>
    where TAnalyzer : DiagnosticAnalyzer, new()
{
  public static DiagnosticResult Diagnostic (DiagnosticDescriptor desc)
  {
    return CSharpAnalyzerVerifier<TAnalyzer, DefaultVerifier>.Diagnostic(desc);
  }

  public static Task VerifyAnalyzerAsync (string source, params DiagnosticResult[] expected)
  {
    var contextAssemblyLocation = typeof(PrivateInvoke).Assembly.Location;
    var contextAssemblyLocation2 = typeof(ObjectFactory).Assembly.Location;
    var contextAssemblyLocation3 = typeof(ParamList).Assembly.Location;

    var test = new Test
               {
                   TestCode = source,
                   ReferenceAssemblies = GetReferenceAssemblies(typeof(ParamList).Assembly).ToArray()[0],
                   SolutionTransforms =
                   {
                       (solution, id) =>
                       {
                         var project = solution.GetProject(id)!;
                         project = project.AddMetadataReferences(
                         [
                             MetadataReference.CreateFromFile(contextAssemblyLocation),
                             MetadataReference.CreateFromFile(contextAssemblyLocation2),
                             MetadataReference.CreateFromFile(contextAssemblyLocation3)
                         ]);
                         return project.Solution;
                       }
                   }
               };
    test.ExpectedDiagnostics.AddRange(expected);

    return test.RunAsync();
  }

  private static IEnumerable<ReferenceAssemblies> GetReferenceAssemblies (params Assembly[] assemblies)
  {
    foreach (var assembly in assemblies)
    {
      yield return assembly.GetCustomAttribute<TargetFrameworkAttribute>()!.FrameworkName switch
      {
          ".NETCoreApp,Version=v8.0" => ReferenceAssemblies.Net.Net80,
          ".NETStandard,Version=v2.0" => ReferenceAssemblies.NetStandard.NetStandard20,
          ".NETFramework,Version=v4.8" => ReferenceAssemblies.NetFramework.Net48.Default,
          var frameworkName => throw new NotSupportedException($"'{frameworkName}' is not supported.")
      };
    }
  }

  private class Test : CSharpAnalyzerTest<TAnalyzer, DefaultVerifier>;
}
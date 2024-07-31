// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: MIT

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Remotion.Infrastructure.Analyzers.ReflectionVerifier;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ReflectionAnalyzer : DiagnosticAnalyzer
{
  //list of Rules
  public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
    [Rules.Rule];

  public override void Initialize (AnalysisContext context)
  {
    context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.CompilationUnit);

    context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

    context.EnableConcurrentExecution();
  }

  private static void AnalyzeNode (SyntaxNodeAnalysisContext context)
  {
  }
}
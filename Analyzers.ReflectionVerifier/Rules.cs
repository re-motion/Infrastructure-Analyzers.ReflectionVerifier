// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: MIT
using Microsoft.CodeAnalysis;

namespace Remotion.Infrastructure.Analyzers.ReflectionVerifier;

public class Rules
{
  private const string c_diagnosticId = "RMBCA0001";
  private const string c_category = "Usage";
  private static readonly LocalizableString s_title = "DummyTitle";
  private static readonly LocalizableString s_messageFormat = "DummyMessage";
  private static readonly LocalizableString s_description = "DummyDescription.";
  public static readonly DiagnosticDescriptor Rule = new(c_diagnosticId, s_title, s_messageFormat, c_category,
      DiagnosticSeverity.Warning, true, s_description);
}
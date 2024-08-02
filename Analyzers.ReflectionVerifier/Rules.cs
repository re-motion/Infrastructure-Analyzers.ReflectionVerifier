// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: MIT
using Microsoft.CodeAnalysis;

namespace Remotion.Infrastructure.Analyzers.ReflectionVerifier;

public static class Rules
{
  private const string c_diagnosticId = "RMRVA0001";
  private const string c_category = "Usage";
  private const DiagnosticSeverity c_severity = DiagnosticSeverity.Warning;
  private static readonly LocalizableString s_title = "Parameters wrong";
  private static readonly LocalizableString s_messageFormat = "Parameters wrong";
  private static readonly LocalizableString s_description = "Parameter count or types do not match the Parameters of the called method.";

  public static readonly DiagnosticDescriptor Rule = new(
      c_diagnosticId,
      s_title,
      s_messageFormat,
      c_category,
      c_severity,
      true,
      s_description);

  private const string c_diagnosticIdError = "RMRVA0000";
  private static readonly LocalizableString s_titleError = "Error";
  private static readonly LocalizableString s_messageError = "Error: {0}";
  private static readonly LocalizableString s_descriptionError = "Error.";

  public static readonly DiagnosticDescriptor Error = new(
      c_diagnosticIdError,
      s_titleError,
      s_messageError,
      c_category,
      c_severity,
      true,
      s_descriptionError);
}
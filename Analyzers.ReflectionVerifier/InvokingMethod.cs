// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: MIT
namespace Remotion.Infrastructure.Analyzers.ReflectionVerifier;

public enum InvokingMethod
{
  CreateInstance,
  InvokeMethod,
  CreateWithGeneric,
  CreateWithoutGeneric,
  NewObjectWithGeneric,
  NewObjectWithOutGeneric,
  MockGeneric,
  MockSetup
}
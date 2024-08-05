// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: MIT
namespace Remotion.Infrastructure.Analyzers.ReflectionVerifier;

public enum InvokingMethod
{
  NotAReflection,
  CreateInstance,
  InvokeMethod,
  CreateWithGeneric,
  CreateWithoutGeneric,
  LifetimeServiceNewObjectWithOutGeneric,
  DomainObjectNewObjectWithGeneric,
  MockGeneric,
  MockSetup
}
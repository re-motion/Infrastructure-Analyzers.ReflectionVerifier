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

  //TODO: implement for MethodInfo.Invoke()
  //TODO: implement for properties and fields
  //TODO: implement check if the called method is public/private when calling PrivateInvoke or overload of ObjectFactory.Create<T>
}
// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: MIT
using System.Collections.Generic;

namespace Remotion.Infrastructure.Analyzers.ReflectionVerifier;

public partial class SignatureFinder
{
  private static readonly Dictionary<string, InvokingMethod> s_methodNameToKind =
      new()
      {
          {
              "System.Activator.CreateInstance",
              InvokingMethod.CreateInstance
          },
          {
              "Remotion.Development.UnitTesting.PrivateInvoke.InvokePublicMethod",
              InvokingMethod.InvokeMethod
          },
          {
              "Remotion.Development.UnitTesting.PrivateInvoke.InvokeNonPublicMethod",
              InvokingMethod.InvokeMethod
          },
          {
              "Remotion.Development.UnitTesting.PrivateInvoke.InvokePublicStaticMethod",
              InvokingMethod.InvokeMethod
          },
          {
              "Remotion.Development.UnitTesting.PrivateInvoke.InvokeNonPublicStaticMethod",
              InvokingMethod.InvokeMethod
          },
          {
              "Remotion.Mixins.ObjectFactory.Create",
              InvokingMethod.CreateWithoutGeneric
          },
          {
              "Remotion.Mixins.ObjectFactory.Create<>",
              InvokingMethod.CreateWithGeneric
          },
          {
              "Remotion.Data.DomainObjects.DomainImplementation.LifetimeService.NewObject",
              InvokingMethod.LifetimeServiceNewObjectWithOutGeneric
          },
          {
              "Remotion.Data.DomainObjects.DomainObject.NewObject<>",
              InvokingMethod.DomainObjectNewObjectWithGeneric
          },
          {
              "Moq.Mock.Mock",
              InvokingMethod.MockGeneric
          },
          {
              "Moq.Protected.Setup",
              InvokingMethod.MockSetup
          }
      };
}
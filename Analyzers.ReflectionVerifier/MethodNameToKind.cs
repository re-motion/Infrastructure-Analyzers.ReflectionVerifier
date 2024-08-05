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
              "System.Activator.CreateInstance(System.Type, params object?[]?)",
              InvokingMethod.CreateInstance
          },
          {
              "Remotion.Development.UnitTesting.PrivateInvoke.InvokePublicMethod(object, string, params object?[]?)",
              InvokingMethod.InvokeMethod
          },
          {
              "Remotion.Development.UnitTesting.PrivateInvoke.InvokeNonPublicMethod(object, string, params object?[]?)",
              InvokingMethod.InvokeMethod
          },
          {
              "Remotion.Development.UnitTesting.PrivateInvoke.InvokePublicStaticMethod(object, string, params object?[]?)",
              InvokingMethod.InvokeMethod
          },
          {
              "Remotion.Development.UnitTesting.PrivateInvoke.InvokeNonPublicStaticMethod(object, string, params object?[]?)",
              InvokingMethod.InvokeMethod
          },
          {
              "Remotion.Mixins.ObjectFactory.Create(System.Type, Remotion.TypePipe.ParamList, params object[])",
              InvokingMethod.CreateWithoutGeneric
          },
          {
              "Remotion.Mixins.ObjectFactory.Create(System.Type)",
              InvokingMethod.CreateWithoutGeneric
          },
          {
              "Remotion.Mixins.ObjectFactory.Create<T>()",
              InvokingMethod.CreateWithGeneric
          },
          {
              "Remotion.Mixins.ObjectFactory.Create<T>(Remotion.TypePipe.ParamList, params object[])",
              InvokingMethod.CreateWithGeneric
          },
          {
              "Remotion.Data.DomainObjects.DomainImplementation.LifetimeService.NewObject(Remotion.Data.DomainObjects.ClientTransaction, System.Type, Remotion.TypePipe.ParamList)",
              InvokingMethod.LifetimeServiceNewObjectWithOutGeneric
          },
          {
              "Remotion.Data.DomainObjects.DomainObject.NewObject<T>(Remotion.TypePipe.ParamList)",
              InvokingMethod.DomainObjectNewObjectWithGeneric
          },
          {
              "Remotion.Data.DomainObjects.DomainObject.NewObject<T>()",
              InvokingMethod.DomainObjectNewObjectWithGeneric
          },
          {
              "Moq.Mock<T>.Mock(params object[])",
              InvokingMethod.MockGeneric
          },
          {
              "Moq.Protected.Setup(string, params object[])",
              InvokingMethod.MockSetup
          }
      };
}
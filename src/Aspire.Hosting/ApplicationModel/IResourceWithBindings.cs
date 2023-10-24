// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Aspire.Hosting.ApplicationModel;

public interface IResourceWithBindings : IResource
{
    public EndpointReference GetEndpoint(string bindingName)
    {
        return new EndpointReference(this, bindingName);
    }
}
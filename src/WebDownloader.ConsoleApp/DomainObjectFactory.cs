// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection;

namespace WebDownloader.ConsoleApp;

internal class DomainObjectFactory(IServiceProvider serviceProvider) : IDomainObjectFactory
{
    public TDomain CreateInstance<TDomain>(params object[] parameters) => ActivatorUtilities.CreateInstance<TDomain>(serviceProvider, parameters);
}

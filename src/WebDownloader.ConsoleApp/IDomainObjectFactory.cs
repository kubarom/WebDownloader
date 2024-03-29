﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace WebDownloader.ConsoleApp;

public interface IDomainObjectFactory
{
    TDomain CreateInstance<TDomain>(params object[] parameters);
}

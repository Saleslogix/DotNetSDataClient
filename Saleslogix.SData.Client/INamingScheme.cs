﻿// Copyright (c) 1997-2014, SalesLogix NA, LLC. All rights reserved.

using System.Reflection;

namespace Saleslogix.SData.Client
{
    public interface INamingScheme
    {
        string GetName(MemberInfo member, bool includeProtocolProps = true);
        string GetName(ParameterInfo param);
    }
}
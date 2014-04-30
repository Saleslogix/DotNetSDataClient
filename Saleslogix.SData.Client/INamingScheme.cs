// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

using System.Reflection;

namespace Saleslogix.SData.Client
{
    public interface INamingScheme
    {
        string GetName(MemberInfo member);
        string GetName(ParameterInfo param);
    }
}
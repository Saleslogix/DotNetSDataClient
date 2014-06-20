﻿// Copyright (c) 1997-2014, SalesLogix NA, LLC. All rights reserved.

namespace Saleslogix.SData.Client
{
    public interface IChangeTracking : System.ComponentModel.IChangeTracking
    {
        object GetChanges();
    }
}
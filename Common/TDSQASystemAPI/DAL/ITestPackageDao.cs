﻿using System.Collections.Generic;

namespace TDSQASystemAPI.DAL
{
    /// <summary>
    /// An interface for persisting objects to the <code>OSS_Configs</code> and/or the <code>OSS_ItemBank</code> database.
    /// </summary>
    /// <typeparam name="T">The type of object being saved.</typeparam>
    public interface ITestPackageDao<T>
    {
        void Insert(IList<T> recordsToSave);
    }
}
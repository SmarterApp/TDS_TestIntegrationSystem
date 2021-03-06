﻿using System.Collections.Generic;

namespace TDSQASystemAPI.DAL
{
    /// <summary>
    /// An interface for persisting objects to the <code>OSS_Configs</code> and/or the <code>OSS_ItemBank</code> database.
    /// </summary>
    /// <typeparam name="T">The type of object being saved.</typeparam>
    public interface ITestPackageDao<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="recordsToSave"></param>
        void Insert(IList<T> recordsToSave);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="recordToSave"></param>
        void Insert(T recordToSave);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="recordToSave"></param>
        bool Exists(T recordToCheck);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        List<T> Find(object criteria);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="recordToUpdate"></param>
        void Update(T recordToUpdate);
		
		List<T> FindByExample(T criteria);
    }
}

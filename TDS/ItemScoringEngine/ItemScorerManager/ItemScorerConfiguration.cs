/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace TDS.ItemScoringEngine
{
    /// <summary>
    /// This is used to load item scorer xml config into manager.
    /// </summary>
    public class ItemScorerConfiguration
    {
        private readonly IItemScorerManager _itemScorerManager;

        public ItemScorerConfiguration(IItemScorerManager itemScorerManager)
        {
            _itemScorerManager = itemScorerManager;
        }

        public bool Load(string filePath)
        {
            // check if file exists
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(String.Format("Could not find the item scorer xml config: {0}", filePath));
            }

            XDocument xmlDoc = XDocument.Load(filePath);

            if (xmlDoc.Root != null)
            {
                XElement itemScorersEl = xmlDoc.Root.Element("itemScorers");
                if (itemScorersEl != null)
                {
                    itemScorersEl.Elements("itemScorer").ForEach(LoadItemScorer);
                    return true;
                }
            }

            return false;
        }

        private Type LookupType(string typeName)
        {
            return Type.GetType(typeName);
        }

        private IItemScorer CreateInstance(Type type)
        {
            return Activator.CreateInstance(type) as IItemScorer;
        }

        private void LoadItemScorer(XElement element)
        {
            string format = element.Attribute("format").Value;
            string typeName = element.Attribute("type").Value;
            Type itemScorerType = LookupType(typeName);

            if (itemScorerType == null)
            {
                throw new TypeLoadException(String.Format("Could not find the item scorer type: {0}", typeName));
            }

            IItemScorer itemScorer = CreateInstance(itemScorerType);
            _itemScorerManager.RegisterItemScorer(format, itemScorer);
        }

    }
}

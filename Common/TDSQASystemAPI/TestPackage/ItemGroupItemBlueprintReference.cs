namespace TDSQASystemAPI.TestPackage
{
    /// <summary>
    /// A partial class to augment the <code>ItemGroupItemBlueprintReference</code> with additional 
    /// methods.
    /// </summary>
    public partial class ItemGroupItemBlueprintReference
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string[] GetContentLevels()
        {
            string[] splitContentLevelIds = idRef.Split('|');
            string[] contentLevels = new string[splitContentLevelIds.Length];

            for (var i = 0; i < splitContentLevelIds.Length; i++)
            {
                var contentLevel = i == 0
                    ? splitContentLevelIds[i]
                    : string.Format("{0}|{1}", contentLevels[i - 1], splitContentLevelIds[i]);

                contentLevels[i] = contentLevel;
            }

            return contentLevels;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace TDSQASystemAPI.TestPackage.utils
{
    public class ToolFactory
    {
        /// <summary>
        /// Define a map of the "universal" tools with their default configuration settings.
        /// </summary>
        private static readonly IDictionary<string, DefaultToolConfiguration> defaultToolMap = new Dictionary<string, DefaultToolConfiguration>
        {
            { "American Sign Language", new DefaultToolConfiguration { Name = "American Sign Language", StudentPackageFieldName = "TDS_Acc-ASL" } },
            { "Audio Playback Controls", new DefaultToolConfiguration { Name ="Audio Playback Controls", StudentPackageFieldName = "TDSAcc-AudioPlaybackControls", Visible = false } },
            { "Braille Transcript", new DefaultToolConfiguration { Name ="Braille Transcript", StudentPackageFieldName = "TDSAcc-BrailleTranscript", Visible = false } },
            { "Braille Type", new DefaultToolConfiguration { Name = "Braille Type", StudentPackageFieldName = "TDSAcc-BrailleType", DependsOnToolType = "Lanuage" } },
            { "Calculator", new DefaultToolConfiguration { Name ="Calculator", StudentPackageFieldName = "TDSAcc-Calculator", DependsOnToolType = "Language" } },
            { "Closed Captioning", new DefaultToolConfiguration { Name = "Closed Captioning", StudentPackageFieldName = "TDSACC-NFCLOSEDCAP", DependsOnToolType = "Language" } },
            { "Color Choices", new DefaultToolConfiguration { Name = "Color Choices", StudentPackageFieldName = "TDSAcc-ColorChoices", DependsOnToolType = "Language" } },
            { "Dictionary", new DefaultToolConfiguration { Name = "Dictionary", StudentPackageFieldName = "TDSAcc-Dictionary", DependsOnToolType = "Language" } },
            { "Emboss", new DefaultToolConfiguration { Name = "Emboss", StudentPackageFieldName = "TDSAcc-Emboss", DependsOnToolType = "Braille Type" } },
            { "Emboss Request Type", new DefaultToolConfiguration { Name = "Emboss Request Type", StudentPackageFieldName = "TDSAcc-EmbossRequestType", DependsOnToolType = "Braille Type"} },
            { "Expandable Passages", new DefaultToolConfiguration { Name = "Expandable Passages", StudentPackageFieldName = "TDSAcc-ExpandablePassages", DependsOnToolType = "Language" } },
            { "Font Type", new DefaultToolConfiguration { Name = "Font Type", StudentPackageFieldName = "TDSAcc-FontType", Visible = false } },
            { "Global Notes", new DefaultToolConfiguration { Name = "Global Notes", StudentPackageFieldName = "TDSAcc-GlobalNotes" } },
            { "Hardware Checks", new DefaultToolConfiguration { Name = "Hardware Checks", StudentPackageFieldName = "TDSAcc-HWCheck", Visible = false} },
            { "Highlight", new DefaultToolConfiguration { Name = "Highlight", StudentPackageFieldName = "TDSAcc-Highlight" } },
            { "Item Font Size", new DefaultToolConfiguration { Name = "Item Font Size", StudentPackageFieldName = "TDSAcc-ItemFontSize", Visible =false } },
            { "Item Tools Menu", new DefaultToolConfiguration { Name = "Item Tools Menu", StudentPackageFieldName = "TDSAcc-ITM", Visible = false } },
            { "Language", new DefaultToolConfiguration { Name = "Language", StudentPackageFieldName = "TDSAcc-Language", AllowChange = false, Required = true } },
            { "Mark for Review", new DefaultToolConfiguration { Name = "Mark for Review" ,StudentPackageFieldName = "TDSAcc-MarkForReview", DependsOnToolType = "Language" } },
            { "Masking", new DefaultToolConfiguration { Name = "Masking", StudentPackageFieldName = "TDSAcc-Masking", DependsOnToolType = "Language" } },
            { "Mute System Volume", new DefaultToolConfiguration { Name = "Mute System Volume", StudentPackageFieldName = "TDSAcc-Mute", DependsOnToolType = "Language", Visible = false } },
            { "Non-Embedded Accommodations", new DefaultToolConfiguration { Name = "Non-Embedded Accommodations", StudentPackageFieldName = "TDSAcc-NonEmbedAcc", AllowMultiple = true, Functional = false } },
            { "Non-Embedded Designated Supports", new DefaultToolConfiguration { Name = "Non-Embedded Designated Supports", StudentPackageFieldName = "TDSAcc-DesigSup", Functional = false, AllowMultiple = true } },
            { "Passage Font Size", new DefaultToolConfiguration { Name = "Passage Font Size", StudentPackageFieldName = "TDSAcc-FontSize", Visible = false } },
            { "Print on Request", new DefaultToolConfiguration { Name = "Print on Request", StudentPackageFieldName = "TDSAcc-PrintOnRequest", DependsOnToolType = "Language" } },
            { "Print Size", new DefaultToolConfiguration { Name = "Print Size", StudentPackageFieldName = "TDSAcc-PrintSize", Required = true } },
            { "Review Screen Layout", new DefaultToolConfiguration { Name = "Review Screen Layout", StudentPackageFieldName = "TDSAcc-RvScrn", Visible = false } },
            { "Streamlined Mode", new DefaultToolConfiguration { Name = "Streamlined Mode", StudentPackageFieldName = "TDSAcc-EAM", DependsOnToolType = "Language" } },
            { "Strikethrough", new DefaultToolConfiguration { Name = "Strikethrough", StudentPackageFieldName = "TDSAcc-Strikethrough", DependsOnToolType = "Lanaguage" } },
            { "Student Comments", new DefaultToolConfiguration { Name = "Student Comments", StudentPackageFieldName = "TDSAcc-StudentComments" } },
            { "System Volume Control", new DefaultToolConfiguration { Name = "System Volume Control", StudentPackageFieldName = "TDSAcc-SVC", Visible = false } },
            { "Test Progress Indicator", new DefaultToolConfiguration { Name = "Test Progress Indicator", StudentPackageFieldName = "TDSAcc-TPI", Visible = false } },
            { "Test Shell", new DefaultToolConfiguration { Name = "Test Shell", StudentPackageFieldName = "TDSAcc-TestShell", Visible = false } },
            { "Thesaurus", new DefaultToolConfiguration { Name = "Thesaurus", StudentPackageFieldName = "TDSAcc-Thesaurus", DependsOnToolType = "Language" } },
            { "TTS", new DefaultToolConfiguration { Name = "TTS", StudentPackageFieldName = "TDSAcc-TTS", DependsOnToolType = "Language" } },
            { "TTS Audio Adjustments", new DefaultToolConfiguration { Name = "TTS Audio Adjustments", StudentPackageFieldName = "TDSAcc-TTSAdjust", Visible = false, DependsOnToolType = "Language" } },
            { "TTS Pausing", new DefaultToolConfiguration { Name = "TTS Pausing", StudentPackageFieldName = "TDSAcc-TTSPausing", Visible = false, DependsOnToolType = "Language" } },
            { "TTX Business Rules", new DefaultToolConfiguration { Name = "TTX Business Rules", StudentPackageFieldName = "TDSAcc-TTXBusinessRules", Visible = false, DependsOnToolType = "Language" } },
            { "Tutorial", new DefaultToolConfiguration { Name = "Tutorial", StudentPackageFieldName = "TDSAcc-Tutorial", Visible = false, DependsOnToolType = "Language" } },
            { "Word List", new DefaultToolConfiguration { Name = "Word List", StudentPackageFieldName = "TDSAcc-WordList", DependsOnToolType = "Language" } }
        };

        /// <summary>
        /// Build a new <code>ToolsTool</code> with the desired settings.
        /// <remarks>
        /// The desired settings have a hierarchy:
        /// 
        /// 1.  Choose whatever setting the user specified in the Test Package XML
        /// 2.  If no setting was specified in the Test Package XML, then look up the value in the collection of
        /// default tools
        /// 3.  If a default tool does not exist (or does not have a default value for the setting in question), 
        /// Fall back to a "universal default", which is usually the value specified as the default in the XSD.
        /// </remarks>
        /// </summary>
        /// <param name="tool">The <code>ToolsTool</code> from the test package XML.</param>
        /// <returns>A new <code>ToolsTool</code> instance with the </returns>
        public static ToolsTool GetInstance(XElement toolElement)
        {
            defaultToolMap.TryGetValue((toolElement.Attribute("name")?.Value ?? string.Empty), out DefaultToolConfiguration defaultTool);

            string name = toolElement.Attribute("name")?.Value
                ?? throw new InvalidOperationException("A tool must have a name attribute and one was not supplied");
            string studentPackageFieldName = toolElement.Attribute("studentPackageFieldName")?.Value
                ?? defaultTool?.StudentPackageFieldName 
                ?? throw new InvalidOperationException("A tool must have a Student Package Field Name attribute and 'studentPackageField' was not supplied");
            string type = toolElement.Attribute("type")?.Value ?? (defaultTool?.Type);

            int sortOrder = TryGetIntAttributeValue(toolElement, "sortOrder") ?? defaultTool?.SortOrder ?? 0;

            bool allowChange = TryGetBooleanAttributeValue(toolElement, "allowChange") ?? defaultTool?.AllowChange ?? true;
            bool allowMultiple = TryGetBooleanAttributeValue(toolElement, "allowMultiple") ?? defaultTool?.AllowMultiple ?? false;
            bool disableOnGuest = TryGetBooleanAttributeValue(toolElement, "disableOnGuest") ?? defaultTool?.DisableOnGuest ?? false;
            bool isRequired = TryGetBooleanAttributeValue(toolElement, "required") ?? defaultTool?.Required ?? false;
            bool isVisible = TryGetBooleanAttributeValue(toolElement, "visible") ?? defaultTool?.Visible ?? true;
            bool isStudentControl = TryGetBooleanAttributeValue(toolElement, "studentControl") ?? defaultTool?.StudentControl ?? false;
            bool isSelectable = TryGetBooleanAttributeValue(toolElement, "selectable") ?? defaultTool?.Selectable ?? true;
            bool isFunctional = TryGetBooleanAttributeValue(toolElement, "functional") ?? defaultTool?.Functional ?? false;

            ToolsToolOption[] options = GetOptions(toolElement);

            return new ToolsTool
            {
                allowChange = allowChange,
                allowMultiple = allowMultiple,
                DependsOnToolType = defaultTool?.DependsOnToolType,
                disableOnGuest = disableOnGuest,
                Functional = isFunctional,
                name = name,
                Options = options,
                required = isRequired,
                sortOrder = sortOrder,
                studentPackageFieldName = studentPackageFieldName,
                type = type,
                Visible = isVisible,
                StudentControl = isStudentControl,
                Selectable = isSelectable
            };
        }

        /// <summary>
        /// Try to get a <code>bool</code> from an XML attribute.
        /// </summary>
        /// <param name="toolElement">The <code>Xelement</code> representing the <code>ToolsTool</code> to inspect.</param>
        /// <param name="attributeName">The name of the attribute for the value we need.</param>
        /// <returns>A <code>bool</code> if the attribute exists; otherwise <code>null</code>.</returns>
        private static bool? TryGetBooleanAttributeValue(XElement toolElement, string attributeName)
        {
            bool? value = null;

            if (toolElement.Attribute(attributeName) != null)
            {
                value = toolElement.Attribute(attributeName).Value == "1"; // "1" is true, "0" is false
            }

            return value;
        }

        /// <summary>
        /// Try to get a <code>int</code> from an XML attribute.
        /// </summary>
        /// <param name="toolElement">The <code>Xelement</code> representing the <code>ToolsTool</code> to inspect.</param>
        /// <param name="attributeName">The name of the attribute for the value we need.</param>
        /// <returns>A <code>int</code> if the attribute exists; otherwise <code>null</code>.</returns>
        private static int? TryGetIntAttributeValue(XElement toolElement, string attributeName)
        {
            int? value = null;

            if (toolElement.Attribute(attributeName) != null)
            {
                value = int.Parse(toolElement.Attribute(attributeName).Value);
            }

            return value;
        }

        /// <summary>
        /// Deserialize the <code>ToolsTool</code>'s options into an array of <code>ToolsToolOption</code>s
        /// </summary>
        /// <param name="toolElement">The <code>XElement</code> representing the <code>ToolsTool</code>.</param>
        /// <returns>An array of <code>ToolsToolOption</code>s.</returns>
        private static ToolsToolOption[] GetOptions(XElement toolElement)
        {
            var options = toolElement.Elements("Options");

            // Tell the serializer what the root is to prevent the "xmlns = '' was not expected" error
            var optionRoot = new XmlRootAttribute
            {
                ElementName = "Option",
                IsNullable = true
            };

            var serializer = new XmlSerializer(typeof(ToolsToolOption), optionRoot);

            return options.Descendants("Option")
                .Select(o => serializer.Deserialize(o.CreateReader()) as ToolsToolOption)
                .ToArray();
        }
    }
}

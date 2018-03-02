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
        private static readonly IDictionary<string, ToolsTool> defaultToolMap = new Dictionary<string, ToolsTool>
        {
            { "American Sign Language", new ToolsTool { name = "American Sign Language", studentPackageFieldName = "TDS_Acc-ASL" } },
            { "Audio Playback Controls", new ToolsTool { name ="Audio Playback Controls", studentPackageFieldName = "TDSAcc-AudioPlaybackControls", Visible = false } },
            { "Braille Transcript", new ToolsTool { name ="Braille Transcript", studentPackageFieldName = "TDSAcc-BrailleTranscript", Visible = false } },
            { "Braille Type", new ToolsTool { name = "Braille Type", studentPackageFieldName = "TDSAcc-BrailleType", DependsOnToolType = "Lanuage" } },
            { "Calculator", new ToolsTool { name ="Calculator", studentPackageFieldName = "TDSAcc-Calculator", DependsOnToolType = "Language" } },
            { "Closed Captioning", new ToolsTool { name = "Closed Captioning", studentPackageFieldName = "TDSACC-NFCLOSEDCAP", DependsOnToolType = "Language" } },
            { "Color Choices", new ToolsTool { name = "Color Choices", studentPackageFieldName = "TDSAcc-ColorChoices", DependsOnToolType = "Language" } },
            { "Dictionary", new ToolsTool { name = "Dictionary", studentPackageFieldName = "TDSAcc-Dictionary", DependsOnToolType = "Language" } },
            { "Emboss", new ToolsTool { name = "Emboss", studentPackageFieldName = "TDSAcc-Emboss", DependsOnToolType = "Braille Type" } },
            { "Emboss Request Type", new ToolsTool { name = "Emboss Request Type", studentPackageFieldName = "TDSAcc-EmbossRequestType", DependsOnToolType = "Braille Type"} },
            { "Expandable Passages", new ToolsTool { name = "Expandable Passages", studentPackageFieldName = "TDSAcc-ExpandablePassages", DependsOnToolType = "Language" } },
            { "Font Type", new ToolsTool { name = "Font Type", studentPackageFieldName = "TDSAcc-FontType", Visible = false } },
            { "Global Notes", new ToolsTool { name = "Global Notes", studentPackageFieldName = "TDSAcc-GlobalNotes" } },
            { "Hardware Checks", new ToolsTool { name = "Hardware Checks", studentPackageFieldName = "TDSAcc-HWCheck", Visible = false} },
            { "Highlight", new ToolsTool { name = "Highlight", studentPackageFieldName = "TDSAcc-Highlight" } },
            { "Item Font Size", new ToolsTool { name = "Item Font Size", studentPackageFieldName = "TDSAcc-ItemFontSize", Visible =false } },
            { "Item Tools Menu", new ToolsTool { name = "Item Tools Menu", studentPackageFieldName = "TDSAcc-ITM", Visible = false } },
            { "Language", new ToolsTool { name = "Language", studentPackageFieldName = "TDSAcc-Language", allowChange = false, required = true } },
            { "Mark for Review", new ToolsTool { name = "Mark for Review" ,studentPackageFieldName = "TDSAcc-MarkForReview", DependsOnToolType = "Language" } },
            { "Masking", new ToolsTool { name = "Masking", studentPackageFieldName = "TDSAcc-Masking", DependsOnToolType = "Language" } },
            { "Mute System Volume", new ToolsTool { name = "Mute System Volume", studentPackageFieldName = "TDSAcc-Mute", DependsOnToolType = "Language", Visible = false } },
            { "Non-Embedded Accommodations", new ToolsTool { name = "Non-Embedded Accommodations", studentPackageFieldName = "TDSAcc-NonEmbedAcc", allowMultiple = true, Functional = false } },
            { "Non-Embedded Designated Supports", new ToolsTool { name = "Non-Embedded Designated Supports", studentPackageFieldName = "TDSAcc-DesigSup", Functional = false, allowMultiple = true } },
            { "Passage Font Size", new ToolsTool { name = "Passage Font Size", studentPackageFieldName = "TDSAcc-FontSize", Visible = false } },
            { "Print on Request", new ToolsTool { name = "Print on Request", studentPackageFieldName = "TDSAcc-PrintOnRequest", DependsOnToolType = "Language" } },
            { "Print Size", new ToolsTool { name = "Print Size", studentPackageFieldName = "TDSAcc-PrintSize", required = true } },
            { "Review Screen Layout", new ToolsTool { name = "Review Screen Layout", studentPackageFieldName = "TDSAcc-RvScrn", Visible = false } },
            { "Streamlined Mode", new ToolsTool { name = "Streamlined Mode", studentPackageFieldName = "TDSAcc-EAM", DependsOnToolType = "Language" } },
            { "Strikethrough", new ToolsTool { name = "Strikethrough", studentPackageFieldName = "TDSAcc-Strikethrough", DependsOnToolType = "Lanaguage" } },
            { "Student Comments", new ToolsTool { name = "Student Comments", studentPackageFieldName = "TDSAcc-StudentComments" } },
            { "System Volume Control", new ToolsTool { name = "System Volume Control", studentPackageFieldName = "TDSAcc-SVC", Visible = false } },
            { "Test Progress Indicator", new ToolsTool { name = "Test Progress Indicator", studentPackageFieldName = "TDSAcc-TPI", Visible = false } },
            { "Test Shell", new ToolsTool { name = "Test Shell", studentPackageFieldName = "TDSAcc-TestShell", Visible = false } },
            { "Thesaurus", new ToolsTool { name = "Thesaurus", studentPackageFieldName = "TDSAcc-Thesaurus", DependsOnToolType = "Language" } },
            { "TTS", new ToolsTool { name = "TTS", studentPackageFieldName = "TDSAcc-TTS", DependsOnToolType = "Language" } },
            { "TTS Audio Adjustments", new ToolsTool { name = "TTS Audio Adjustments", studentPackageFieldName = "TDSAcc-TTSAdjust", Visible = false, DependsOnToolType = "Language" } },
            { "TTS Pausing", new ToolsTool { name = "TTS Pausing", studentPackageFieldName = "TDSAcc-TTSPausing", Visible = false, DependsOnToolType = "Language" } },
            { "TTX Business Rules", new ToolsTool { name = "TTX Business Rules", studentPackageFieldName = "TDSAcc-TTXBusinessRules", Visible = false, DependsOnToolType = "Language" } },
            { "Tutorial", new ToolsTool { name = "Tutorial", studentPackageFieldName = "TDSAcc-Tutorial", Visible = false, DependsOnToolType = "Language" } },
            { "Word List", new ToolsTool { name = "Word List", studentPackageFieldName = "TDSAcc-WordList", DependsOnToolType = "Language" } }
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
            defaultToolMap.TryGetValue(toolElement.Attribute("name").Value, out ToolsTool defaultTool);

            string name = toolElement.Attribute("name").Value
                ?? throw new InvalidOperationException("A tool must have a name and one was not supplied");
            string studentPackageFieldName = toolElement.Attribute("studentPackageFieldName")?.Value
                ?? (defaultTool == null
                    ? throw new InvalidOperationException("A tool must have a Student Package Field Name")
                    : defaultTool.studentPackageFieldName);
            string type = toolElement.Attribute("type")?.Value 
                ?? (defaultTool?.type);

            bool allowChange = TryGetBooleanAttributeValue(toolElement, "allowChange")
                ?? (defaultTool == null ? true : defaultTool.allowChange);
            bool allowMultiple = TryGetBooleanAttributeValue(toolElement, "allowMultiple")
                ?? (defaultTool == null ? false : defaultTool.allowMultiple);
            bool disableOnGuest = TryGetBooleanAttributeValue(toolElement, "disableOnGuest")
                ?? (defaultTool == null ? false : defaultTool.disableOnGuest);
            bool isRequired = TryGetBooleanAttributeValue(toolElement, "required")
                ?? (defaultTool == null ? false : defaultTool.required);
            bool isVisible = TryGetBooleanAttributeValue(toolElement, "visible")
                ?? (defaultTool == null ? true : defaultTool.Visible);
            bool isStudentControl = TryGetBooleanAttributeValue(toolElement, "studentControl")
                ?? (defaultTool == null ? false : defaultTool.StudentControl);
            bool isSelectable = TryGetBooleanAttributeValue(toolElement, "selectable")
                ?? (defaultTool == null ? true : defaultTool.Selectable);

            ToolsToolOption[] options = GetOptions(toolElement);

            return new ToolsTool
            {
                allowChange = allowChange,
                allowMultiple = allowMultiple,
                DependsOnToolType = defaultTool?.DependsOnToolType,
                disableOnGuest = disableOnGuest,
                Functional = defaultTool == null ? true : defaultTool.Functional,
                name = name,
                Options = options,
                required = isRequired,
                sortOrder = TryGetIntAttributeValue(toolElement, "sortOrder") ?? 0,
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

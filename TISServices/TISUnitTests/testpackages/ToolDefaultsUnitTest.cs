using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using TDSQASystemAPI.TestPackage;
using TDSQASystemAPI.TestPackage.utils;

namespace TISUnitTests.testpackages
{
    [TestClass]
    public class ToolDefaultsUnitTest
    {
        [TestMethod]
        public void ShouldCreatePrintRequestToolWithCorrectDefaultsWhenOnlyToolNameIsSpecified()
        {
            var printRequestXmlToolFragment = 
                @"<Tool name='Print Size' studentPackageFieldName='TDSAcc-PrintSize'></Tool>";

            var toolElement = XElement.Parse(printRequestXmlToolFragment);

            var result = ToolMapper.FromXml(toolElement);

            Assert.AreEqual("Print Size", result.name);
            Assert.AreEqual("TDSAcc-PrintSize", result.studentPackageFieldName);
            Assert.IsTrue(result.required);
        }

        [TestMethod]
        public void ShouldCreatePrintRequestWithRequiredSetToFalseDueToXmlSetting()
        {
            var printRequestXmlToolFragment =
                @"<Tool name='Print Size' studentPackageFieldName='TDSAcc-PrintSize' required='0'></Tool>";

            var toolElement = XElement.Parse(printRequestXmlToolFragment);

            var result = ToolMapper.FromXml(toolElement);

            Assert.AreEqual("Print Size", result.name);
            Assert.AreEqual("TDSAcc-PrintSize", result.studentPackageFieldName);
            Assert.IsFalse(result.required);
        }

        [TestMethod]
        public void ShouldCreateAToolWithOptionsAndDependencies()
        {
            var calculatorToolXmlFragment = @"
                <Tool name='Calculator' studentPackageFieldName='TDSAcc-Calculator' allowChange='1'>
                    <Options>
                        <Option code='TDS_Calc0' sortOrder='0'>
                            <Dependencies>
                                <Dependency ifToolType='Language' ifToolCode='ENU-Braille' enabled='false'/>
                            </Dependencies>
                        </Option>
                        <Option code='TDS_CalcSciInv&amp;TDS_CalcGraphingInv&amp;TDS_CalcRegress' sortOrder='1'>
                            <Dependencies>
                                <Dependency ifToolType='Language' ifToolCode='ENU-Braille' enabled='false'/>
                            </Dependencies>
                        </Option>
                    </Options>
                </Tool>";

            var toolElement = XElement.Parse(calculatorToolXmlFragment);

            var result = ToolMapper.FromXml(toolElement);

            // Verify the Tool settings/configuration
            Assert.AreEqual("Calculator", result.name);
            Assert.AreEqual("TDSAcc-Calculator", result.studentPackageFieldName);
            Assert.AreEqual("Language", result.DependsOnToolType);
            Assert.IsTrue(result.allowChange);
            Assert.IsTrue(result.Visible);
            Assert.IsTrue(result.Selectable);
            Assert.IsFalse(result.allowMultiple);
            Assert.IsFalse(result.disableOnGuest);
            Assert.IsFalse(result.required);
            Assert.IsFalse(result.StudentControl);
            Assert.AreEqual(0, result.sortOrder);

            // Verify the Options
            Assert.AreEqual(2, result.Options.Length);

            var firstOption = result.Options.First();
            Assert.AreEqual("TDS_Calc0", firstOption.code);
            Assert.AreEqual(0, firstOption.sortOrder);
            Assert.AreEqual(1, firstOption.Dependencies.Length);

            // Verify first option's dependencies
            var firstOptionDependency = firstOption.Dependencies.First();
            Assert.AreEqual("Language", firstOptionDependency.ifToolType);
            Assert.AreEqual("ENU-Braille", firstOptionDependency.ifToolCode);
            Assert.IsFalse(firstOptionDependency.enabled);

            var secondOption = result.Options.Last();
            Assert.AreEqual("TDS_CalcSciInv&TDS_CalcGraphingInv&TDS_CalcRegress", secondOption.code);
            Assert.AreEqual(1, secondOption.sortOrder);
            Assert.AreEqual(1, secondOption.Dependencies.Length);

            // Verify second option's dependencies
            var secondOptionDependency = secondOption.Dependencies.First();
            Assert.AreEqual("Language", secondOptionDependency.ifToolType);
            Assert.AreEqual("ENU-Braille", secondOptionDependency.ifToolCode);
            Assert.IsFalse(secondOptionDependency.enabled);
        }

        [TestMethod]
        public void ShouldThrowExceptionWhenNoStudentPackageFieldNameCanBeFound()
        {
            var toolFragment =
                @"<Tool name='I do not exist'></Tool>";

            var toolElement = XElement.Parse(toolFragment);

            var exception = Assert.ThrowsException<InvalidOperationException>(() => ToolMapper.FromXml(toolElement));
            Assert.AreEqual("A tool must have a Student Package Field Name attribute and 'studentPackageField' was not supplied",
                exception.Message);
        }

        [TestMethod]
        public void ShouldThrowExceptionWhenNameIsNotProvided()
        {
            var toolFragment =
                @"<Tool studentPackageFieldName='I am a horse with no name'></Tool>";

            var toolElement = XElement.Parse(toolFragment);

            var exception = Assert.ThrowsException<InvalidOperationException>(() => ToolMapper.FromXml(toolElement));
            Assert.AreEqual("A tool must have a name attribute and one was not supplied",
                exception.Message);
        }
    }
}

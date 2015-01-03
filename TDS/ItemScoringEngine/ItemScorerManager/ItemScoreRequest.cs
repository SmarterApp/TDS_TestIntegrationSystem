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
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using AIR.Common.Web;

namespace TDS.ItemScoringEngine
{
    /*
        <ItemScoreRequest callbackUrl="http://localhost/ItemScoreClient/Scored.axd">
            <ResponseInfo itemIdentifier="I-100-1001" itemFormat="GI">
                <StudentResponse encrypted="true"><![CDATA[<Question></Question>]]></StudentResponse>
                <Rubric type="Uri" cancache="true" encrypted="true">c:\rubrics\rubric.xml</Rubric>
                <ContextToken><![CDATA[xxxxxxx]]></ContextToken>
                <IncomingBindings>
                    <Bind name="chosenoption" type="string" value="A"/>              
                </IncomingBindings>
                <OutgoingBindings> 
                    <Binding name="objectsetACount"/>              
                </OutgoingBindings>
            </ResponseInfo>            
        </ItemScoreRequest>
    */

    /// <summary>
    /// A class used for transporting a scoring request.
    /// </summary>
    public class ItemScoreRequest : IXmlSerializable
    {
        public ResponseInfo ResponseInfo { get; set; }
        public string CallbackUrl { get; set; }        

        public ItemScoreRequest()
        {
        }

        public ItemScoreRequest(ResponseInfo responseInfo)
        {
            ResponseInfo = responseInfo;
        }

        public void ReadXml(XmlReader reader)
        {
            // <ItemScoreRequest>
            reader.MoveToContent();
            
            // "callbackUrl"
            if (reader.MoveToAttribute("callbackUrl"))
            {
                CallbackUrl = reader.ReadContentAsString();
            }

            // <ResponseInfo>
            reader.ReadToFollowing("ResponseInfo");

            // "itemIdentifier"
            string itemIdentifier = null;
            if (reader.MoveToAttribute("itemIdentifier")) itemIdentifier = reader.ReadContentAsString();

            // "itemFormat"
            string itemFormat = null;
            if (reader.MoveToAttribute("itemFormat")) itemFormat = reader.ReadContentAsString();

            // <StudentResponse>
            string studentResponse = null;
            bool studentResponseIsEncrypted = false;

            if (reader.Name == "StudentResponse" || reader.ReadToFollowing("StudentResponse"))
            {
                if(reader.MoveToAttribute("encrypted"))
                {
                    Boolean.TryParse(reader.ReadContentAsString(), out studentResponseIsEncrypted);
                }

                reader.MoveToElement();

                studentResponse = reader.ReadElementContentAsString();
            }

            // <Rubric>
            object rubric = null;
            RubricContentType rubricContentType = RubricContentType.Uri;
            bool canCache = true;
            bool rubricEncrypted = false;

            if (reader.Name == "Rubric" || reader.ReadToFollowing("Rubric"))
            {
                reader.MoveToAttribute("type");                
                string rubricType = reader.ReadContentAsString();    
                                                
                if(reader.MoveToAttribute("cancache"))
                {
                    Boolean.TryParse(reader.ReadContentAsString(), out canCache);
                }

                if (reader.MoveToAttribute("encrypted"))
                {
                    Boolean.TryParse(reader.ReadContentAsString(), out rubricEncrypted);
                }
                
                reader.MoveToElement();

                rubricContentType = rubricType == "Data" ? RubricContentType.ContentString : RubricContentType.Uri;
                rubric = reader.ReadElementContentAsString();       // Reading the rubric contents

                // We can safely convert to Uri only if the rubric is not encrypted. If it is, then you have to keep it as a string till someone can decrypt it
                if(!rubricEncrypted && rubricContentType == RubricContentType.Uri)
                {                    
                    rubric = new Uri((string)rubric);       
                }
                
            }

            // Now the optional stuff

            // <ContextToken>
            string contextToken = null;
            if (reader.Name == "ContextToken")
            {
                contextToken = reader.ReadElementContentAsString();
            }
            
            // <IncomingBindings>
            List<VarBinding> incomingBindings = new List<VarBinding>(); 
            if (reader.Name == "IncomingBindings")
            {
                XmlReader bindingsReader = reader.ReadSubtree();
                while (bindingsReader.ReadToFollowing("Binding"))
                {                    
                    string name = bindingsReader.MoveToAttribute("name") ? bindingsReader.ReadContentAsString() : null;
                    string type = bindingsReader.MoveToAttribute("type") ? bindingsReader.ReadContentAsString() : null;
                    string value = bindingsReader.MoveToAttribute("value") ? bindingsReader.ReadContentAsString() : null;

                    if(!String.IsNullOrEmpty(name)&&!String.IsNullOrEmpty(type)&&!String.IsNullOrEmpty(value)) 
                        incomingBindings.Add(new VarBinding(){Name=name, Type = type, Value = value});
                }
                bindingsReader.Close();
                reader.Read();  // move past the </IncomingBindings
            }

            // <OutgoingBindings>
            List<VarBinding> outgoingBindings = new List<VarBinding>();
            if (reader.Name == "OutgoingBindings")
            {
                XmlReader bindingsReader = reader.ReadSubtree();
                while (bindingsReader.ReadToFollowing("Binding"))
                {
                    string name = bindingsReader.MoveToAttribute("name") ? bindingsReader.ReadContentAsString() : null;                    
                    if (!String.IsNullOrEmpty(name))
                        outgoingBindings.Add(new VarBinding() { Name = name, Type = String.Empty, Value = String.Empty });
                }
                bindingsReader.Close();
            }

            ResponseInfo = new ResponseInfo(itemFormat, itemIdentifier, studentResponse, rubric, rubricContentType, contextToken, canCache);
            ResponseInfo.IsStudentResponseEncrypted = studentResponseIsEncrypted;
            ResponseInfo.IsRubricEncrypted = rubricEncrypted;
            ResponseInfo.IncomingBindings = incomingBindings;
            ResponseInfo.OutgoingBindings = outgoingBindings;
        }

        public void WriteXml(XmlWriter writer)
        {
            // The XmlSerializer writes out the <ItemScoreRequest>
            
            if (CallbackUrl != null)
            {
                writer.WriteAttributeString("callbackUrl", CallbackUrl);
            }

            // <ResponseInfo>
            writer.WriteStartElement("ResponseInfo");
            writer.WriteAttributeString("itemIdentifier", ResponseInfo.ItemIdentifier);
            writer.WriteAttributeString("itemFormat", ResponseInfo.ItemFormat);

            // <StudentResponse>
            writer.WriteStartElement("StudentResponse");
            writer.WriteAttributeString("encrypted", ResponseInfo.IsStudentResponseEncrypted.ToString().ToLower());
            writer.WriteCData(ResponseInfo.StudentResponse);
            writer.WriteEndElement(); // </StudentResponse>

            // <Rubric>
            writer.WriteStartElement("Rubric");            
            writer.WriteAttributeString("type", ResponseInfo.ContentType == RubricContentType.ContentString? "Data" : "Uri");
            writer.WriteAttributeString("cancache",ResponseInfo.CanCacheRubric.ToString().ToLower());
            writer.WriteAttributeString("encrypted", ResponseInfo.IsRubricEncrypted.ToString().ToLower());
            writer.WriteCData(ResponseInfo.Rubric.ToString());                       
            writer.WriteEndElement(); // </Rubric>

            // <ContextInfo>
            writer.WriteStartElement("ContextToken");
            writer.WriteCData(ResponseInfo.ContextToken);
            writer.WriteEndElement(); // </ContextInfo>

            // <IncomingBindings>
            if (ResponseInfo.IncomingBindings != null && ResponseInfo.IncomingBindings.Count > 0)
            {
                writer.WriteStartElement("IncomingBindings");
                foreach (var varBinding in ResponseInfo.IncomingBindings)
                {
                    writer.WriteStartElement("Binding");
                    writer.WriteAttributeString("name", varBinding.Name);
                    writer.WriteAttributeString("type", varBinding.Type);
                    writer.WriteAttributeString("value", varBinding.Value);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }

            // <OutgoingBindings>      
            if (ResponseInfo.OutgoingBindings != null && ResponseInfo.OutgoingBindings.Count > 0)
            {
                writer.WriteStartElement("OutgoingBindings");
                foreach (var varBinding in ResponseInfo.OutgoingBindings)
                {
                    writer.WriteStartElement("Binding");
                    writer.WriteAttributeString("name", varBinding.Name);
                    writer.WriteAttributeString("type", varBinding.Type);
                    writer.WriteAttributeString("value", varBinding.Value);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }

            writer.WriteEndElement(); // </ResponseInfo>
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Encrypt response and rubric info
        /// </summary>
        /// <param name="encryptStudentResponse"></param>
        /// <param name="encryptRubric"></param>
        public void Encrypt(bool encryptStudentResponse, bool encryptRubric)
        {
            if(encryptStudentResponse && !ResponseInfo.IsStudentResponseEncrypted)
            {
                ResponseInfo.StudentResponse = EncryptionHelper.EncryptToBase64(ResponseInfo.StudentResponse);
                ResponseInfo.IsStudentResponseEncrypted = true;    
            }
            
            if(encryptRubric && !ResponseInfo.IsRubricEncrypted)
            {
                ResponseInfo.Rubric = EncryptionHelper.EncryptToBase64((string)ResponseInfo.Rubric);
                ResponseInfo.IsRubricEncrypted = true;    
            }            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="decryptStudentResponse"></param>
        /// <param name="decryptRubric"></param>
        public void Decrypt(bool decryptStudentResponse, bool decryptRubric)
        {
            if (decryptStudentResponse && ResponseInfo.IsStudentResponseEncrypted)
            {
                ResponseInfo.StudentResponse = EncryptionHelper.DecryptFromBase64(ResponseInfo.StudentResponse);
                ResponseInfo.IsStudentResponseEncrypted = false;
            }

            if (decryptRubric && ResponseInfo.IsRubricEncrypted)
            {
                ResponseInfo.Rubric = EncryptionHelper.DecryptFromBase64((string)ResponseInfo.Rubric);
                ResponseInfo.IsRubricEncrypted = false;

                // We can now safely convert it to a URI if needed
                if(ResponseInfo.ContentType == RubricContentType.Uri)
                    ResponseInfo.Rubric = new Uri((string)ResponseInfo.Rubric);  
            }   
        }

    }
}
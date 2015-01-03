/*******************************************************************************
* Educational Online Test Delivery System
* Copyright (c) 2014 American Institutes for Research
*
* Distributed under the AIR Open Source License, Version 1.0
* See accompanying file AIR-License-1_0.txt or at
* http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf
******************************************************************************/
using System;
using System.Xml.Serialization;

namespace TDS.ItemScoringEngine
{
    /*
    <ItemScoreResponse>
		<Score>
			<ScoreInfo scorePoint="0" maxScore="2" scoreDimension="overall" confLevel="23.5,45.6" scoreStatus="Scored">
				<ScoreRationale>
					<Propositions>
						<Proposition name="Jar" description="The student correctly orders the liquids by density." state="NotAsserted" />
						<Proposition name="Earth" description="The student correctly ranks the layers of Earth by density." state="NotAsserted" />
						<Proposition name="Match" description="The student understands that Veg. Oil is the Atmosphere, Dish Soap is the Ocean, Milk is the Crust, and Honey is the Core." state="NotAsserted" />
					</Propositions>
					<Bindings />
					<Message>
						<![CDATA[Your response earned a score of 0. Full credit requires: BothOrder]]>
					</Message>
				</ScoreRationale>
				<SubScoreList>
					<!-- a list of children ScoreInfo instances -->
				</SubScoreList>	
			</ScoreInfo>
			<ContextToken>
				<![CDATA[ This token is what we received with the score request ]]>
			</ContextToken> 
			<ScoreLatency>10</ScoreLatency>
		</Score>
	</ItemScoreResponse>
    */

    /// <summary>
    /// A class used for transporting a scoring response.
    /// </summary>
    [Serializable]
    [XmlRoot("ItemScoreResponse")]
    public class ItemScoreResponse
    {
        [XmlElement("Score")]
        public ItemScore Score { get; set; }       

        public ItemScoreResponse()
        {
        }

        public ItemScoreResponse(ItemScore itemScore)
        {
            Score = itemScore;
        }

        public ItemScoreResponse(ItemScore itemScore, string contextToken)
        {
            Score = itemScore;
            Score.ContextToken = contextToken;
        }
    }
}
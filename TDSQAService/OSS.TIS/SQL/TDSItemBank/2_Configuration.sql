---Create TDSConfigs and TestScoringConfigs Synonyms
---NOTE: Make sure TDSConfigs and TestScoringConfgis databases are set up pripr to creating the Synonyms
--Synonyms for TDS Configs
Exec dbo.Create_Synonyms '<TDS Configs dataabse name'>, 0,1
--Synonyms for TestScoringConfigs
Exec Create_Synonyms <'<Test Scoring Configs Database name>',0,0,1


---Inserting into tblclient
Insert into tblclient (name, description,homepath)
values ('SBAC', null, '/usr/local/tomcat/resources/tds/')

---Inserting into tblitembank
insert into tblitembank(_fk_client,homepath,itempath,stimulipath,name,_efk_itembank,_key,contract)
values (1, 'bank/', 'items/', 'stimuli/', null, 1, 1, null);


---Load the test package by running this stored pocduere
EXEC tp.spLoader_Main <'Test Package file(XML file), copy and paste here>'

---run  UpdateTDSConfigs  by running this stored proc
EXEC dbo.UpdateTDSConfigs 1










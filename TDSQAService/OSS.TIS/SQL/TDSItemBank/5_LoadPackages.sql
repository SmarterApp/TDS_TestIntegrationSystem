
select 'TODO: needs input' -- comment out after setting the values below
return;

/*
Use tp.spLoader_Main to load as many admin packages and/or test scoring packages as needed by copying/pasting the package XML file
into the parameter value and repeating for each (new) package file.

Then run dbo.UpdateTDSConfigs once all (new) packages have been loaded.

Note that dbo.UpdateTDSConfigs can be run after each package, but it's not necessary and will be slower than running it once
at the end of a package-loading session.

Alternatively, a simple utility could be written to load all packages in a directory or table and then call UpdateTDSConfigs when done.
*/

---Load admin package(s) or scoring package(s); repeat as many times as there are (new) packages to load.
EXEC tp.spLoader_Main '<Test Package file(XML file), copy and paste here>'

---run UpdateTDSConfigs after loading all (new) test packages.  Will not impact pre-existing data, only data that has been loaded
--	since the last time it was called.
EXEC dbo.UpdateTDSConfigs 1

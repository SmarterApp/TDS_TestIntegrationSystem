declare @ItemBankKey bigint
declare @clientKey bigint

-- currently set to the item bank key for the SBAC ICA tests
set @ItemBankKey = 200

---Inserting into tblclient
Insert into tblclient (name)
values ('SBAC')

set @clientKey = @@IDENTITY

---Inserting into tblitembank
insert into tblitembank(_fk_client, _efk_itembank, _key)
values (@clientKey, @ItemBankKey, @ItemBankKey);

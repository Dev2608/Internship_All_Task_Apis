alter procedure spGetPurchaseId

as
begin
select * from purchase_table
end

spGetPurchaseId


alter procedure spGetAllProductsById

@param int

as
begin
select * from purchaseProductTable where purchase_id = @param
end

spGetAllProductsById 1001

create procedure spGetAllDetailByOne

as 
begin
select * from purchase_table
select * from purchaseProductTable;
end

spGetAllDetailByOne
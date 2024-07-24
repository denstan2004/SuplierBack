using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;
using Dapper;
using project_back.Models;
using project_back.ViewModel;
using project_back.Helpers;
using project_back.Models.Enums;
using project_back.Models.DiscountModels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace project_back
{
    public class Oracle
    {
        OracleConnection connection = null;
        //OracleTransaction transaction = null;
        string ConectionString = "";
     
        
        public Oracle(string UserId,string password)
        {
            ConectionString = $"Data Source=ZNP.vopak.local:1521/orcl.vopak.local; User Id = {UserId}; Password={password};";
            connection = new OracleConnection(ConectionString);
        }

        public string ExecuteApi(string p)
        {
            var cmd = new OracleCommand();
            cmd.Connection = connection;
            cmd.CommandText = "c.web.Api";
            cmd.CommandType = CommandType.StoredProcedure;
            string res = $"{{ \"State\": -1, \"TextError\":\"Rez=|>NULL\"}}";


            cmd.Parameters.Add("res", OracleDbType.Clob, res, ParameterDirection.ReturnValue);
            cmd.Parameters.Add("parData", OracleDbType.Clob, p, ParameterDirection.Input);
            cmd.Parameters.Add("is_utf8", OracleDbType.Int64, (object)1, ParameterDirection.Input);

            try
            {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                return $"{{ \"State\": -1, \"TextError\":\"{e.Message}\" }}";
            }

            OracleClob aa = (OracleClob)cmd.Parameters["res"].Value;
            if (aa == null || aa.Value == null)
            {
              //  FileLogger.WriteLogMessage($"Oracle\\ExecuteApi\\{this.ConectionString}\\{p} \\ Res=> NULL");
            }
            else
            {
                res = aa.Value.ToString();
            }
            cmd.Connection.Close();
            return res;
        }
        public Status<UserRolesOracle> GetRole(string UserId, string password)
        {
            try {

                var role= connection.Query<UserRolesOracle>(@"select c.useraccess.GetUserProfile(null,-2 ) as IsManager, c.useraccess.GetUserProfile(null,-4 ) as IsSupplier from dual").FirstOrDefault();
                return new Status<UserRolesOracle>(role);
            }
            catch (Exception ex)
            {
                return new Status<UserRolesOracle>(System.Net.HttpStatusCode.Unauthorized);
            } 
        }

        // Specification Metods 
      public Status< List<SuplierPostition>> GetAllSuplier()
        {
            try
            {
                var aa = connection.Query<SuplierPostition>(@"
               select distinct  a.code_supplier as CodeFirm,  w.code_wares as CodeWares ,w.articl as Aritcle,w.name_wares as NameWares,b.code_brand as BrandCode,b.name_brand as NameBrand ,C.PriceSupplier.GetPriceSuply(parCodeSupplagrRegister => gs.code_supplagr_register,
                                                                                                                            parCodeWares            => w.code_wares) as price
                ,br.bar_code as BarCode
                , (select  nvl(max(se.price),0)  from c.specification_edit se  where  se.codefirm=a.code_supplier and se.codewares=w.code_wares) as UpdatedPrice
                , (select  max(se.Datestart)  from c.specification_edit se  where  se.codefirm=a.code_supplier and se.codewares=w.code_wares) as DateStart
                ,  (select commentspec  from c.specification_edit se  where  se.codefirm=a.code_supplier and se.codewares=w.code_wares) as commentspec
                , (select  se.status  from c.specification_edit se  where  se.codefirm=a.code_supplier and se.codewares=w.code_wares) as status
                ,(select  max(se.DATESPECIFICATION   )  from c.specification_edit se  where  se.codefirm=a.code_supplier and se.codewares=w.code_wares ) as DATESPECIFICATION

                 from  c.group_supply gs 
                join c.group_supply_brand gsb on gs.code_group_supply=gsb.code_group_supply
                join dw.brand b on gsb.code_brand=b.code_brand
                join dw.wares w on w.code_brand=b.code_brand
                join (select distinct la.code_wares  from c.list_assortment la where la.quantity>0) la on la.code_wares = w.code_wares
                 join (select  br.code_wares,LISTAGG(br.bar_code,';')  WITHIN GROUP (ORDER BY bar_code) bar_code from dw.bar_code_additional_unit br group by  code_wares )br on br.code_wares=  w.code_wares
                 join dw.agr_supplagr_register a on gs.code_supplagr_register=a.code_supplagr_register 
                 --join dw.firms f on f.code_firm=a.code_supplier

                where c.useraccess.GetUserAccess(parLevelAccess => 22,parCodeUser => null,   parTypeAccess => 5,parCodeAccess => gs.code_group_supply ) = 1  and gs.code_group_supply>0").ToList();
                return  new Status<List<SuplierPostition>> (aa);
            }
            catch(Exception ex)
            {
                return new Status<List<SuplierPostition>> (ex);
            }
        }
        public Status CreateRequest(PriceChangeRequest request)
        {
            try
            {
                connection.Execute(
                @"MERGE INTO c.specification_edit target
                    USING (SELECT :CodeFirm AS CodeFirm, :CodeWares AS CodeWares FROM dual) source
                    ON (target.CODEFIRM = source.CodeFirm AND target.CODEWARES = source.CodeWares)
                 WHEN MATCHED THEN
                    UPDATE SET
                        DATESPECIFICATION = TO_DATE(:DATESPECIFICATION, 'YYYY-MM-DD'),
                        DATESTART = TO_DATE(:DateStart, 'YYYY-MM-DD'),
                        PRICE = :Price,
                        STATUS = :Status,
                        COMMENTSPEC = :CommentText
                 WHEN NOT MATCHED THEN
                    INSERT (DATESPECIFICATION, CODEFIRM, DATESTART, CODEWARES, PRICE, STATUS, COMMENTSPEC)
                    VALUES (TO_DATE(:DATESPECIFICATION, 'YYYY-MM-DD'), :CodeFirm, TO_DATE(:DateStart, 'YYYY-MM-DD'), :CodeWares, :Price, :Status, :CommentText)", new
       {
           DATESPECIFICATION = DateTime.Now.ToString("yyyy-MM-dd"),
           CodeFirm = request.CodeFirm,
           DateStart = request.DateStart.ToString("yyyy-MM-dd"),
           CodeWares = request.CodeWares,
           Price = request.Price,
           Status = request.status,
           CommentText = request.CommentSpec
       });
            
                return new Status();
            }
            catch(Exception ex) 
            {
                return new Status(ex);
            }

        }
      /*  public Status<List<PriceChangeRequest>> GetAllSpecificationRequest()
        {
            try
            {
                var request = connection.Query<PriceChangeRequest>("Select * FROM c.specification_edit");
                return new Status<List<PriceChangeRequest>> (request.ToList());
            }
            catch (Exception ex)
            {
                return new Status<List<PriceChangeRequest>> (ex);
            }
        }*/

        public Status<List<SuplierPostition>> GetAllSpecificationManager()
        {
            try
            {
                var aa = connection.Query<SuplierPostition>(@"
SELECT DISTINCT 
     se.codefirm AS CodeFirm,  
     w.code_wares AS CodeWares,
     w.articl AS Aritcle,
     w.name_wares AS NameWares,
     b.code_brand AS BrandCode,
     b.name_brand AS NameBrand,
     C.PriceSupplier.GetPriceFirm(pCodeFirm => se.codefirm ,pCodeWares => se.CodeWares) AS price,
     br.bar_code AS BarCode,
     se.price AS UpdatedPrice,
     se.Datestart AS DateStart,
      se.commentspec AS commentspec,
      se.status  AS status,
      se.DATESPECIFICATION AS DATESPECIFICATION
     ,f.name AS GroupName
 FROM  c.specification_edit se    
     join dw.wares w ON w.Code_Wares = se.CodeWares
     JOIN dw.brand b ON w.code_brand = b.code_brand 
     join dw.firms f on f.code_firm=se.codefirm
     JOIN (SELECT DISTINCT la.code_wares FROM c.list_assortment la WHERE la.quantity > 0) la ON la.code_wares = w.code_wares
     JOIN (SELECT br.code_wares, LISTAGG(br.bar_code, ';') WITHIN GROUP (ORDER BY bar_code) bar_code FROM dw.bar_code_additional_unit br GROUP BY code_wares) br ON br.code_wares = w.code_wares
     --JOIN dw.agr_supplagr_register a ON gs.code_supplagr_register = a.code_supplagr_register 
 WHERE c.useraccess.GetUserAccess(parLevelAccess => 15, parTypeAccess => 5,parCodeAccess => c.procwares.GetDirection(w.code_group )) = 1
      and  se.status = 0
    ").ToList();
                return new Status<List<SuplierPostition>> (aa);
            }
            catch(Exception ex)
            {
                return new Status<List<SuplierPostition>> (ex);
            }
        }

        public Status UpdateRequest(ChangeRequestStatus change)
        {
            try
            {
                connection.Execute(@"UPDATE c.specification_edit
                             SET status = :status, commentspec = :commentspec
                             WHERE codefirm = :codefirm AND codewares = :codewares AND status = 0",
                                     new
                                     {
                                         status = change.status,
                                         commentspec = change.CommentSpec,
                                         codefirm = change.CodeFirm,
                                         codewares = change.CodeWares
                                     });
                return new Status(0,"статус змінено успішно");
            }
            catch (Exception ex)
            {
                return new Status(ex);
            }
        }
        public SuplierPostition GetSpecificationByCode(string code)
        {
            var aa = connection.Query<SuplierPostition>(@"
   select distinct  a.code_supplier as CodeFirm,  w.code_wares as CodeWares ,w.articl as Aritcle,w.name_wares as NameWares,b.code_brand as BrandCode,b.name_brand as NameBrand ,C.PriceSupplier.GetPriceSuply(parCodeSupplagrRegister => gs.code_supplagr_register,
                                                                                                             parCodeWares            => w.code_wares) as price
 ,br.bar_code as BarCode
 , (select  nvl(max(se.price),0)  from c.specification_edit se  where  se.codefirm=a.code_supplier and se.codewares=w.code_wares) as UpdatedPrice
 , (select  max(se.Datestart)  from c.specification_edit se  where  se.codefirm=a.code_supplier and se.codewares=w.code_wares) as DateStart
 ,  (select commentspec  from c.specification_edit se  where  se.codefirm=a.code_supplier and se.codewares=w.code_wares) as commentspec
 , (select  se.status  from c.specification_edit se  where  se.codefirm=a.code_supplier and se.codewares=w.code_wares) as status
 ,(select  max(se.DATESPECIFICATION   )  from c.specification_edit se  where  se.codefirm=a.code_supplier and se.codewares=w.code_wares ) as DATESPECIFICATION
,gs.name_group_supply AS GroupName
  from  c.group_supply gs 
 join c.group_supply_brand gsb on gs.code_group_supply=gsb.code_group_supply
 join dw.brand b on gsb.code_brand=b.code_brand
 join dw.wares w on w.code_brand=b.code_brand
 join (select distinct la.code_wares  from c.list_assortment la where la.quantity>0) la on la.code_wares = w.code_wares
  join (select  br.code_wares,LISTAGG(br.bar_code,';')  WITHIN GROUP (ORDER BY bar_code) bar_code from dw.bar_code_additional_unit br group by  code_wares )br on br.code_wares=  w.code_wares
  join dw.agr_supplagr_register a on gs.code_supplagr_register=a.code_supplagr_register 
  --join dw.firms f on f.code_firm=a.code_supplier

 where c.useraccess.GetUserAccess(parLevelAccess => 22,parCodeUser => null,   parTypeAccess => 5,parCodeAccess => gs.code_group_supply ) = 1  and gs.code_group_supply>0 and w.code_wares=:code
    ", new {code}).ToList().FirstOrDefault();
            return aa;
        }

        //-----------------------------------             методи знижки 
        public  Status AddDiscount(AddDiscountVM addDiscountVM)
        {
            using (var connection = new OracleConnection(ConectionString))
            {
                try
                {
                    connection.Open(); // Ensure connection is open
                    foreach (var disc in addDiscountVM.DiscountPositions)
                    {
                        connection.Execute(
                                        @"MERGE INTO  c.discounts target
                        USING (SELECT :codewares AS CodeWares, :number_ AS Number_ FROM dual) source
                        ON (target.CODEWARES = source.CodeWares AND target.Number_ = source.Number_)
                        WHEN MATCHED THEN 
                            UPDATE SET
                                Status = :status,
                                PlanedSales = :planedsales,
                                DiscountPrice = :discountprice,
                                CompensationAmount = :compensationamount,
                                DiscountInitPrice = :discountinitprice
                        WHEN NOT MATCHED THEN
                            INSERT (Status, PlanedSales, DiscountPrice, CompensationAmount, DiscountInitPrice, Number_, CODEWARES)
                            VALUES (:status, :planedsales, :discountprice, :compensationamount, :discountinitprice, :number_, :codewares)",
                            new
                            {
                                status = (int)RequestStatus.Pennding,
                                number_ = addDiscountVM.DiscountNumber,
                                planedsales = disc.PlanedSales,
                                discountprice = disc.DiscountPrice,
                                discountinitprice = disc.DiscountInitPrice,
                                compensationamount = disc.CompensationAmount,
                                codewares = disc.CodeWares
                            });
                    }
                    return new Status(0, "Пропозицію на знижку додано");
                }
                catch (Exception ex)
                {
                    // Log the exception
                   // Console.WriteLine(ex.Message);
                    return new Status(ex);

                }
                finally
                {
                    connection.Close(); // Ensure connection is closed
                }
            }
        }


        public Status<List<DiscountRequestModel>> GetAllDiscRequests( bool isSuplier)
        {
            string query;
            if (isSuplier==false)
           query= @"SELECT PlanedSales as PlannedSales, DiscountInitPrice, CompensationAmount, DiscountPrice, Number_, CodeWares, Status, CommentDisc as DiscountComment 
                  FROM c.discounts 
                  WHERE Status = 0";
            else
            {
                query = @"SELECT PlanedSales as PlannedSales, DiscountInitPrice, CompensationAmount, DiscountPrice, Number_ as Number_, CodeWares, Status, CommentDisc as DiscountComment 
                  FROM c.discounts";
            }

            using (var connection = new OracleConnection(ConectionString))
            {
                try
                {
                    return new Status<List<DiscountRequestModel>> (connection.Query<DiscountRequestModel>(query).ToList());
                }
                catch (Exception e)
                {
                    // Log exception
                    
                   return new Status<List<DiscountRequestModel>>(e);
                }
            }
        }
      
        public Status UpdateStatus(RequestStatus status, string number_, string commentdisc, string codewares)
        {
            var query = @"UPDATE c.discounts 
                  SET Status = :status, CommentDisc = :commentdisc
                  WHERE CodeWares = :codewares AND Number_ = :number_";

            using (var connection = new OracleConnection(ConectionString))
            {
                try
                {
                    connection.Execute(query, new { status = (int)status, number_, commentdisc, codewares });
                    return new Status(0,"Повідомлення надіслано");
                }
                catch (Exception e)
                {
                    // Log exception
                    return new Status(e);
                }
            }
        }





    }

}

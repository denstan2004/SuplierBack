using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;
using Dapper;
using project_back.Models;

namespace project_back
{
    public class Oracle
    {
        OracleConnection connection = null;
        //OracleTransaction transaction = null;
        string ConectionString = "";
        public Oracle()
        {
            ConectionString = $"Data Source=ZNP.vopak.local:1521/orcl.vopak.local; User Id = test_spl; Password=test_spl;";
            connection = new OracleConnection(ConectionString);
            //connection.Open();
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

      public  List<SuplierPostition> res()
        {
            var aa = connection.Query<SuplierPostition>(@"
                SELECT DISTINCT 
                    gs.code_supplagr_register AS Code,
                    w.code_wares AS CodeWares,
                    w.articl AS Aritcle,
                    w.name_wares AS NameWares,
                    b.code_brand AS BrandCode,
                    b.name_brand AS NameBrand,
                    C.PriceSupplier.GetPriceSuply(
                        parCodeSupplagrRegister => gs.code_supplagr_register,
                        parCodeWares => w.code_wares) AS Price,
                    br.bar_code AS BarCode
                FROM c.group_supply gs 
                JOIN c.group_supply_brand gsb ON gs.code_group_supply = gsb.code_group_supply
                JOIN dw.brand b ON gsb.code_brand = b.code_brand
                JOIN dw.wares w ON w.code_brand = b.code_brand
                JOIN (
                    SELECT DISTINCT la.code_wares  
                    FROM c.list_assortment la 
                    WHERE la.quantity > 0
                ) la ON la.code_wares = w.code_wares
                JOIN (
                    SELECT br.code_wares, LISTAGG(br.bar_code, ';') WITHIN GROUP (ORDER BY bar_code) AS bar_code  
                    FROM dw.bar_code_additional_unit br 
                    GROUP BY code_wares 
                ) br ON br.code_wares = w.code_wares
                WHERE c.useraccess.GetUserAccess(
                    parLevelAccess => 22,
                    parCodeUser => NULL,
                    parTypeAccess => 5,
                    parCodeAccess => gs.code_group_supply
                ) = 1 AND gs.code_group_supply > 0
                ORDER BY b.name_brand, w.name_wares").ToList();
            return aa;
        }
    }
}

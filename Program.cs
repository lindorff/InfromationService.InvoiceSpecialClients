using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpecialClients.Utility;

namespace SpecialClients
{
    internal class Program
    {
        static void Main(string[] args)
        {
            
            
            try
            {
                // Get dates for invoicing period
                var firstDayOfBillableMonth = DateTimeHelper.GetFirstDayOfLastMonth();
                var lastDayOfBillableMonth = DateTimeHelper.GetLastDayOfLastMonth();
                string connection = ConfigurationManager.ConnectionStrings["InformationServiceEntities"].ConnectionString;
                SqlConnection conn = new SqlConnection(connection);
                SqlDataReader rdr = null;
                Console.WriteLine("Openning connecton...");
                conn.Open();
                SqlCommand cmd = new SqlCommand("GetProductRowsForInvoicingTinyFee_Kauppalehti", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@StartDate", SqlDbType.DateTime).Value = firstDayOfBillableMonth;
                cmd.Parameters.Add("@EndDate", SqlDbType.DateTime).Value = lastDayOfBillableMonth;
                rdr = cmd.ExecuteReader();
                List<InvoiceRow> list = new List<InvoiceRow>();
               while (rdr.Read())
               {
                    InvoiceRow row = new InvoiceRow();
                    row.ClientBusinessId = rdr["ClientBusinessId"].ToString();
                    row.ClientName = rdr["ClientName"].ToString();
                    row.Amount = rdr["Amount"].ToString();
                    row.Description = rdr["Description"].ToString();
                    row.ProductCode = rdr["ProductCode"].ToString();
                    list.Add(row);
                }
                var billableRowLogData = BillableDataExportFileBuilder.BuildInformationServiceBillableExportData(list, firstDayOfBillableMonth, lastDayOfBillableMonth);
                BillableExportFileWriter.BuildCreditServiceBillableLogFile(billableRowLogData);
                Console.WriteLine("Connection successful...");
            }
            catch (Exception e) {
                Console.WriteLine("error..."+e.Message);
                throw;
            }
        

        } 
    }
}

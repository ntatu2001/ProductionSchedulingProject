using System.Data;

namespace ProductionSchedulingProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            DataTable bomTable = ReadDataFromCSV.ReadDataFromBOM();
            Console.WriteLine();
            DataTable partnerTable = ReadDataFromCSV.ReadDataFromPartners();
            Console.WriteLine();
            DataTable productTable = ReadDataFromCSV.ReadDataFromProducts();
            Console.WriteLine();
            DataTable workCenterTable = ReadDataFromCSV.ReadDataFromWorkCenters();
            Console.WriteLine();
            DataTable orderTable = ReadDataFromCSV.ReadDataFromOrders();
            Console.WriteLine();

            //List<JobInfor> jobInfors = GetData.dataDatePlannedStart(bomTable, partnerTable, productTable, workCenterTable, orderTable);

            DataTable scheduledOrderTable = TabuSearch.returnScheduledDataTable(orderTable, workCenterTable, productTable, partnerTable, bomTable);
        }
    }
}

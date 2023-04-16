using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductionSchedulingProject
{
    public class ReadDataFromCSV
    {
        /// <summary>
        /// Create a DataTable of Maintenance Deadlines and Tasks
        /// </summary>
        /// <returns></returns>
        public static DataTable ReadDataFromOrders()
        {
            string filePath = @"D:\Scheduling Maintenance\data for production scheduling\File CSV\Orders.csv";
            DataTable orderTable = new DataTable();
            orderTable.Columns.Add("DueDate");
            orderTable.Columns.Add("NoOfPartner");
            orderTable.Columns.Add("Partner");
            orderTable.Columns.Add("ProductCode");
            orderTable.Columns.Add("Quantity");
            orderTable.Columns.Add("ReleaseDate");
            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    reader.ReadLine();
                    while ((line = reader.ReadLine()) != null)
                    {
                        DataRow newRow = orderTable.NewRow();
                        newRow["DueDate"] = line.Split(',')[0];
                        newRow["NoOfPartner"] = line.Split(',')[1];
                        newRow["Partner"] = line.Split(',')[2];
                        newRow["ProductCode"] = line.Split(',')[3];
                        newRow["Quantity"] = line.Split(',')[4];
                        newRow["ReleaseDate"] = line.Split(',')[5];
                        orderTable.Rows.Add(newRow);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            //foreach (DataRow row in orderTable.Rows)
            //{
            //    Console.WriteLine(row["DueDate"] + " " + row["NoOfPartner"] + " " + row["Partner"] + " " + row["ProductCode"] + " " + row["Quantity"] + " " + row["ReleaseDate"]);
            //}

            return orderTable;
        }

        /// <summary>
        /// Create a DataTable of Device Working Hours 
        /// </summary>
        /// <returns></returns>
        public static DataTable ReadDataFromPartners()
        {
            string filePath = @"D:\Scheduling Maintenance\data for production scheduling\File CSV\Partners.csv";
            DataTable partnerTable = new DataTable();
            partnerTable.Columns.Add("No");
            partnerTable.Columns.Add("Weight");
            partnerTable.Columns.Add("Partner");
            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    reader.ReadLine();
                    while ((line = reader.ReadLine()) != null)
                    {
                        DataRow newRow = partnerTable.NewRow();
                        newRow["No"] = line.Split(',')[0];
                        newRow["Weight"] = double.Parse(line.Split(',')[1]);
                        newRow["Partner"] = line.Split(',')[2];
                        partnerTable.Rows.Add(newRow);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            foreach (DataRow row in partnerTable.Rows)
            {
                Console.WriteLine(row["No"] + " " + row["Weight"] + " " + row["Partner"]);
            }

            return partnerTable;
        }


        /// <summary>
        /// Create a DataTable of Technician Working Hours 
        /// </summary>
        /// <returns></returns>
        public static DataTable ReadDataFromWorkCenters()
        {
            string filePath = @"D:\Scheduling Maintenance\data for production scheduling\File CSV\WorkCenters.csv";
            DataTable workCenterTable = new DataTable();
            workCenterTable.Columns.Add("Code");
            workCenterTable.Columns.Add("WorkCenter");
            workCenterTable.Columns.Add("Capacity");
            workCenterTable.Columns.Add("OEETarget");
            workCenterTable.Columns.Add("TimeEfficient");
            workCenterTable.Columns.Add("WorkingHours");
            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    reader.ReadLine();
                    while ((line = reader.ReadLine()) != null)
                    {
                        DataRow newRow = workCenterTable.NewRow();
                        newRow["Code"] = line.Split(',')[0];
                        newRow["WorkCenter"] = line.Split(',')[1];
                        newRow["Capacity"] = int.Parse(line.Split(',')[2]);
                        newRow["OEETarget"] = double.Parse(line.Split(',')[3]);
                        newRow["TimeEfficient"] = double.Parse(line.Split(',')[4]);
                        newRow["WorkingHours"] = line.Split(',')[5];
                        workCenterTable.Rows.Add(newRow);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            //foreach (DataRow row in workCenterTable.Rows)
            //{
            //    Console.WriteLine(row["Code"] + " " + row["WorkCenter"] + " " + row["Capacity"] + " " + row["OEETarget"] + " " + row["TimeEfficient"] + " " + row["WorkingHours"]);
            //}

            return workCenterTable;
        }

        /// <summary>
        /// Create a DataTable of spare Part in Warehouse
        /// </summary>
        /// <returns></returns>
        public static DataTable ReadDataFromBOM()
        {
            string filePath = @"D:\Scheduling Maintenance\data for production scheduling\File CSV\BOM.csv";
            DataTable BOMTable = new DataTable();
            BOMTable.Columns.Add("Product");
            BOMTable.Columns.Add("Reference");
            BOMTable.Columns.Add("BOMType");
            BOMTable.Columns.Add("Quantity");
            BOMTable.Columns.Add("UnitOfMeasure");
            BOMTable.Columns.Add("BOMComponent");
            BOMTable.Columns.Add("BOMProductUnitOfMeasure");
            BOMTable.Columns.Add("BOMQuantity");
            BOMTable.Columns.Add("OperationsOperation");
            BOMTable.Columns.Add("OperationsWorkCenter");
            BOMTable.Columns.Add("OperationsAlternativeWorkCenters");
            BOMTable.Columns.Add("OperationsDurationComputation");
            BOMTable.Columns.Add("OperationsManualDuration");
            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    reader.ReadLine();
                    while ((line = reader.ReadLine()) != null)
                    {
                        DataRow newRow = BOMTable.NewRow();
                        newRow["Product"] = line.Split(',')[0];
                        newRow["Reference"] = line.Split(',')[1];
                        newRow["BOMType"] = line.Split(',')[2];
                        newRow["Quantity"] = line.Split(',')[3];
                        newRow["UnitOfMeasure"] = line.Split(',')[4];
                        newRow["BOMComponent"] = line.Split(',')[5];
                        newRow["BOMProductUnitOfMeasure"] = line.Split(',')[6];
                        newRow["BOMQuantity"] = line.Split(',')[7];
                        newRow["OperationsOperation"] = line.Split(',')[8];
                        newRow["OperationsWorkCenter"] = line.Split(',')[9];
                        newRow["OperationsAlternativeWorkCenters"] = line.Split(',')[10];
                        newRow["OperationsDurationComputation"] = line.Split(',')[11];
                        newRow["OperationsManualDuration"] = double.Parse(line.Split(',')[12]);
                        BOMTable.Rows.Add(newRow);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            //foreach (DataRow row in BOMTable.Rows)
            //{
            //    Console.WriteLine(row["Product"] + " " + row["Reference"] + " " + row["BOMType"] + " " + row["Quantity"] + " " + row["UnitOfMeasure"] + " " + row["BOMComponent"] + " " + row["BOMProductUnitOfMeasure"]
            //        + " " + row["BOMQuantity"] + " " + row["OperationsOperation"] + " " + row["OperationsWorkCenter"] + " " + row["OperationsAlternativeWorkCenters"]  + " " + row["OperationsDurationComputation"] 
            //        + " " + row["OperationsManualDuration"]);
            //}

            return BOMTable;
        }

        /// <summary>
        /// Create a DataTable of spare Part in Warehouse
        /// </summary>
        /// <returns></returns>
        public static DataTable ReadDataFromProducts()
        {
            //string filePath = @"D:\Scheduling Maintenance\data input\sparePartList.csv";

            string filePath = @"D:\Scheduling Maintenance\data for production scheduling\File CSV\Products.csv";
            DataTable productTable = new DataTable();
            productTable.Columns.Add("Name");
            productTable.Columns.Add("InternalReference");
            productTable.Columns.Add("SalesPrice");
            productTable.Columns.Add("Mold");
            productTable.Columns.Add("ProductCategory");
            productTable.Columns.Add("ProductType");
            productTable.Columns.Add("CanBePurchased");
            productTable.Columns.Add("CanBeSold");
            productTable.Columns.Add("Routes");
            productTable.Columns.Add("Description");
            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    reader.ReadLine();
                    while ((line = reader.ReadLine()) != null)
                    {
                        DataRow newRow = productTable.NewRow();
                        newRow["Name"] = line.Split(',')[0];
                        newRow["InternalReference"] = line.Split(',')[1];
                        newRow["SalesPrice"] = double.Parse(line.Split(',')[2]);
                        newRow["Mold"] = line.Split(',')[3];
                        newRow["ProductCategory"] = line.Split(',')[4];
                        newRow["ProductType"] = line.Split(',')[5];
                        newRow["CanBePurchased"] = line.Split(',')[6];
                        newRow["CanBeSold"] = line.Split(',')[7];
                        newRow["Routes"] = line.Split(',')[8];
                        newRow["Description"] = line.Split(',')[8];
                        productTable.Rows.Add(newRow);
                    }

                    //Preprocessing to sort job orders, the higher the priority get the smaller the order number
                    //workTable.DefaultView.Sort = "Priority";
                    //workTable = workTable.DefaultView.ToTable();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            //foreach (DataRow row in productTable.Rows)
            //{
            //    Console.WriteLine(row["Name"] + " " + row["InternalReference"] + " " + row["SalesPrice"] + " " + row["Mold"] + " " + row["ProductCategory"] + " " + row["ProductType"] + " " + row["CanBePurchased"] + " " + row["CanBeSold"] + " " + row["Routes"] + " " + row["Description"]);
            //}

            return productTable;
        }

        /// <summary>
        /// Create an empty dictionary is similar to deviceDictionary. 
        /// </summary>
        /// <param name="workCenterTable"></param>
        /// <returns></returns>
        public static Dictionary<string, List<JobInfor>> deviceStructure(DataTable workCenterTable)
        {
            Dictionary<string, List<JobInfor>> productionDeviceDict = new Dictionary<string, List<JobInfor>>();
            foreach (DataRow row in workCenterTable.Rows)
            {
                List<JobInfor> listJobInforTemp = new List<JobInfor>();
                productionDeviceDict.Add(row["WorkCenter"].ToString(), listJobInforTemp);
            }

            return productionDeviceDict;
        }
    }
}

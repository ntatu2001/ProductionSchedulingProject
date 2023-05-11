using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductionSchedulingProject
{
    public class JobInfor
    {
        public int id { get; set; }
        public double priority { get; set; }
        public string? productCode { get; set; }
        public string? codeBOM { get; set; }
        public string? workCenter { get; set; }
        public List<string>? alternativeWorkCenters { get; set; }
        public string? mold { get; set; }
        public DateTime releaseDate { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public DateTime dueDate { get; set; }
        public double processingTime { get; set; }
    }

    public class GetData
    {
        public static List<JobInfor> dataDatePlannedStart(DataTable bomTable, DataTable partnerTable, DataTable productTable,
                                                     DataTable workCenterTable, DataTable orderTable)
        {
            List<JobInfor> listOrderInfor = new List<JobInfor>();
            foreach (DataRow orderRow in orderTable.Rows)
            {
                JobInfor orderInfor = new JobInfor();
                orderInfor.id = orderTable.Rows.IndexOf(orderRow) + 1;
                orderInfor.dueDate = DateTime.ParseExact(orderRow["DueDate"].ToString(), "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                orderInfor.productCode = orderRow["ProductCode"].ToString();
                orderInfor.releaseDate = DateTime.ParseExact(orderRow["ReleaseDate"].ToString(), "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                orderInfor.startDate = DateTime.Now;
                orderInfor.endDate = DateTime.Now;
                double numberOfProduct = double.Parse(orderRow["Quantity"].ToString());
                int NoOfPartner = int.Parse(orderRow["NoOfPartner"].ToString());
                foreach (DataRow partnerRow in partnerTable.Rows)
                {
                    int noTemp = int.Parse(partnerRow["No"].ToString());
                    double weightTemp = double.Parse(partnerRow["Weight"].ToString());
                    if (noTemp == NoOfPartner)
                    {
                        orderInfor.priority = weightTemp;
                        break;
                    }
                }

                double processingTimeOfUnitProduct = 0;
                foreach (DataRow bomRow in bomTable.Rows)
                {
                    string productCode = bomRow["Reference"].ToString();
                    if (productCode == orderInfor.productCode)
                    {
                        orderInfor.workCenter = bomRow["OperationsWorkCenter"].ToString();
                        orderInfor.alternativeWorkCenters = bomRow["OperationsAlternativeWorkCenters"].ToString().Split(' ').ToList();
                        processingTimeOfUnitProduct = double.Parse(bomRow["OperationsManualDuration"].ToString());
                        orderInfor.processingTime = processingTimeOfUnitProduct * numberOfProduct;
                        orderInfor.codeBOM = bomRow["OperationsOperation"].ToString();
                    }
                }

                foreach(DataRow productRow in productTable.Rows)
                {
                    string productCode = productRow["InternalReference"].ToString();
                    if (productCode == orderInfor.productCode)
                    {
                        orderInfor.mold = productRow["Mold"].ToString();
                    }
                }

                listOrderInfor.Add(orderInfor);
            }

            //foreach (JobInfor job in listOrderInfor)
            //{
            //    Console.WriteLine($"{job.priority} + {job.productCode} + {job.codeBOM} + {job.workCenter} + {job.alternativeWorkCenters.Count} +{job.mold} + {job.releaseDate} + {job.startDate} + {job.endDate} + {job.dueDate} + {job.processingTime}");
            //    Console.WriteLine("-----------------------------------------------------------------------------------------");
            //}
            return listOrderInfor;
        }
    }
}

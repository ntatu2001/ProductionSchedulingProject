using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductionSchedulingProject
{
    public class FindPlannedDate
    {

        public static JobInfor findPlannedDate(JobInfor jobInfor, DateTime firstDateStart, Dictionary<string, List<JobInfor>> productionDeviceDict)
        {
            string nameOfDevice = jobInfor.workCenter;
            int numberOfWorkOnDevice = productionDeviceDict[nameOfDevice].Count;

            if (numberOfWorkOnDevice == 0)
            {
                jobInfor.startDate = firstDateStart;

                if(jobInfor.releaseDate > jobInfor.startDate)
                {
                    jobInfor.startDate = jobInfor.releaseDate;
                }

                TimeSpan executionTime = TimeSpan.FromMinutes(jobInfor.processingTime);
                jobInfor.endDate = jobInfor.startDate.Add(executionTime);
            }
            else
            {
                JobInfor jobPreviousInfor = productionDeviceDict[nameOfDevice][numberOfWorkOnDevice - 1];
                jobInfor.startDate = jobPreviousInfor.endDate;
                if (jobInfor.mold != jobPreviousInfor.mold)
                {
                    TimeSpan conversionTime = TimeSpan.FromMinutes(3*60);
                    jobInfor.startDate = jobInfor.startDate.Add(conversionTime);
                }
                
                if (jobInfor.releaseDate > jobInfor.startDate)
                {
                    jobInfor.startDate = jobInfor.releaseDate;
                }

                TimeSpan executionTime = TimeSpan.FromMinutes(jobInfor.processingTime);
                jobInfor.endDate = jobInfor.startDate.Add(executionTime);
            }
            return jobInfor;
        }

        public static JobInfor findAnotherWorkCenter(JobInfor jobInfor, Dictionary<string, List<JobInfor>> productionDeviceDict)
        {
            List<string> listAlternativeWorkCenter = jobInfor.alternativeWorkCenters;

            if (listAlternativeWorkCenter.Count > 0)
            {
                List<JobInfor> listJobConversion = new List<JobInfor>();
                foreach (string workCenterCheck in productionDeviceDict.Keys)
                {
                    int numberOnWorkCenter = productionDeviceDict[workCenterCheck].Count;
                    bool check = false;
                    foreach (string workCenterAvailable in listAlternativeWorkCenter)
                    {
                        if (workCenterAvailable == workCenterCheck)
                        {
                            if (numberOnWorkCenter > 1)
                            {
                                listJobConversion.Add(productionDeviceDict[workCenterCheck][numberOnWorkCenter - 1]);
                                check = true;
                                break;
                            }
                            else
                            {
                                listJobConversion.Add(productionDeviceDict[workCenterCheck][0]);
                                check = true;
                                break;
                            }
                            
                        }
                    }

                    if (check)
                    {
                        break;
                    }
                }

                DateTime minDateFinish = listJobConversion[0].endDate;
                for (int i = 0; i < listJobConversion.Count; i++)
                {
                    if (listJobConversion[i].endDate < minDateFinish)
                    {
                        minDateFinish = listJobConversion[i].endDate;
                        jobInfor.workCenter = listJobConversion[i].workCenter;
                        jobInfor.startDate = listJobConversion[i].endDate;
                        TimeSpan executionTime = TimeSpan.FromMinutes(jobInfor.processingTime * 60);
                        jobInfor.endDate = jobInfor.startDate.Add(executionTime);
                    }
                }
            }

            return jobInfor;
        }


        public static JobInfor changePlannedDate(List<JobInfor> listJobInfor, JobInfor jobInfor, DateTime firstDateStart, Dictionary<string, List<JobInfor>> productionDeviceDict)
        {
            List<string> listMoldAvailable = new List<string> { "K032-1", "K037-1", "K096-1" };
            jobInfor = findPlannedDate(jobInfor, firstDateStart, productionDeviceDict);
            if (jobInfor.endDate > jobInfor.dueDate)
            {
                bool checkMold = false;
                foreach(string moldAvailable in listMoldAvailable)
                {
                    if (jobInfor.mold == moldAvailable)
                    {
                        checkMold = true;
                    }
                }

                if (checkMold)
                {
                    List<JobInfor> listJobDuplicateMold = new List<JobInfor>();
                    for (int i = 0; i < jobInfor.id - 1; i++)
                    {
                        if(jobInfor.mold == listJobInfor[i].mold)
                        {
                            listJobDuplicateMold.Add(listJobInfor[i]);
                        }
                    }

                    int numberOfJobDuplicate = listJobDuplicateMold.Count;
                    if (numberOfJobDuplicate < 1)
                    {
                        jobInfor = findAnotherWorkCenter(jobInfor, productionDeviceDict);
                        jobInfor = findPlannedDate(jobInfor, firstDateStart, productionDeviceDict);
                    }
                    else
                    {
                        if (listJobDuplicateMold[numberOfJobDuplicate - 1].workCenter == listJobDuplicateMold[numberOfJobDuplicate - 2].workCenter)
                        {
                            jobInfor = findAnotherWorkCenter(jobInfor, productionDeviceDict);
                            jobInfor = findPlannedDate(jobInfor, firstDateStart, productionDeviceDict);
                        }
                        else
                        {
                            List<JobInfor> listJob1 = new List<JobInfor>();
                            List<JobInfor> listJob2 = new List<JobInfor>();
                            foreach (JobInfor work in listJobInfor)
                            {
                                if (work.workCenter == listJobDuplicateMold[numberOfJobDuplicate - 1].workCenter)
                                {
                                    listJob1.Add(work);
                                    break;
                                }

                                if (work.workCenter == listJobDuplicateMold[numberOfJobDuplicate - 2].workCenter)
                                {
                                    listJob2.Add(work);
                                    break;
                                }
                            }

                            if (listJob1.LastOrDefault().endDate < listJob2.LastOrDefault().endDate)
                            {
                                jobInfor.workCenter = listJob1.LastOrDefault().workCenter;
                            }
                            else
                            {
                                jobInfor.workCenter = listJob2.LastOrDefault().workCenter;
                            }
                            jobInfor = findPlannedDate(jobInfor, firstDateStart, productionDeviceDict);
                        }
                    }
                }
            }

            return jobInfor;
        }
    }
}


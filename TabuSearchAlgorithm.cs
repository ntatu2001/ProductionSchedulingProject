using System.Data;

namespace ProductionSchedulingProject
{
    public class TabuSearch
    {
        /// <summary>
        /// Determine the length of Tabu List
        /// </summary>
        /// <param name="bestListWorkInfor"></param>
        /// <returns></returns>
        public static int getTenure(List<JobInfor> bestListWorkInfor)
        {
            int numberOfWorks = bestListWorkInfor.Count;
            //if (numberOfWorks < 10)
            //{
            //    return 2;
            //}
            //else if (numberOfWorks < 20)
            //{
            //    return 5;
            //}
            //else
            //{
            //    return 31;
            //}
            return 25;
        }


        /// <summary>
        /// Initialize the solution in the order 1, 2, 3,..., workAvailableTable.Rows.Count
        /// </summary>
        /// <param name="workTable"></param>
        /// <returns></returns>
        public static List<int> initialSolution(List<JobInfor> bestListWorkInfor)
        {
            List<int> solution = new List<int>();
            foreach (JobInfor work in bestListWorkInfor)
            {
                solution.Add(work.id);
            }

            return solution;
        }


        /// <summary>
        /// Calculate the value of the objective function, the objective function is the sum of the early/delay times of the maintenance orders
        /// The Value of Object Function performed by minutes
        /// </summary>
        /// <param name="solution"></param>
        /// <param name="workTable"></param>
        /// <returns></returns>
        public static double objectValue(List<int> solution, DataTable workTable, List<JobInfor> bestListWorkInfor, Dictionary<string, List<JobInfor>> productionDeviceDict)
        {
            double objectValue = 0;
            
            foreach(string device in productionDeviceDict.Keys)
            {
                productionDeviceDict[device] = new List<JobInfor>();
            }

            foreach (int job in solution)
            {
                //Console.WriteLine(job);
                JobInfor workInfor = new JobInfor();

                foreach(JobInfor workTemp in bestListWorkInfor)
                {
                    if (workTemp.id == job)
                    {
                        workInfor = workTemp;
                    }
                }

                var firstDateStart = DateTime.ParseExact("01/02/2023 07:00", "dd/MM/yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture);

                workInfor = FindPlannedDate.changePlannedDate(bestListWorkInfor, workInfor, firstDateStart, productionDeviceDict);

                //Console.WriteLine($"The start date of job {workInfor.id} on device {workInfor.workCenter} is {workInfor.startDate} - {workInfor.endDate}");
                //double differenceMinutes = 0;
                //bool checkLate = false;
                DateTime dueDate = workInfor.dueDate;
                //if (workInfor.endDate >= workInfor.dueDate)
                //{
                //    // The unit of value returned from Ticks property is 10^-7 ticks/second
                //    differenceMinutes = TimeSpan.FromTicks((workInfor.endDate - workInfor.dueDate).Ticks).TotalMinutes;
                //    differenceMinutes = Math.Round(differenceMinutes, 0);
                //    //Console.WriteLine($"The delay time in each job = {differenceMinutes}");
                //    checkLate = true;
                //}
                //else if (workInfor.endDate < workInfor.dueDate)
                //{
                //    differenceMinutes = TimeSpan.FromTicks((workInfor.dueDate - workInfor.endDate).Ticks).TotalMinutes;

                //    differenceMinutes = Math.Round(differenceMinutes, 0);
                //    //Console.WriteLine($"The early time in each job = {differenceMinutes}");
                //}

                //if (differenceMinutes != 0)
                //{
                //    objectValue += differenceMinutes;
                //}

                if (workInfor.endDate > dueDate)
                {
                    //Console.WriteLine($"The priority of {workInfor.id} is {workInfor.priority}");
                    objectValue += workInfor.priority;
                }
            }

            return objectValue;
        }


        /// <summary>
        /// Create a frame consisting of: Conversion Pair and Objective Function Value
        /// </summary>
        /// <param name="solution"></param>
        /// <returns></returns>
        public static Dictionary<List<int>, double> tabuStructure(List<int> solution)
        {
            //tabuAttribute(conversion pair, move value)
            Dictionary<List<int>, double> tabuAttribute = new Dictionary<List<int>, double>();

            foreach (int i in solution)
            {
                if (i < solution.Count)
                {
                    List<int> listTemp = new List<int> { i, i + 1 };
                    tabuAttribute.Add(listTemp, 0);
                }
            }

            //foreach (var kvp in tabuAttribute)
            //{
            //    Console.WriteLine(kvp.Key[0].ToString() + " - " + kvp.Key[1].ToString());
            //    Console.WriteLine(kvp.Value);
            //}

            return tabuAttribute;
        }


        /// <summary>
        /// Takes a list (solution) returns a new neighbor solution with i, j swapped
        /// </summary>
        /// <param name="solution"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public static List<int> swapPairs(List<int> solution, int i, int j)
        {
            int iIndex = solution.IndexOf(i);
            int jIndex = solution.IndexOf(j);
            int temp = solution[iIndex];
            solution[iIndex] = solution[jIndex];
            solution[jIndex] = temp;
            return solution;
        }


        /// <summary>
        /// Check: Have the considerd pair existed in Tabu List? 
        /// If it existed, return true. Otherwise, return false
        /// </summary>
        /// <param name="bestPair"></param>
        /// <param name="tabuList"></param>
        /// <returns></returns>
        public static bool checkTabuList(List<int> bestPair, List<List<int>> tabuList)
        {
            for (int i = 0; i < tabuList.Count; i++)
            {
                if ((bestPair[0] == tabuList[i][0]) && (bestPair[1] == tabuList[i][1]))
                {
                    return true;
                }
            }
            return false;
        }

        public static List<List<int>> updateTabuList(List<int> bestMove, List<List<int>> tabuList, int tenure)
        {
            if (tabuList.Count < tenure)
            {
                tabuList.Add(bestMove);
            }
            else
            {
                for (int i = 0; i < (tabuList.Count - 1); i++)
                {
                    tabuList[i] = tabuList[i + 1];
                }
                tabuList[tabuList.Count - 1] = bestMove;
            }

            return tabuList;
        }


        /// <summary>
        /// Returns the swap pair have minimum objective function value in each iteration. 
        /// In addition, check: Have this pair already existed in the Tabu List? If not, add it to Tabu List!
        /// </summary>
        /// <param name="tabuAttribute"></param>
        /// <param name="tabuList"></param>
        /// <returns></returns>
        public static List<int> getBestPair(Dictionary<List<int>, double> tabuAttribute, List<List<int>> tabuList, int tenure)
        {
            //Find the minimum Move Value in Tabu Attribute dictionary
            var keyMinMoveValue = tabuAttribute.MinBy(item => item.Value).Key;
            double minMoveValue = tabuAttribute[keyMinMoveValue];

            var listKey = tabuAttribute.Keys.ToList();
            var listValue = tabuAttribute.Values.ToList();

            //There can be many pairs with the same minimum value
            List<List<int>> listKeyMinValue = new List<List<int>>();
            List<int> bestPair = new List<int>();
            foreach (List<int> key in listKey)
            {
                if (tabuAttribute[key] == minMoveValue)
                {
                    listKeyMinValue.Add(key);
                }
            }

            //Console.WriteLine($"The number of Pair have the same minimum Object Value: {listKeyMinValue.Count}");

            for (int index = 0; index < listKeyMinValue.Count; index++)
            {
                bestPair = listKeyMinValue[index];
                //Console.WriteLine(bestPair[0].ToString() + " - " + bestPair[1].ToString());
                if (checkTabuList(bestPair, tabuList) == false)
                {
                    //tabuList.Add(bestPair);
                    tabuList = updateTabuList(bestPair, tabuList, tenure);
                    break;
                }
            }
            return bestPair;
        }

        /// <summary>
        /// The implementation Tabu Search algorithm with short-term memory and pair swap as Tabu attribute
        /// </summary>
        /// <param name="workTable"></param>
        /// <returns></returns>
        public static List<JobInfor> tabuSearch(DataTable workTable, DataTable workCenterTable,
                                           DataTable productTable, DataTable partnerTable,
                                           DataTable bomTable)
        {
            List<JobInfor> bestListWorkInfor = GetData.dataDatePlannedStart(bomTable, partnerTable, productTable, workCenterTable, workTable);

            int tenure = getTenure(bestListWorkInfor);
            Console.WriteLine($"The Tenure: {tenure}");
            List<List<int>> tabuList = new List<List<int>>();
            List<int> listBestPair = new List<int>();

            List<int> currentSolution = initialSolution(bestListWorkInfor);
            for (int i = 0; i < currentSolution.Count; i++)
            {
                bestListWorkInfor[i].id = currentSolution[i];
            }

            Dictionary<string, List<JobInfor>> productionDeviceDict = ReadDataFromCSV.deviceStructure(workCenterTable);
            double bestObjectValue = objectValue(currentSolution, workTable, bestListWorkInfor, productionDeviceDict);
            List<int> bestSolution = currentSolution;

            int iterations = 50;
            int terminate = 0;
            var listObjectValue = new List<double>();
            while (terminate < iterations)
            {
                // Searching the whole neighborhood of the current solution
                Dictionary<List<int>, double> dictTabuAttribute = tabuStructure(bestSolution);

                //Calculate the objective function value corresponding to each swap pair in TabuAttribute
                var listKey = dictTabuAttribute.Keys.ToList();

                foreach (List<int> key in listKey)
                {
                    List<int> candidateSolution = swapPairs(bestSolution, key[0], key[1]);
                    Console.WriteLine("Print candidate solution");
                    foreach (int item in candidateSolution)
                    {
                        Console.Write(item + " - ");
                    }

                    double candidateObjectValue = objectValue(candidateSolution, workTable, bestListWorkInfor, productionDeviceDict);
                    Console.WriteLine(key[0].ToString() + " - " + key[1].ToString());
                    Console.WriteLine(candidateObjectValue);
                    dictTabuAttribute[key] = candidateObjectValue;
                }

                //Select the move with the lowest ObjValue in the neighborhood (minimization)              
                listBestPair = getBestPair(dictTabuAttribute, tabuList, tenure);
                Console.WriteLine("---------------------------------------------------------");
                Console.WriteLine($"The size of ListBestPair = {listBestPair.Count}");
                Console.WriteLine($"The Best Pair : {listBestPair[0]} - {listBestPair[1]}");
                Console.WriteLine("The elements in Tabu List");

                for (int i = 0; i < tabuList.Count; i++)
                {
                    Console.Write(tabuList[i][0].ToString() + " - " + tabuList[i][1].ToString() + "||");
                }
                Console.WriteLine();
                Console.WriteLine("---------------------------------------------------------");


                if (listBestPair.Count > 0)
                {
                    currentSolution = swapPairs(bestSolution, listBestPair[0], listBestPair[1]);
                    double currentObjectValue = objectValue(currentSolution, workTable, bestListWorkInfor, productionDeviceDict);

                    if (currentObjectValue < bestObjectValue)
                    {
                        bestSolution = currentSolution;
                        bestObjectValue = currentObjectValue;
                    }
                    Console.WriteLine($"The Best Pair in terminate {terminate}: {listBestPair[0]} - {listBestPair[1]}");
                    Console.WriteLine($"The Object Value for Best Pair in terminate {terminate}: {bestObjectValue}");
                    listObjectValue.Add(bestObjectValue);
                }

                terminate += 1;
            }

            Console.WriteLine("The best Solution with the minimum Object Value");
            List<JobInfor> newListJobInfor = new List<JobInfor>();
            foreach (int item in bestSolution)
            {
                Console.Write(item + " - ");
                foreach(JobInfor workInfor in bestListWorkInfor)
                {
                    if (item == workInfor.id)
                    {
                        newListJobInfor.Add(workInfor);
                        break;
                    }
                }
            }
            Console.WriteLine();
            Console.WriteLine(bestObjectValue);


            Console.WriteLine();
            Console.WriteLine("Print planned date for all job in Timeline");
            foreach (JobInfor workInfor in bestListWorkInfor)
            {
                Console.WriteLine($"The job: {workInfor.id}. Planned Date: {workInfor.startDate} - {workInfor.endDate}");
            }


            foreach (string device in productionDeviceDict.Keys)
            {
                Console.WriteLine(device);
                foreach (JobInfor item in productionDeviceDict[device])
                {
                    Console.WriteLine(item.startDate);
                }
                Console.WriteLine("-------------------");
                foreach (JobInfor item in productionDeviceDict[device])
                {
                    Console.WriteLine(item.endDate);
                }
            }
            //foreach (double item in listObjectValue)
            //{
            //    Console.WriteLine(item);
            //}
            return newListJobInfor;
        }


        /// <summary>
        /// Returns a DataTable of Work after scheduling
        /// </summary>
        /// <param name="workTable"></param>
        /// <returns></returns>
        public static DataTable returnScheduledDataTable(DataTable workTable, DataTable workCenterTable, 
                                                         DataTable productTable, DataTable partnerTable, 
                                                         DataTable bomTable)
        {
            //Find the order of work such that the objective function value is minimal
            List<JobInfor> bestSolution = tabuSearch(workTable, workCenterTable, productTable, partnerTable, bomTable);

            //Create a new data table that includes the jobs sorted by bestSolution
            DataTable scheduledWorkTable = new DataTable();
            return scheduledWorkTable;
        }
    }
}

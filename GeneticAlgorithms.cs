using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using GAF;
using GAF.Extensions;
using GAF.Operators;
using System.Threading.Tasks;

namespace ProductionSchedulingProject
{
    public class GeneticAlgorithms
    {
        public static Population createChromosomes(int maxGeneration, int populationSize)
        {
            var population = new Population();
            for (var p = 0; p < maxGeneration; p++)
            {
                var chromosome = new Chromosome();
                for (var g = 1; g <= populationSize; g++)
                {
                    chromosome.Genes.Add(new Gene(g));
                }
                chromosome.Genes.ShuffleFast();
                population.Solutions.Add(chromosome);
            }
            return population;
        }

        public static double calculateObjectValue(Chromosome chromosome)
        {
            double fitnessValue = 0;
            List<int> solution = new List<int>();
            foreach (Gene gene in chromosome.Genes.ToList())
            {
                solution.Add((int)gene.RealValue);
            }

            DataTable bomTable = ReadDataFromCSV.ReadDataFromBOM();
            DataTable partnerTable = ReadDataFromCSV.ReadDataFromPartners();
            DataTable productTable = ReadDataFromCSV.ReadDataFromProducts();
            DataTable workCenterTable = ReadDataFromCSV.ReadDataFromWorkCenters();
            DataTable orderTable = ReadDataFromCSV.ReadDataFromOrders();

            List<JobInfor> bestListWorkInfor = GetData.dataDatePlannedStart(bomTable, partnerTable, productTable, workCenterTable, orderTable);

            Dictionary<string, List<JobInfor>> productionDeviceDict = ReadDataFromCSV.deviceStructure(workCenterTable);

            double totalPriority = 0;
            foreach (int job in solution)
            {
                //Console.WriteLine($"The gene real value: {(int)gene.RealValue}");
                JobInfor workInfor = new JobInfor();

                foreach (JobInfor workTemp in bestListWorkInfor)
                {
                    if (workTemp.id == job)
                    {
                        workInfor = workTemp;
                    }
                }
                //Console.WriteLine(job - 1);
                var firstDateStart = DateTime.ParseExact("01/02/2023 07:00", "dd/MM/yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture);

                workInfor = FindPlannedDate.changePlannedDate(bestListWorkInfor, workInfor, firstDateStart, productionDeviceDict);

                //if ((job - 1) == 0)
                //{
                //    workInfor = FindPlannedDate.changePlannedDate(bestListWorkInfor, workInfor, firstDateStart, productionDeviceDict);
                //}
                //else
                //{
                //    workInfor = FindPlannedDate.changePlannedDate(bestListWorkInfor, workInfor, firstDateStart, productionDeviceDict);
                //}

                //if ((job - 1) == 0)
                //{
                //    workInfor.startDate = firstDateStart;
                //    TimeSpan executionTime = TimeSpan.FromMinutes(workInfor.processingTime);
                //    workInfor.endDate = workInfor.startDate.Add(executionTime);
                //}
                //else
                //{
                //    TimeSpan executionTime = TimeSpan.FromMinutes(workInfor.processingTime);
                //    workInfor.endDate = workInfor.startDate.Add(executionTime);
                //}

                DateTime dueDate = workInfor.dueDate;

                if (workInfor.endDate > dueDate)
                {
                    //Console.WriteLine($"The priority of {workInfor.id} is {workInfor.priority}");
                    fitnessValue += workInfor.priority;
                }

                totalPriority += workInfor.priority;
            }

            //Console.WriteLine($"Total Priority: {totalPriority}");
            //Console.WriteLine("------------------------");
            return fitnessValue;
        }

        public static double calculateFitness(Chromosome chromosome)

        {
            double fitnessValue = calculateObjectValue(chromosome);
            //Console.WriteLine($"fitness value: {fitnessValue}");
            double fitness = 1 - fitnessValue / 396.8;
            //Console.WriteLine($"fitness: {fitness}");
            return fitness;
        }

        public static bool Terminate(Population population, int currentGeneration, long currentEvaluation)
        {
            return currentGeneration > 100;
        }

        public static void createGA(DataTable orderTable)
        {
            int numberOfWorks = orderTable.Rows.Count;
            int maxGeneration = 100;
            int populationSize = numberOfWorks;

            Population population = createChromosomes(maxGeneration, populationSize);


            int elitism = 15;
            float crossoverProbability = 0.9f;
            float mutationProbability = 0.05f;

            var elite = new Elite(elitism);

            var crossover = new Crossover(crossoverProbability)
            {
                CrossoverType = CrossoverType.DoublePointOrdered
            };

            var mutate = new SwapMutate(mutationProbability);
            var ga = new GeneticAlgorithm(population, calculateFitness);

            ga.OnGenerationComplete += ga_OnGenerationComplete;
            ga.OnRunComplete += ga_OnRunComplete;
            ga.Operators.Add(elite);
            ga.Operators.Add(crossover);
            ga.Operators.Add(mutate);

            ga.Run(Terminate);
        }

        static void ga_OnRunComplete(object sender, GaEventArgs e)
        {
            var fittest = e.Population.GetTop(1)[0];
            Console.WriteLine($"The scheduled list of work: ");

            foreach (var gene in fittest.Genes)
            {
                Console.WriteLine($"The no of work: {gene.RealValue}");
            }
        }

        private static void ga_OnGenerationComplete(object sender, GaEventArgs e)
        {
            var fittest = e.Population.GetTop(1)[0];
            var listChromosome = e.Population.GetTop(10);

            foreach (Chromosome chromosome in listChromosome)
            {
                foreach (var gene in chromosome.Genes)
                {
                    Console.Write(gene.RealValue + " - ");
                }
                Console.WriteLine();
                Console.WriteLine($"The fitness value of chromosome: {chromosome.Fitness}");
                Console.WriteLine($"The object value of chromosome: {calculateObjectValue(chromosome)}");
            }

            var objectValue = calculateObjectValue(fittest);
            Console.WriteLine("Generation: {0}, Fitness: {1}, Object Value: {2}", e.Generation, fittest.Fitness, objectValue);
            Console.WriteLine("------------------------------------------------");
            Console.WriteLine($"The generation: {e.Generation}");
            Console.WriteLine($"The fitness value of generation: {fittest.Fitness}");
            Console.WriteLine($"The object value of generation: {objectValue}");
        }
    }
}

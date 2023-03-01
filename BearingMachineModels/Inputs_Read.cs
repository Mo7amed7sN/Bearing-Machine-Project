using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BearingMachineModels
{
    public class Inputs_Read
    {
        private SimulationSystem system;

        public Inputs_Read() { system = new SimulationSystem(); }

        public SimulationSystem Read_fromFile()
        {
            FileStream fs = new FileStream("TestCase1.txt", FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            string s = ""; decimal acc = 0; int prev_min = 0;
            string[] spliter = new string[2];

            string w = "";
            while ((w = sr.ReadLine()) != null)
            {
                if (w == "") continue;

                if (w == "DowntimeCost")
                    system.DowntimeCost = int.Parse(sr.ReadLine());
                else if (w == "RepairPersonCost")
                    system.RepairPersonCost = int.Parse(sr.ReadLine());
                else if (w == "BearingCost")
                    system.BearingCost = int.Parse(sr.ReadLine());
                else if (w == "NumberOfHours")
                    system.NumberOfHours = int.Parse(sr.ReadLine());
                else if (w == "NumberOfBearings")
                    system.NumberOfBearings = int.Parse(sr.ReadLine());
                else if (w == "RepairTimeForOneBearing")
                    system.RepairTimeForOneBearing = int.Parse(sr.ReadLine());
                else if (w == "RepairTimeForAllBearings")
                    system.RepairTimeForAllBearings = int.Parse(sr.ReadLine());
                else if (w == "DelayTimeDistribution")
                {
                    s = ""; acc = 0; prev_min = 0;
                    while ((s = sr.ReadLine()) != "")
                    {
                        spliter = s.Split(',');
                        TimeDistribution dist = new TimeDistribution();

                        dist.Time = int.Parse(spliter[0]);
                        dist.Probability = decimal.Parse(spliter[1]);

                        dist.CummProbability = (decimal)(acc + dist.Probability);

                        dist.MinRange = prev_min + 1;
                        dist.MaxRange = (int)(dist.CummProbability * 100);

                        prev_min = dist.MaxRange;
                        acc = dist.CummProbability;

                        system.DelayTimeDistribution.Add(dist);
                    }
                }
                else
                {
                    s = ""; acc = 0; prev_min = 0;
                    while ((s = sr.ReadLine()) != null)
                    {
                        spliter = s.Split(',');
                        TimeDistribution dist = new TimeDistribution();

                        dist.Time = int.Parse(spliter[0]);
                        dist.Probability = decimal.Parse(spliter[1]);

                        dist.CummProbability = (decimal)(acc + dist.Probability);

                        dist.MinRange = prev_min + 1;
                        dist.MaxRange = (int)(dist.CummProbability * 100);

                        acc = dist.CummProbability;
                        prev_min = dist.MaxRange;

                        system.BearingLifeDistribution.Add(dist);
                    }
                    break;
                }
            }
            sr.Close();
            fs.Close();
            return system;
        }
    }
}

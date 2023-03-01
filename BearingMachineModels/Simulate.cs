using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BearingMachineModels
{
    public class Simulate
    {
        private SimulationSystem system;
        private List<CurrentSimulationCase> cur;
        private List<ProposedSimulationCase> pro;

        public Simulate(SimulationSystem sy)
        {
            system = sy;
            cur = new List<CurrentSimulationCase>();
            pro = new List<ProposedSimulationCase>();
        }

        Random R = new Random();
        int rand, Acc_Hours;
        Dictionary<int, List<Bearing>> Bear_Info = new Dictionary<int, List<Bearing>>();
        int total_current_delay = 0, total_proposed_delay = 0;
        int ind = 0;
        int[] cnt;
        public List<CurrentSimulationCase> current_table()
        {
            cnt = new int[system.NumberOfBearings];
            for (int i = 0; i < system.NumberOfBearings; i++)
                cnt[i] = 0;

            for (int i = 1; i <= system.NumberOfBearings; i++)
            {
                Acc_Hours = 0;
                while (Acc_Hours < system.NumberOfHours)
                {
                    cnt[i - 1]++;
                    Bearing B = new Bearing();
                    CurrentSimulationCase cs = new CurrentSimulationCase();
                    B.Index = i;
                    rand = R.Next(1, 101);
                    B.RandomHours = rand;
                    for (int j = 0; j < system.BearingLifeDistribution.Count; j++) 
                    {
                        if (system.BearingLifeDistribution[j].MinRange <= rand && system.BearingLifeDistribution[j].MaxRange >= rand)
                        {
                            B.Hours = system.BearingLifeDistribution[j].Time;
                            break;
                        }
                    }

                    try
                    {
                        Bear_Info[i].Add(B);
                    }
                    catch(Exception ex)
                    {
                        Bear_Info[i] = new List<Bearing>();
                        Bear_Info[i].Add(B);
                    }

                    cs.Bearing = B;
                    cs.AccumulatedHours = Acc_Hours + B.Hours;
                    rand = R.Next(1, 101);
                    cs.RandomDelay = rand;
                    for(int j = 0; j < system.DelayTimeDistribution.Count; j++)
                    {
                        if(system.DelayTimeDistribution[j].MinRange <= rand && system.DelayTimeDistribution[j].MaxRange >= rand)
                        {
                            cs.Delay = system.DelayTimeDistribution[j].Time;
                            break;
                        }
                    }

                    total_current_delay += cs.Delay;
                    Acc_Hours = cs.AccumulatedHours;
                    cur.Add(cs);
                }
            }
            return cur;
        }

        public List<ProposedSimulationCase> proposed_table()
        {
            Acc_Hours = 0; ind = 0;
            while(Acc_Hours < system.NumberOfHours)
            {
                ProposedSimulationCase cs = new ProposedSimulationCase();

                int first_fail = int.MaxValue;
                List<Bearing> bears = new List<Bearing>();
                for(int i = 1; i <= system.NumberOfBearings; i++)
                {
                    try
                    {
                        bears.Add(Bear_Info[i][ind]);
                        if (Bear_Info[i][ind].Hours < first_fail)
                            first_fail = Bear_Info[i][ind].Hours;
                        
                    }
                    catch(Exception ex)
                    {
                        Bearing nb = new Bearing();

                        nb.Index = i;
                        rand = R.Next(1, 101);
                        nb.RandomHours = rand;
                        for(int j = 0; j < system.BearingLifeDistribution.Count; j++)
                        {
                            if (system.BearingLifeDistribution[j].MinRange <= rand && system.BearingLifeDistribution[j].MaxRange >= rand)
                            {
                                nb.Hours = system.BearingLifeDistribution[j].Time;
                                break;
                            }
                        }

                        bears.Add(nb);
                        if (nb.Hours < first_fail)
                            first_fail = nb.Hours;
                    }
                }
                cs.Bearings = bears;
                cs.FirstFailure = first_fail;
                cs.AccumulatedHours = Acc_Hours + first_fail;
                rand = R.Next(1, 101);
                cs.RandomDelay = rand;
                for (int j = 0; j < system.DelayTimeDistribution.Count; j++)
                {
                    if (system.DelayTimeDistribution[j].MinRange <= rand && system.DelayTimeDistribution[j].MaxRange >= rand)
                    {
                        cs.Delay = system.DelayTimeDistribution[j].Time;
                        break;
                    }
                }

                Acc_Hours = cs.AccumulatedHours;
                ind++;
                total_proposed_delay += cs.Delay;

                pro.Add(cs);
            }
            return pro;
        }

        public PerformanceMeasures current_performance()
        {
            PerformanceMeasures p = new PerformanceMeasures();

            int sum = 0;
            for (int i = 0; i < system.NumberOfBearings; i++)
                sum += cnt[i];

            p.BearingCost = (decimal)(sum * system.BearingCost);
            p.DelayCost = (decimal)(total_current_delay * system.DowntimeCost);
            p.DowntimeCost = (decimal)(sum * system.RepairTimeForOneBearing * system.DowntimeCost);
            p.RepairPersonCost = (decimal)(sum * system.RepairTimeForOneBearing * system.RepairPersonCost);
            p.RepairPersonCost = (decimal)(p.RepairPersonCost / 60);
            p.TotalCost = (decimal)(p.BearingCost + p.DelayCost + p.DowntimeCost + p.RepairPersonCost);

            return p;
        }

        public PerformanceMeasures proposed_performance()
        {
            PerformanceMeasures p = new PerformanceMeasures();

            p.BearingCost = (decimal)((ind * system.NumberOfBearings)* system.BearingCost);
            p.DelayCost = (decimal)(total_proposed_delay * system.DowntimeCost);
            p.DowntimeCost = (decimal)(ind * system.RepairTimeForAllBearings * system.DowntimeCost);
            p.RepairPersonCost = (decimal)(ind * system.RepairTimeForAllBearings * system.RepairPersonCost);
            p.RepairPersonCost = (decimal)(p.RepairPersonCost / 60);
            p.TotalCost = (decimal)(p.BearingCost + p.DelayCost + p.DowntimeCost + p.RepairPersonCost);

            return p;
        }

        public decimal get_delc() { return (decimal)(total_current_delay); }
        public decimal get_delp() { return (decimal)(total_proposed_delay); }
    }
}

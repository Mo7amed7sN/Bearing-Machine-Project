using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BearingMachineModels;
using BearingMachineTesting;

namespace BearingMachineSimulation
{
    public partial class Bearing_Problem : Form
    {
        public Bearing_Problem()
        {
            InitializeComponent();
        }

        SimulationSystem sys;
        private void Bearing_Problem_Load(object sender, EventArgs e)
        {
            sys = new SimulationSystem();
        }

        private void rd_data_Click(object sender, EventArgs e)
        {
            Inputs_Read File_Obj = new Inputs_Read();

            sys = File_Obj.Read_fromFile();

            MessageBox.Show("Done");
        }

        Simulate sim;
        private void run_program_Click(object sender, EventArgs e)
        {
            sim = new Simulate(sys);

            sys.CurrentSimulationTable = sim.current_table();
            sys.ProposedSimulationTable = sim.proposed_table();
            sys.CurrentPerformanceMeasures = sim.current_performance();
            sys.ProposedPerformanceMeasures = sim.proposed_performance();

            MessageBox.Show("Done");
        }

        private void show_outputs_Click(object sender, EventArgs e)
        {
            c_bearc.Text = sys.CurrentPerformanceMeasures.BearingCost.ToString();
            c_delayc.Text = sys.CurrentPerformanceMeasures.DelayCost.ToString();
            c_downtimec.Text = sys.CurrentPerformanceMeasures.DowntimeCost.ToString();
            c_repairc.Text = sys.CurrentPerformanceMeasures.RepairPersonCost.ToString();
            c_totc.Text = sys.CurrentPerformanceMeasures.TotalCost.ToString();
            p_bearc.Text = sys.ProposedPerformanceMeasures.BearingCost.ToString();
            p_delayc.Text = sys.ProposedPerformanceMeasures.DelayCost.ToString();
            p_dowmtimec.Text = sys.ProposedPerformanceMeasures.DowntimeCost.ToString();
            p_repairc.Text = sys.ProposedPerformanceMeasures.RepairPersonCost.ToString();
            p_totc.Text = sys.ProposedPerformanceMeasures.TotalCost.ToString();
            tot_delayc.Text = sim.get_delc().ToString();
            tot_delayp.Text = sim.get_delp().ToString();

            DataTable dt = new DataTable();
            dt.Columns.Add("Index"); dt.Columns.Add("Random Hours"); dt.Columns.Add("Hours");
            dt.Columns.Add("Accumulated Hours"); dt.Columns.Add("Random Delay"); dt.Columns.Add("Delay");
            for(int i = 0; i < sys.CurrentSimulationTable.Count; i++)
            {
                DataRow dr = dt.NewRow();
                dr["Index"] = sys.CurrentSimulationTable[i].Bearing.Index;
                dr["Random Hours"] = sys.CurrentSimulationTable[i].Bearing.RandomHours;
                dr["Hours"] = sys.CurrentSimulationTable[i].Bearing.Hours;
                dr["Accumulated Hours"] = sys.CurrentSimulationTable[i].AccumulatedHours;
                dr["Random Delay"] = sys.CurrentSimulationTable[i].RandomDelay;
                dr["Delay"] = sys.CurrentSimulationTable[i].Delay;

                dt.Rows.Add(dr);
            }
            cur_table.DataSource = dt;

            DataTable dt1 = new DataTable();
            for(int i = 0; i < sys.NumberOfBearings; i++)
            {
                dt1.Columns.Add("Bear " + ((i + 1).ToString()));
            }
            dt1.Columns.Add("First Failure"); dt1.Columns.Add("Random Delay"); dt1.Columns.Add("Delay");

            for(int i = 0; i <sys.ProposedSimulationTable.Count; i++)
            {
                DataRow dr1 = dt1.NewRow();
                for(int j = 0; j < sys.ProposedSimulationTable[i].Bearings.Count; j++)
                {
                    dr1["Bear " + ((j + 1).ToString())] = sys.ProposedSimulationTable[i].Bearings[j].Hours;
                }
                dr1["First Failure"] = sys.ProposedSimulationTable[i].FirstFailure;
                dr1["Random Delay"] = sys.ProposedSimulationTable[i].RandomDelay;
                dr1["Delay"] = sys.ProposedSimulationTable[i].Delay;

                dt1.Rows.Add(dr1);
            }
            pro_table.DataSource = dt1;

            string testing = TestingManager.Test(sys, Constants.FileNames.TestCase1);
            MessageBox.Show(testing);
        }
    }
}

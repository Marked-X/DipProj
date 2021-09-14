using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace DipProj
{
    public partial class Form1 : Form
    {
        Variable input1;
        Variable input2;
        Variable output;
        List<Rule> rules = new List<Rule>();
        public Form1()
        {      
            InitializeComponent();
            FillVariables();
            FillRules();
            FillTable();
            Chart_draw();
            dataGridView1.DoubleBuffered(true);
            this.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
        }
        private void FillVariables()
        {
            input1 = new Variable() { Id = 0,Name = "Temperature"};
            input2 = new Variable() { Id = 1,Name = "Delta Temperature"};
            output = new Variable() { Id = 2,Name = "Motor Speed"};

            input1.FuzzySets.Add("VC", new FuzzySet() { Variable = input1, Name = "Very Cold", MembershipFunction = MathCalc.S_func(1.0, 1.5) });
            input1.FuzzySets.Add("CD", new FuzzySet() { Variable = input1, Name = "Cold", MembershipFunction = MathCalc.Triangle(0.5, 1, 1.5) });
            input1.FuzzySets.Add("CL", new FuzzySet() { Variable = input1, Name = "Cool", MembershipFunction = MathCalc.Triangle(0, 0.5, 1) });
            input1.FuzzySets.Add("NR", new FuzzySet() { Variable = input1, Name = "Normal", MembershipFunction = MathCalc.Triangle(-0.5, 0, 0.5) });
            input1.FuzzySets.Add("LH", new FuzzySet() { Variable = input1, Name = "Less Hot", MembershipFunction = MathCalc.Triangle(-1, -0.5, 0) });
            input1.FuzzySets.Add("HT", new FuzzySet() { Variable = input1, Name = "Hot", MembershipFunction = MathCalc.Triangle(-1.5, -1, -0.5) });
            input1.FuzzySets.Add("VH", new FuzzySet() { Variable = input1, Name = "Very Hot", MembershipFunction = MathCalc.Z_func(-1.5, -1) });

            input2.FuzzySets.Add("PL", new FuzzySet() { Variable = input2, Name = "Positive Large", MembershipFunction = MathCalc.S_func(1, 1.5) });
            input2.FuzzySets.Add("PM", new FuzzySet() { Variable = input2, Name = "Positive Medium", MembershipFunction = MathCalc.Triangle(0.5, 1, 1.5) });
            input2.FuzzySets.Add("PS", new FuzzySet() { Variable = input2, Name = "Positive Small", MembershipFunction = MathCalc.Triangle(0, 0.5, 1) });
            input2.FuzzySets.Add("NU", new FuzzySet() { Variable = input2, Name = "Neutral", MembershipFunction = MathCalc.Triangle(-0.5, 0, 0.5) });
            input2.FuzzySets.Add("NS", new FuzzySet() { Variable = input2, Name = "Negative Small", MembershipFunction = MathCalc.Triangle(-1, -0.5, 0) });
            input2.FuzzySets.Add("NM", new FuzzySet() { Variable = input2, Name = "Negative Medium", MembershipFunction = MathCalc.Triangle(-1.5, -1, -0.5) });
            input2.FuzzySets.Add("NL", new FuzzySet() { Variable = input2, Name = "Negative Large", MembershipFunction = MathCalc.Z_func(-1.5, -1) });

            output.FuzzySets.Add("VS", new FuzzySet() { Variable = output, Name = "Very Slow", MembershipFunction = MathCalc.Z_func(15, 30) });
            output.FuzzySets.Add("SL", new FuzzySet() { Variable = output, Name = "Slow", MembershipFunction = MathCalc.Triangle(15, 30, 40) });
            output.FuzzySets.Add("LS", new FuzzySet() { Variable = output, Name = "Less Speed", MembershipFunction = MathCalc.Triangle(30, 40, 50) });
            output.FuzzySets.Add("NO", new FuzzySet() { Variable = output, Name = "Normal Speed", MembershipFunction = MathCalc.Triangle(40, 50, 60) });
            output.FuzzySets.Add("LF", new FuzzySet() { Variable = output, Name = "Less Fast", MembershipFunction = MathCalc.Triangle(50, 60, 70) });
            output.FuzzySets.Add("FT", new FuzzySet() { Variable = output, Name = "Fast", MembershipFunction = MathCalc.Triangle(60, 70, 85) });
            output.FuzzySets.Add("VF", new FuzzySet() { Variable = output, Name = "Very Fast", MembershipFunction = MathCalc.S_func(70, 85) });
        }
        private void FillRules()
        {
            string rulesText = "NL VC VS\nNL CD VS\nNL CL SL\nNL NR LS\nNL LH LS\nNL HT NO\nNL VH LF\nNM VC SL\nNM CD SL\nNM CL LS\nNM NR LS\nNM LH NO\nNM HT LF\nNM VH LF\nNS VC SL" +
                "\nNS CD LS\nNS CL LS\nNS NR NO\nNS LH NO\nNS HT LF\nNS VH LF\nNU VC LS\nNU CD LS\nNU CL NO\nNU NR NO\nNU LH LF\nNU HT LF\nNU VH FT\nPS VC LS\nPS CD NO\nPS CL LF" +
                "\nPS NR LF\nPS LH LF\nPS HT FT\nPS VH FT\nPM VC NO\nPM CD LF\nPM CL LF\nPM NR LF\nPM LH LF\nPM HT FT\nPM VH VF\nPL VC LF\nPL CD LF\nPL CL LF\nPL NR LF\nPL LH FT\nPL HT VF\nPL VH VF";

            foreach (var line in rulesText.Split('\n'))
            {
                Rule temp = new Rule();
                var FuzzySet = line.Split(' ');
                temp.Conditions.Add(input2.FuzzySets[FuzzySet[0]]);
                temp.Conditions.Add(input1.FuzzySets[FuzzySet[1]]);
                temp.Conclusion = output.FuzzySets[FuzzySet[2]];
                rules.Add(temp);
            };
        }
        private void FillTable()
        {
            for (int i = 0; i < 49; i++)
            {
                dataGridView1.Rows.Add();
                dataGridView1.Rows[i].Cells[0].Value = i + 1;
                dataGridView1.Rows[i].Cells[1].Value = "If";
                dataGridView1.Rows[i].Cells[2].Value = rules[i].Conditions[1].Name;
                dataGridView1.Rows[i].Cells[3].Value = "and";
                dataGridView1.Rows[i].Cells[4].Value = rules[i].Conditions[0].Name;
                dataGridView1.Rows[i].Cells[5].Value = "then";
                dataGridView1.Rows[i].Cells[6].Value = rules[i].Conclusion.Name;
            };
        }
        public void Chart_draw()
        {
            for(double i = -2; i < 2; i += 0.1)
            {
                chartTemp.Series[0].Points.AddXY(i, input1.FuzzySets["VC"].GetValue(i));
                chartTemp.Series[1].Points.AddXY(i, input1.FuzzySets["CD"].GetValue(i));
                chartTemp.Series[2].Points.AddXY(i, input1.FuzzySets["CL"].GetValue(i));
                chartTemp.Series[3].Points.AddXY(i, input1.FuzzySets["NR"].GetValue(i));
                chartTemp.Series[4].Points.AddXY(i, input1.FuzzySets["LH"].GetValue(i));
                chartTemp.Series[5].Points.AddXY(i, input1.FuzzySets["HT"].GetValue(i));
                chartTemp.Series[6].Points.AddXY(i, input1.FuzzySets["VH"].GetValue(i));
                    
                chartDiff.Series[0].Points.AddXY(i, input2.FuzzySets["PL"].GetValue(i));
                chartDiff.Series[1].Points.AddXY(i, input2.FuzzySets["PM"].GetValue(i));
                chartDiff.Series[2].Points.AddXY(i, input2.FuzzySets["PS"].GetValue(i));
                chartDiff.Series[3].Points.AddXY(i, input2.FuzzySets["NU"].GetValue(i));
                chartDiff.Series[4].Points.AddXY(i, input2.FuzzySets["NS"].GetValue(i));
                chartDiff.Series[5].Points.AddXY(i, input2.FuzzySets["NM"].GetValue(i));
                chartDiff.Series[6].Points.AddXY(i, input2.FuzzySets["NL"].GetValue(i));
            }
            for(int i = 0; i < 100; i++)
            {
                chartSpeed.Series[0].Points.AddXY(i, output.FuzzySets["VS"].GetValue(i));
                chartSpeed.Series[1].Points.AddXY(i, output.FuzzySets["SL"].GetValue(i));
                chartSpeed.Series[2].Points.AddXY(i, output.FuzzySets["LS"].GetValue(i));
                chartSpeed.Series[3].Points.AddXY(i, output.FuzzySets["NO"].GetValue(i));
                chartSpeed.Series[4].Points.AddXY(i, output.FuzzySets["LF"].GetValue(i));
                chartSpeed.Series[5].Points.AddXY(i, output.FuzzySets["FT"].GetValue(i));
                chartSpeed.Series[6].Points.AddXY(i, output.FuzzySets["VF"].GetValue(i));
            }
        }
        private void Calculate_Click(object sender, EventArgs e)
        {
            Chart chart = chartOne;
            double[] input = new double[2];
            input[0] = Double.Parse(textBoxTemp.Text, System.Globalization.CultureInfo.InvariantCulture);
            input[1] = Double.Parse(textBoxDiff.Text, System.Globalization.CultureInfo.InvariantCulture);
            double temp = MathCalc.Solve(rules, input, chart);
            textBoxMotorSpeed.Text = temp.ToString();
        }
    }
}

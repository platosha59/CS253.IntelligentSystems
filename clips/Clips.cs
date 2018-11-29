﻿using System;
using System.Text;
using System.Windows.Forms;

using CLIPSNET;

namespace ClipsFormsExample
{
    public partial class ClipsMatchmaking : Form
    {
        private CLIPSNET.Environment all_villains = new CLIPSNET.Environment();
        private CLIPSNET.Environment clips = new CLIPSNET.Environment();

        public ClipsMatchmaking()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            LoadAllVillains();
        }

        private void HandleResponse()
        {
            //  Вытаскиаваем факт из ЭС
            FactAddressValue fv = clips.FindFact("ioproxy");

            MultifieldValue damf = (MultifieldValue)fv["messages"];
            MultifieldValue vamf = (MultifieldValue)fv["answers"];

            textBox1.Text += "Новая итерация : " + System.Environment.NewLine;
            for (int i = 0; i < damf.Count; i++)
            {
                LexemeValue da = (LexemeValue)damf[i];
                byte[] bytes = Encoding.Default.GetBytes(da.Value);
                textBox1.Text += Encoding.UTF8.GetString(bytes) + System.Environment.NewLine;
            }

            if (vamf.Count > 0)
            {
                textBox1.Text += "----------------------------------------------------" + System.Environment.NewLine;
                for (int i = 0; i < damf.Count; i++)
                {

                    LexemeValue va = (LexemeValue)vamf[i];
                    textBox1.Text += va.Value + System.Environment.NewLine;
                }
            }

            clips.Eval("(assert (clearmessage))");
        }

        private void nextBtn_Click(object sender, EventArgs e)
        {
            clips.Run();
            HandleResponse();
        }

        private void resetBtn_Click(object sender, EventArgs e)
        {
            //clips.Clear();

            ///*string stroka = codeBox.Text;
            //System.IO.File.WriteAllText("tmp.clp", codeBox.Text);
            //clips.Load("tmp.clp");*/

            ////  Так тоже можно - без промежуточного вывода в файл
            ////clips.LoadFromString(codeBox.Text);
            //clips.LoadFromString(System.IO.File.ReadAllText("../../../rules.clp"));

            //clips.Reset();

            ////init_listbox();
            //textBox1.Text = "Выполнены команды Clear и Reset." + System.Environment.NewLine;
        }

        public void init_listbox()
        {
            all_villains.Run();
            var villains = all_villains.FindAllFacts("villain");
            foreach (var v in villains)
            {
                byte[] bytes = Encoding.Default.GetBytes(((LexemeValue)v["name"]).Value);
                list_villains.Items.Add(Encoding.UTF8.GetString(bytes));
            }
        }

        private void LoadAllVillains()
        {
            all_villains.Clear();
            all_villains.LoadFromString(System.IO.File.ReadAllText("../../../villains.clp"));
            all_villains.Reset();
            init_listbox();
            codeBox.Text = System.IO.File.ReadAllText("../../../villains.clp");
        }

        private void openFile_Click(object sender, EventArgs e)
        {
        }

        private void fontSelect_Click(object sender, EventArgs e)
        {
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                codeBox.Font = fontDialog1.Font;
                textBox1.Font = fontDialog1.Font;
            }
        }

        private void saveAsButton_Click(object sender, EventArgs e)
        {
            clipsSaveFileDialog.FileName = clipsOpenFileDialog.FileName;
            if (clipsSaveFileDialog.ShowDialog() == DialogResult.OK)
            {
                System.IO.File.WriteAllText(clipsSaveFileDialog.FileName, codeBox.Text);
            }
        }

        private void button_exec_Click(object sender, EventArgs e)
        {
            clips.Clear();
            clips.LoadFromString(System.IO.File.ReadAllText("../../../templates.clp"));

            string villains = "(deffacts villains";
            foreach (int ind in list_villains.SelectedIndices)
            {
                villains += " (villain (name \"" + list_villains.Items[ind].ToString() + "\"))";
            }
            villains += ")";
            System.IO.File.WriteAllText("tmp.clp", villains);
            clips.LoadFromString(System.IO.File.ReadAllText("tmp.clp"));

            clips.LoadFromString(System.IO.File.ReadAllText("../../../rules.clp"));

            clips.Reset();
            textBox1.Text = "Выполнены команды Clear и Reset." + System.Environment.NewLine;
        }
    }
}
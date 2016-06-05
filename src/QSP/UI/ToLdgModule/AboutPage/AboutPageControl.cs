﻿using QSP.UI.Utilities;
using QSP.Utilities;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace QSP.UI.ToLdgModule.AboutPage
{
    public partial class AboutPageControl : UserControl
    {
        public AboutPageControl()
        {
            InitializeComponent();
            initControls();
        }

        private void initControls()
        {
            panel1.BackColor = Color.FromArgb(160, Color.White);

            var ver = Assembly.GetEntryAssembly().GetName().Version;
            versionLbl.Text =
                ver.Major.ToString() + "." +
                ver.Minor.ToString() + "." +
                ver.Build.ToString();
        }

        private void tryOpenFile(string fileName)
        {
            try
            {
                Process.Start(fileName);
            }
            catch (Exception ex)
            {
                LoggerInstance.WriteToLog(ex);
                MsgBoxHelper.ShowWarning("Cannot open the specified file.");
            }
        }

        private void licenseBtn_Click(object sender, EventArgs e)
        {
            tryOpenFile("LICENSE.txt");
        }

        private void siteBtn_Click(object sender, EventArgs e)
        {
            tryOpenFile("https://qsimplan.wordpress.com/");
        }

        private void githubBtn_Click(object sender, EventArgs e)
        {
            tryOpenFile("https://github.com/JetStream96/QSimPlanner");
        }
    }
}
﻿using System;
using System.Windows.Forms;
using System.Management;
using System.Diagnostics;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        ManagementEventWatcher startWatch;
        ManagementEventWatcher stopWatch;

        private void Form1_Load(object sender, EventArgs e)
        {
            startWatch = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace WHERE ProcessName = \"Slack.exe\""));
            startWatch.EventArrived += new EventArrivedEventHandler(startWatch_EventArrived);
            startWatch.Start();

            stopWatch = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStopTrace WHERE ProcessName = \"Slack.exe\""));
            stopWatch.EventArrived += new EventArrivedEventHandler(stopWatch_EventArrived);
            stopWatch.Start();
        }

        static void stopWatch_EventArrived(object sender, EventArrivedEventArgs e)
        {
            Process[] proc = Process.GetProcessesByName("Slack");
            if (proc.Length < 8)
            {
                if (counter == 3)
                {
                    MessageBox.Show("Stop");
                    //Stop record
                    //Give notification about stop
                    counter = 0;
                }
                counter++;
            }
        }

        public static int counter = 0;

        static void startWatch_EventArrived(object sender, EventArrivedEventArgs e)
        {
            Process[] proc = Process.GetProcessesByName("Slack");
            if (proc.Length >= 8)
            {
                if (counter == 3)
                {
                    MessageBox.Show("Start");
                    //Start record
                    //Give notification about start
                    counter = 0;
                }
                counter++;
            }
        }

        private void recordButton_Click(object sender, EventArgs e)
        {
           if (recordButton.Text == "Start recording")
           {
                startWatch.Start();
                stopWatch.Start();
                recordButton.Text = "Stop recording";
           }

           else
           {
                startWatch.Stop();
                stopWatch.Stop();
                recordButton.Text = "Start recording";
           }
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog saveDirectory = new FolderBrowserDialog();

            if (saveDirectory.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(saveDirectory.SelectedPath))
            {
                saveDirectoryTextBox.Text = saveDirectory.SelectedPath;
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            bool cursorNotInBar = Screen.GetWorkingArea(this).Contains(Cursor.Position);

            if (this.WindowState == FormWindowState.Minimized && cursorNotInBar)
            {
                this.ShowInTaskbar = false;
                notifyIcon.Visible = true;
                this.Hide();
            }
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon.Visible = false;
        }
    }
}

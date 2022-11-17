﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;


// VMIPCheck v1.0.1

namespace VMIPCheck
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //avoid loading the app more than once
            String currentprocess = Process.GetCurrentProcess().ProcessName;

            if (Process.GetProcesses().Count(p => p.ProcessName == currentprocess) > 1)
                return;


            using (Process process = new Process())
            {
                args = Environment.GetCommandLineArgs();
                string output;
                try
                {
                    //launch vboxmanage
                    process.StartInfo.FileName = args[1] + "\\VBoxManage.exe";
                    process.StartInfo.Arguments = "guestproperty get \"" + args[2] + "\" \"/VirtualBox/GuestInfo/Net/0/V4/IP\"";
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.Start();

                    //read the standard output of vboxmanage
                    StreamReader reader = process.StandardOutput;
                    output = reader.ReadToEnd();

                    if (output.Equals("")) { output = "VM not found or available."; } else if (args[1].Equals("about")) { output = "KosmicTeal - 2022"; }
                }
                catch (System.IndexOutOfRangeException)
                {
                    output = "Out of range, I need two arguments!";
                }
                catch (System.ComponentModel.Win32Exception)
                {
                    output = "VBoxManage.exe not found, check the file address argument.";
                }

                //get value of IP
                if (output.StartsWith("Value"))
                {
                    output = output.Substring(7);
                }

                //send a notification about the result
                NotifyIcon notifyIcon1 = new NotifyIcon();
                notifyIcon1.Icon = Properties.Resources.kosmicVMapp;
                notifyIcon1.BalloonTipTitle = "VMIPCheck | " + args[2];
                notifyIcon1.BalloonTipText = output;
                notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
                notifyIcon1.Visible = true;
                notifyIcon1.ShowBalloonTip(60000);


                try
                {
                    //close the app once it's complete
                    System.Threading.Thread.Sleep(1000);
                    if (!process.HasExited)
                    {
                        process.Kill();
                    }
                }
                catch (System.InvalidOperationException)
                {
                    Console.WriteLine("Closing software, unable to get VBoxManage.exe");
                }
            }

        }
    }
}

﻿using System;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Windows.Forms;
using System.Configuration.Install;
using System.Diagnostics;

namespace ServiceInstaller
{
    class Program
    {
        static void Main()
        {
            EventLog serviceLogger = new EventLog("BackupManagerServiceLog", ".", "BackupManagerServiceSource");
            string serviceIsInstalled = ServiceController.GetServices().Any(s => s.ServiceName == "BackupManagerService") ? "--uninstall" : "--install";
            var servicePath = Directory.GetCurrentDirectory() + "\\BackupManagerService.exe";

            switch (serviceIsInstalled)
            {
                case "--install":
                    try
                    {
                        ManagedInstallerClass.InstallHelper(new string[] { servicePath });
                        MessageBox.Show($"Служба \"Backup Manager Service\" установлена.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    break;

                case "--uninstall":
                    try
                    {
                        ServiceController serviceController = new ServiceController("BackupManagerService");

                        if (serviceController.Status == ServiceControllerStatus.Running)
                        {
                            serviceController.Stop();
                            serviceLogger.WriteEntry("Service stopped from Service Installer");
                            MessageBox.Show($"Служба \"Backup Manager Service\" уже была запущена и в результате остановлена.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }

                        ManagedInstallerClass.InstallHelper(new string[] { "/u", servicePath });
                        MessageBox.Show($"Служба \"Backup Manager Service\" удалена.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    break;
            }
        }
    }
}

﻿using System;
using System.ComponentModel;
using System.Deployment.Application;

namespace WindowWalker.Components
{
    public class ApplicationUpdates
    {
        static DateTime _lastUpdateCheck = DateTime.Now;
        static int numberOfDaysBetweenCheck = 1;

        static bool alreadyCheckingForUpdate = false;

        static bool updateAvailable = false;

        public static void InstallUpdateSyncWithInfo()
        {
            if (alreadyCheckingForUpdate)
            {
                return;
            }
            else 
            {
                alreadyCheckingForUpdate = true;
            }

            var daysSinceLastUpdate = (DateTime.Now - _lastUpdateCheck).Days;

            if (ApplicationDeployment.IsNetworkDeployed)
            {
                if (updateAvailable)
                {
                    UpdateCheckInfo info = null;
                    ApplicationDeployment ad = ApplicationDeployment.CurrentDeployment;

                    try
                    {
                        info = ad.CheckForDetailedUpdate();
                    }
                    catch {
                        return;
                    }
                    finally
                    {
                        _lastUpdateCheck = DateTime.Now;
                    }

                    if (info.UpdateAvailable || true)
                    {
                        try
                        {
                            ad.Update();
                            System.Windows.Application.Current.Shutdown();
                            System.Windows.Forms.Application.Restart();
                        }
                        catch {
                            return;
                        }
                    }
                }
                else
                {
                    ApplicationDeployment ad = ApplicationDeployment.CurrentDeployment;

                    ad.CheckForUpdateCompleted += new CheckForUpdateCompletedEventHandler(CheckForUpdateCompleted);
                    ad.CheckForUpdateAsync();

                    _lastUpdateCheck = DateTime.Now;
                }
            }
        }

        private static void CheckForUpdateCompleted(object sender, CheckForUpdateCompletedEventArgs e)
        {
            if (e.Error != null || !(e.UpdateAvailable))
            {
                alreadyCheckingForUpdate = false;
                return;
            }
            else
            {
                updateAvailable = true;
                alreadyCheckingForUpdate = false;
            }
        }
    }
}

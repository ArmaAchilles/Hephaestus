using System;
using Hephaestus.Common.Classes;

namespace Hephaestus.Common.Utilities
{
    public static class DriverUtility
    {
        public static Driver GetSelectedDriver(Project project)
        {
            Driver selectedDriver = null;
            
            foreach (Driver driver in project.Drivers)
            {
                if (driver.Name == project.SelectedDriver)
                {
                    selectedDriver = driver;
                }
            }
            
            if (selectedDriver == null)
            {
                throw new ArgumentException($"{project.SelectedDriver} is an invalid driver. Select another driver and try again.");
            }

            return selectedDriver;
        }
    }
}
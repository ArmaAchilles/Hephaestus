using System;
using Hephaestus.Common.Classes;

namespace Hephaestus.Common.Utilities
{
    public static class DriverUtility
    {
        /// <summary>
        /// Gets the currently selected driver from JSON and casts it to the Driver class.
        /// </summary>
        /// <param name="project">Project data.</param>
        /// <returns>Selected driver.</returns>
        /// <exception cref="ArgumentException">If the selected driver does not exist in the Drivers list in the JSON.</exception>
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
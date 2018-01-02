using System;

namespace AddonBuilder.Classes.Updates
{
    class Updater
    {
        public static void UpdateManager(string currentVersion)
        {
            string releaseVersion = GetLatestVersion.GetVersionFromGitHub();
            if (releaseVersion != "")
            {
                bool isLatestVersion = CheckIfLatest.IsLatest(currentVersion, releaseVersion);

                if (isLatestVersion) // TODO: Don't forget to set back to !isLatestVersion!
                {
                    Console.WriteLine("Update is available to Addon Builder!\nDo you wish to update (y/n)?");
                    ConsoleKeyInfo pressedKey = Console.ReadKey();

                    switch (pressedKey.Key)
                    {
                        case ConsoleKey.Y:
                            AutoUpdater.DoUpdate();
                            break;

                        case ConsoleKey.N:
                            Console.WriteLine("Canceled auto updating!");
                            break;

                        default:
                            Console.WriteLine("Wrong key pressed! Canceling auto update!");
                            break;
                    }
                }
            }
        }
    }
}

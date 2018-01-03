using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddonBuilder.Classes.Updates
{
    class CheckIfLatest
    {
        public static bool IsLatest(string currentVersion, string releaseVersion)
        {
            bool isLatest = false;

            Version _currentVersion = Version.Parse(currentVersion.Remove(0, 1));
            Version _releaseVersion = Version.Parse(releaseVersion.Remove(0, 1));

            if (_currentVersion >= _releaseVersion)
                isLatest = true;

            return isLatest;
        }
    }
}
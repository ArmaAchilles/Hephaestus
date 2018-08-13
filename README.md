# Hephaestus
Builds PBOs from folders using Bohemia Interactive's Addon Builder.

## Setup
To download Hephaestus go to the [releases tab](https://github.com/ArmaAchilles/AddonBuilder/releases) and download the .rar file.

1. Unzip the files and store them out of any tracked Git folders.
2. Configure Hephaestus in the supplied **Hephaestus.ini** file.
3. Build as many PBOs as you need!

## Features
- Supports custom key versions by launching with the first parameter, e.g. 0.1.0.
- Closes Arma 3 and awaits full closure before starting the building of the addon folders (if configured).
- Supports launching with no key to not sign the addon.
- Builds addons in parallel to short down building times.
- After completion of building, auto-launches Arma 3 (configurable).
- Checks SHA1 checksums of all code folders and builds only if at least one of them is modified
- Stores all temporary files into a one tidy folder in temp (for quick and easy removal of cached files).
- Builds PBOs, what did you expect?

## Credits
- [CreepPork_LV](https://github.com/CreepPork) the originator of Hephaestus
- [ranta](https://social.msdn.microsoft.com/profile/ranta/?ws=usercard-mini) for hashing methods
- [MaliceGFS](https://github.com/MaliceGFS) for QoL improvements and various refactorings

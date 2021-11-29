# ![CleanSweep2 Logo](https://i.imgur.com/mdSoQbh.png) CleanSweep2
The easiest and quickest Windows junk file remover on the planet.

## CleanSweep2 Demonstration
![CleanSweep2](https://i.imgur.com/BsTk0Wd.gif)

## Supported Operating Systems
* Windows 11
* Windows 10
* Windows 8/8.1
* Windows 7
* Previous Windows OS's *may* work, but some features may be incompatible or produce undesired results.

## Cleaning Features
* Temporary Files
* Temporary Setup Files
* Temporary Internet Files
* Event Viewer Logs
* Recycle Bin
* Chrome Cache
* Thumbnail Cache
* User File History Snapshots
* Windows .Old Folder
* Windows Defender Logs
* Microsoft Office Cache
* Microsoft Edge Cache
* Windows Installer Cache
* Windows Update Logs
* Windows Error Reports
* Windows Delivery Optimization

## Additional Features
* Provides potential reclaimable disk space if all cleaning options were selected.
* Provides total reclaimed disk space after all selected cleaning operations have completed.
* Detailed log window of all cleaning operations.
* Verbose and non-verbose output to the log window.
* Show or hide operation windows (command prompts, etc) that perform tasks.
* Update checking.

## Potential Upcoming Features and Roadmap
* I am now developing a CLI-based version of CleanSweep2 for silent and scheduled cleans.
* Future updates past major feature releases will be quality-of-life additions and bugfixes.

## Getting Started With CleanSweep2
1. Download the [latest](https://github.com/thomasloupe/CleanSweep2/releases) CleanSweep2 release and extract the contents. You can download [WinRAR](https://www.rarlab.com/download.htm) or use [7ZIP](https://www.7-zip.org/) to extract the contents.
1. Run CleanSweep2.exe.
1. Select any cleaning options you wish.
1. Click "Sweep it!".

# CleanSweep2_CLI
CleanSweep2_CLI is the command-line based version of CleanSweep2 which offers all features CleanSweep2 has to offer, except that jobs are passed through arguments, including logging actions to a logfile on disk, changing verbosity modes, selective cleaning, and silent running for scheduled cleaning through Windows Task Scheduler.

## Getting Started with CleanSweep2_CLI
1. Download the [latest](https://github.com/thomasloupe/CleanSweep2/releases) CleanSweep2 release and extract the contents.
1. For manual cleaning, create a shortcut to the executable, and modify the arguments in the "Target" properties field, then run CleanSweep2_CLI.
1. For automated cleaning, launch Task Scheduler in Windows, click "Create Basic Task". Enter a name for the task (example: CleanSweep2_CLI) and give a description if desired. Select a scheduled time for CleanSweep2_CLI to run. On the "Action" page, select "Start a program". Select the CleanSweep2_CLI executable, and pass desired arguments. For a list of arguments, see below.

## CleanSweep2_CLI Arguments
`-1` *(Remove Temporary Files)*  
`-2` *(Remove Temporary Setup Files)*  
``-3`` *(Remove Temporary Internet Files)*  
``-4`` *(Removes Event Viewer Logs)*  
``-5`` *(Empties the Recycle Bin)*  
``-6`` *(Removes Windows Error Reports)*  
``-7`` *(Removes Delivery Optimization Files)*  
``-8`` *(Clears Thumbnail Cache)*  
``-9`` *(Remove User File History)*  
``10`` *(Removes Windows.old Directory)*  
``-11`` *(Removes Windows Defender Log Files)*  
``-12`` *(Removes Microsoft Office Cache)*  
``-13`` *(Removes Microsoft Edge Cache)*  
``-14`` *(Removes Google Chome Cache)*  
``-15`` *(Removes Windows Installer Cache)*  
``-16`` *(Removes Windows Update Log Files)*  
``-log "path"`` *(Writes a log file to the path specified of actions performed. Path must be in double quotes.)*  
``-v1`` *(Sets the verbosity to low)*  
``-v2`` *(Sets the verbosity to high)*  
``-visible`` *(Makes the CleanSweep2_CLI console window visible. By default, CleanSweep2_CLI runs hidden.)*  

## Potential Data Loss Warning and Disclaimer:
Due to the nature of CleanSweep2 being an application that removes data from a Windows machine, it is imperative to note that potential or undesired data loss can potentially occur. Please do not select options in which you wish to keep data for, especially for Windows .Old folders, and previous Windows installs. These are unrecoverable once removed, as are most options in this application. Please use CleanSweep2 at your own risk.

# Important: Please Read!
* If you need help, please feel free to get in touch with me on [Twitter](https://twitter.com/acid_rain), or open a new issue if it doesn't already exist.
* CleanSweep and CleanSweep2 are free, and they will always be free. Please do not pay anyone for this application. However, if you found CleanSweep or CleanSweep2 worth donating something, you can donate from within CleanSweep2 by accessing the "Donate" menu option from the "Help" context menu or just [click here](https://paypal.me/thomasloupe).
* CleanSweep, CleanSweep2, and CleanSweep2_CLI are applications I created in my spare time to continue learning multiple programming languages. I would not consider myself an "expert" programmer, and should probably not be treated as such.

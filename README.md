# ![CleanSweep Logo](https://i.imgur.com/mdSoQbh.png) CleanSweep
The easiest and quickest Windows junk file remover on the planet.

## Supported Operating Systems
* Windows 11, 10, 8.1, 8, 7. Windows 11 is the only OS version CleanSweep is tested on.

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
* Provides potential reclaimable disk space prior to cleaning.
* Provides total reclaimed disk space post cleaning.
* Detailed logging to log window/file.
* Verbose and non-verbose output.
* Remembers configuration and previous cleaning selections (GUI only).
* Show or hide operation windows (command prompts, etc) that perform tasks.
* Update checking.
* Perform tasks silently without user interaction (CLI only).
* Supports Windows Task Scheduler (CLI preferred).

## CleanSweep_GUI Demonstration
![CleanSweep](https://i.imgur.com/SMhZIGp.gif)

## Getting Started With CleanSweep_GUI
1. Download the [latest](https://github.com/thomasloupe/CleanSweep/releases) CleanSweep release and extract the contents. You can download [WinRAR](https://www.rarlab.com/download.htm) or use [7ZIP](https://www.7-zip.org/) to extract the contents.
1. Run CleanSweep.exe.
1. Select any cleaning options you wish.
1. Click "Sweep it!".

# CleanSweep_CLI
CleanSweep_CLI is the command-line based version of the CleanSweep Windows application, which offers all the same features. Cleaning obs are passed through arguments, including logging actions to a logfile on disk, changing verbosity modes, selective cleaning, and silent running for scheduled cleaning through Windows Task Scheduler.

## CleanSweep_CLI Demonstration
![CleanSweep](https://i.imgur.com/eqwglBE.gif)

## Getting Started with CleanSweep_CLI
1. Download the [latest](https://github.com/thomasloupe/CleanSweep/releases) CleanSweep release and extract the contents.
1. For manual cleaning, create a shortcut to the executable, and modify the arguments in the "Target" properties field, then run CleanSweep_CLI.
1. For automated cleaning, launch Task Scheduler in Windows, click "Create Basic Task". Enter a name for the task (example: CleanSweep_CLI) and give a description if desired. Select a scheduled time for CleanSweep_CLI to run. On the "Action" page, select "Start a program". Select the CleanSweep_CLI executable, and pass desired arguments. For a list of arguments, see below.

## CleanSweep_CLI Arguments
`-1` *(Remove Temporary Files)*  
`-2` *(Remove Temporary Setup Files)*  
``-3`` *(Remove Temporary Internet Files)*  
``-4`` *(Removes Event Viewer Logs)*  
``-5`` *(Empties the Recycle Bin)*  
``-6`` *(Removes Windows Error Reports)*  
``-7`` *(Removes Delivery Optimization Files)*  
``-8`` *(Clears Thumbnail Cache)*  
``-9`` *(Remove User File History)*  
``-10`` *(Removes Windows.old Directory)*  
``-11`` *(Removes Windows Defender Log Files)*  
``-12`` *(Removes Microsoft Office Cache)*  
``-13`` *(Removes Microsoft Edge Cache)*  
``-14`` *(Removes Google Chome Cache)*  
``-15`` *(Removes Windows Installer Cache)*  
``-16`` *(Removes Windows Update Log Files)*  
``-log`` *(Writes a log file of actions performed and their timestamps to the "CleanSweep Logs" folder under the user account's Documents folder.)*  
``-showoperationwindows`` *(Make all spawned windows visible.)*  
``-update`` *(Checks for updates. This argument requires the -visible argument.)*  
``-v1`` *(Sets the verbosity to low)*  
``-v2`` *(Sets the verbosity to high)*  

## Potential Data Loss Warning and Disclaimer:
Due to the nature of CleanSweep being an application that removes data from a Windows machine, it is imperative to note that potential or undesired data loss can potentially occur. Please do not select options in which you wish to keep data for, especially for Windows .Old folders, and previous Windows installs. These are unrecoverable once removed, as are most options in this application. Please use CleanSweep at your own risk.

# Important: Please Read!
* If you need help, please feel free to get in touch with me on [Twitter](https://twitter.com/acid_rain), or open a new issue if it doesn't already exist.
* CleanSweep is free, and it will always be free. Please do not pay anyone for this application. However, if you found CleanSweep worth donating for, you can donate from within the CleanSweep application by accessing the "Donate" menu option from the "Help" context menu in Windows, or by copying the donate link provided in CleanSweep_CLI. You can also donate directly to me [here](https://paypal.me/thomasloupe) (PayPal).
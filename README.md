# BatterySaverAppKiller <br />
This simple app shuts down and restarts chosen processes depending on the battery percentage to save battery.
<br />

## Detailed Description <br />
When your battery percentage ticks or becomes lower than the number you specified in the `AppConfig.json` file, it will try and find running processes with the names you specified and kill them. When your battery percentage becomes higher than your specified number or it starts charging, it will (try to) start them again.
When this program is already running and you open it again, it will close the old one AND itself, so basically toggling itself.
<br />

## Config <br />
For configuration of this program, open `AppConfig.json`:
- `batteryPercentage`: change this to the percentage the program should toggle on.
- `processNames`: add the name of the processes you want to kill here. (e.g. [ "Discord", "Outlook" ] etc.)<br />

To find the name of the process you want to add, the program creates the file `RunningProcesses.txt`, which lists all running processes at that time. 

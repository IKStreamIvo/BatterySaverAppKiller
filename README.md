# BatterySaverAppKiller <br />
This simple app shuts down and restarts chosen processes depending on the batterypercentage to save battery.
<br />
## Config <br />
For configuration of this program, open `AppConfig.json`:
- `batteryPercentage`: change this to the percentage the program should toggle on.
- `processNames`: add the name of the processes you want to kill here. (e.g. [ "Discord", "Outlook" ] etc.)<br />

To find the name of the process you want to add, the program creates the file `RunningProcesses.txt`, which lists all running processes at that time. 
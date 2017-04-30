# FileWatcher
A quick and simple implementation of Microsoft's FileSystemWatcher to track and monitor changes to files on my computer.

SET-UP:
I currently have the service to run on my 3 drives. If you have a different configuration, go into FileWatcher > Service.cs and edit lines 26-28.

INSTALLATION (Win10):
1) Open up developer command prompt in Administration mode. 
2) Navigate to the folder containing the executable file.
3) Type in "installutil FileWatcher.exe" and verify successful install.
4) Then go to start > run > services.
5) Look for a service called "File Watcher" and start the service.
6) If you decide to make changes to the service code, simply stop the service, add any changes, save changes, and start the service back up.

TODO:
1) Add configuration on which file types to monitor
2) Add configuration on which folders to monitor
3) Add minimalist/verbose/default options. The current version will be default, verbose will include more details about changes in file size, user who initiated changes, etc. Minimalist will be just the file name, change type, and timestamp.
4) If I get around to it, add an installer that will automatically execute the installation steps.

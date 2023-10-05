Creating / Deploying the Windows Services


dotnet publish -c Release 

sc.exe create WorkerServiceShutdownTesting binPath= "C:\kbs\private\WorkerServiceShutdownTesting\WorkerServiceShutdownTesting\bin\Release\net7.0\publish\WorkerServiceShutdownTesting.exe"

sc.exe delete WorkerServiceShutdownTesting

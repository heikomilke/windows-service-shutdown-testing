# windows-service-shutdown-testing
We want to find out what the events are that we (a dotnet core service) catch when windows is shutting down. How long we can live. How often we get notified


# Findings so far

- when using dotnet BackgroundService we have 3 lifecycle event methods
  - start
  - execute (long running)
  - stop
 
- we also have a shutdown token that is set to canceled by the OS once service is requested to shut down

- the order of things:
  1. stop is called
  2. token is set to canceled
 
- we can ignore the cancel and continue to do some more work (i.e. logging :)

# questions

- can we find out the reason for the shutdown
- for how long can we ignore the shutdown
  - are methods like Delay etc still available to us while in mode shutdown?
 
# update

- we added some code that attempts to stay alive after the shutdown has been caught
- we managed to stay in a loop for 1 more second but after that we fail any further exceptions or anything else and the system reboots

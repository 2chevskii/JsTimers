# JsTimers [![CodeFactor](https://www.codefactor.io/repository/github/2chevskii/jstimers/badge/master)](https://www.codefactor.io/repository/github/2chevskii/jstimers/overview/master)
*Simple and easy-to-use library bringing JavaScript-style timers into .NET*

## Description
This library brings timer functions similar to browser and NodeJS's enviroments.

## Functions
```cs
// Invokes callback after set time
BaseTimer SetTimeout(Action callback /* callback cannot be null */, float timeoutSeconds /* timeout cannot be negative, float is converted to uint */);
BaseTimer SetTimeout(Action callback, uint timeoutMilliseconds);

// Repetiately invokes callback until stopped by ClearTimer
BaseTimer SetInterval(Action callback, float intervalSeconds, executeImmediate = false /* execute callback right after interval initialization */);
BaseTimer SetInterval(Action callback, uint timeoutMilliseconds, executeImmediate = false);

//Clears any existing timer if it is not cleared already
bool ClearTimer(BaseTimer timer, executeCallback = false /* where to execute callback or not before clearing timer */);
```

## Usage
```cs
using JsTimers; // This namespace contains library fucntions

// Let's imagine we're inside some function

// Delayed callback invocation
BaseTimer.SetTimeout(() => Console.WriteLine("A little callback was born to say these words!"), 3.0f); // 3sec delay, then "A little callback was born to say these words!"

int iteration = 0;
BaseTimer timer = null;
timer = BaseTimer.SetInterval(() => {
    Console.WriteLine("This is iteration " + ++iteration)
    if(iteration >= 4){
        BaseTimer.ClearTimer(timer);
    }
}, 1000u); // Says "This is iteration {number}" 4 times and then destroys the timer
```

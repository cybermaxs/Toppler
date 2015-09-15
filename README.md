[![Build status](https://ci.appveyor.com/api/projects/status/ld2m0ue663fpd10n?svg=true)](https://ci.appveyor.com/project/Cybermaxs/toppler)
[![Nuget](https://img.shields.io/nuget/dt/toppler.svg)](http://nuget.org/packages/toppler)
[![Nuget](https://img.shields.io/nuget/v/toppler.svg)](http://nuget.org/packages/toppler)
[![Coverage Status](https://coveralls.io/repos/Cybermaxs/toppler/badge.svg?branch=master&service=github)](https://coveralls.io/github/Cybermaxs/toppler?branch=master)

# Toppler
a minimalist library to manage countings, rankings &amp; overall leaderboard

## Installing via NuGet
```
Install-Package Toppler
```

## Overview 
TODO


## Use Cases

1. Counting stuff ( in a Distributed environnment)
2. Leaderboards & Ranking tables with Custom Time Ranges (Second, Minute, Hour, Day)
3. Very Basic Recommendation system (mixing random & popular values)
4. Rate limiter with a fixed/sliding time range (eg last N minutes, last N seconds, ...)


# Show me the code !

##Step 1 : Setup Redis connection & Toppler Settings
```csharp
    Top.Setup(redisConfiguration: "localhost:6379");
```
the string parameter is the redis configuration for SE.Redis Read more on available options [here](https://github.com/StackExchange/StackExchange.Redis/blob/master/Docs/Configuration.md)

Note : additional parameters are available for advanced usage.

## Step 2 : Add hit(s) (when something interesing happened)
```csharp
    //somewhere
    Top.Counter.HitAsync("myevent");
    //elsewhere
    Top.Counter.HitAsync("myevent");
```

## Step 3 : Get Top events
```csharp
//get all for the current day
var tops = await Top.Ranking.AllAsync(Granularity.Day);
//returns "myevent", 2
```

That'all ! Many additional options are available to manage granularities, resolutions, contexts, ... Read the wiki (Coming soon).

# Acknowledgements
+ Salvatore Sanfilippo (@antirez) : Creator of Redis
+ Marc Gravell(@marcgravell) : Creator of [StackExchange.Redis](https://github.com/StackExchange)) is a high performance general purpose redis client for .NET languages
+ Stuart Quin (ApiAxle) : [Storing time series statistics in Redis](http://blog.apiaxle.com/post/storing-near-realtime-stats-in-redis/)

# License
Licensed under the terms of the [MIT License](http://opensource.org/licenses/MIT)

Want to contribute ?
------------------
- Beginner => Download, Star, Comment/Tweet, Kudo, ...
- Amateur => Ask for help, send feature request, send bugs
- Pro => Pull request, promote

Thank you

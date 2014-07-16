*Work in progress ...*

# Toppler
a minimalist library to manage countings, rankings &amp; overall leaderboard

## Installing via NuGet
```
Install-Package Toppler
```

## Overview 
TODO


## Use Cases

1. Counting stuff
2. Leaderboards & Rankings with Custom Time Ranges (Second, Minute, Hour, Day)
3. Very Basic Recommendation system (mixing random & topN values)


# Show me the code !

##Step 1 : Setup Redis connection & Toppler Settings
```csharp
    TopplerClient.Setup(redisConfiguration: "localhost:6379");
```
the string parameter is the redis configuration for SE.Redis Read more on available options [here](https://github.com/StackExchange/StackExchange.Redis/blob/master/Docs/Configuration.md)

Note : additional parameters are available for advanced usage.

## Step 2 : Add hit(s) when something nteresing happened)
```csharp
    TopplerClient.Counter.HitAsync("myevent");
```
Note : additional parameters are available for advanced usage


## Step 3 : Get Top events
```csharp
TopplerClient.Ranking.GetOverallTops(Granularity.Day);
//returns "myevent", 1
```

That'all ! Many additional options are available to manage granularities, resolutions, contexts, ... Read the wiki (Coming soon)

# Acknowledgements
Salvatore Sanfilippo (@antirez) : Creator of Redis
Marc Gravell(@marcgravell) : Creator of [StackExchange.Redis](https://github.com/StackExchange)) is a high performance general purpose redis client for .NET languages
Stuart Quin (ApiAxle) : [Storing time series statistics in Redis](http://blog.apiaxle.com/post/storing-near-realtime-stats-in-redis/)

# License
Licensed under the terms of the [MIT License](http://opensource.org/licenses/MIT)
SET NUGET=.nuget\nuget
%NUGET% pack src\Toppler_Net40\Toppler_Net40.csproj  -OutputDirectory dist -Build -Prop Configuration=Release
%NUGET% pack src\Toppler\Toppler.csproj  -OutputDirectory dist -Build -Prop Configuration=Release

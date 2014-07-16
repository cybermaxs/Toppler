SET NUGET=.nuget\nuget
%NUGET% pack src\Toppler\Toppler.csproj -OutputDirectory dist -Build -Prop Configuration=Release

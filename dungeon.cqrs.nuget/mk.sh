nuget pack ./dungeon.cqrs.nuspec -NoDefaultExcludes
nuget pack ./dungeon.cqrs.mongodb.nuspec -NoDefaultExcludes
cp ./dungeon.cqrs.*.nupkg ../../dungeon_nugets/
rm ./dungeon.cqrs.*.nupkg
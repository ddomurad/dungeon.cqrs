cdir="$(pwd)"
wdir="$(dirname $0)"

echo "Move to: " $wdir
cd $wdir


echo "Build and packing library (configuration=Release)"
dotnet pack --configuration=Release

echo "Copy packages to nuget_output"
find src/ -type f | grep -i nupkg | xargs -i cp {} ./nuget_output/

echo "Move back to: " $cdir
cd $cdir
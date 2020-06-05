dotnet publish ../src/Patcher.sln -o="./publish/win-x64" -f="netcoreapp3.1" -p:PublishSingleFile=true  -c=Release --runtime win-x64
dotnet publish ../src/Patcher.sln -o="./publish/osx-64" -f="netcoreapp3.1" -p:PublishSingleFile=true -c=Release --runtime osx-x64
dotnet publish ../src/Patcher.sln -o="./publish/linux-x64" -f="netcoreapp3.1" -p:PublishSingleFile=true -c=Release --runtime linux-x64
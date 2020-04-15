dotnet publish ./Patcher.sln -o="./publish/win-x64" -f="netcoreapp3.1" --runtime win-x64
dotnet publish ./Patcher.sln -o="./publish/osx-64" -f="netcoreapp3.1" --runtime osx-x64
dotnet publish ./Patcher.sln -o="./publish/linux-x64" -f="netcoreapp3.1" --runtime linux-x64
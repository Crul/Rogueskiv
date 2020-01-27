dotnet publish Rogueskiv.Run --configuration Release --framework netcoreapp3.0 --output releases\Rogueskiv-%1-linux-x64 --runtime linux-x64 --self-contained
cd releases
7z a Rogueskiv-%1-linux-x64-compressed.zip Rogueskiv-%1-linux-x64
cd ..

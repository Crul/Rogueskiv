dotnet publish Rogueskiv.Run --configuration Release --framework netcoreapp3.0 --output releases\Rogueskiv-%1-win-x64 --runtime win-x64 --self-contained
copy Scripts\rogueskiv.nsi releases\Rogueskiv-%1-win-x64\.
cd releases
7z a Rogueskiv-%1-win-x64-compressed.zip Rogueskiv-%1-win-x64
cd Rogueskiv-%1-win-x64
makensis rogueskiv.nsi
cd ..
rename rogueskiv-win-x64-installer.exe rogueskiv-%1-win-x64-installer.exe
cd ..

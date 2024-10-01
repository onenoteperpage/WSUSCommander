dotnet clean;
dotnet restore;
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true;
& C:\Users\Danijel.Wynyard\source\repos\WSUSCommander\bin\Release\net8.0-windows\win-x64\publish\WSUSCommander.exe

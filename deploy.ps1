$env:Path += ";C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin"
MSBuild PlaylistGrabber.sln -t:Rebuild -p:Configuration=Release
xcopy /I /Y .\PlaylistGrabber\bin\Release\netcoreapp3.1 ..\..\Tools\PlaylistGrabber
@echo off

pushd src
dotnet publish -c Release
popd

rmdir /s /q dist
mkdir dist

xcopy /y /e src\bin\Release\netstandard2.0\publish dist
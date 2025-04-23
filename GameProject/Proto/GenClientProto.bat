@echo off
setlocal

:: 设置protoc.exe路径
set PROTOC_PATH=E:\UnityWorkSpace\MyGame\GameProject\Proto\protoc.exe

:: 设置proto文件源目录
set PROTO_SRC_DIR=E:\UnityWorkSpace\MyGame\GameProject\Proto\Src

:: 设置输出目录
set OUTPUT_DIR=E:\UnityWorkSpace\MyGame\GameProject\Unity\Assets\Script\GameScript\ProtoBuf\Proto

:: 创建输出目录（如果不存在）
if not exist "%OUTPUT_DIR%" mkdir "%OUTPUT_DIR%"

:: 遍历proto文件并生成C#代码
for %%f in ("%PROTO_SRC_DIR%\*.proto") do (
    echo Processing %%f...
    "%PROTOC_PATH%" --csharp_out="%OUTPUT_DIR%" --proto_path="%PROTO_SRC_DIR%" "%%f"
)

echo All proto files have been processed.
pause
endlocal
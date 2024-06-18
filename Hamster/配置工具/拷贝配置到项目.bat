@echo off
echo copy config
copy /y "表结构\*.*" "..\Project\Assets\Scripts\Configs\"
copy /y "配置输出JSON\*.*" "..\Project\Assets\Resources\Config\"
echo success!
pause
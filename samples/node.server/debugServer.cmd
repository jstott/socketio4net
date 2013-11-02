REM Must start inspector first!!
set ip=localhost
start "node-inspector" /MIN node-inspector
REM Waiting 4 seconds
PING 1.1.1.1 -n 1 -w 4000 > NUL
start "node" /MIN  node --debug-brk  server.js
PING 1.1.1.1 -n 1 -w 3000 > NUL
start "chrome debug" chrome.exe http://%ip%:8080/debug?port=5858
//start "chrome session"  chrome.exe http://%ip%:8000/

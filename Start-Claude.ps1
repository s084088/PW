# 设置控制台输出编码为 UTF-8，避免中文乱码
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

# ========== 第一步：配置进程级代理环境变量（Claude 进程会读取） ==========
# 设置 HTTP/HTTPS 代理（127.0.0.1:7890 替换为你的实际代理地址）
$env:HTTP_PROXY = "http://127.0.0.1:7890"
$env:HTTPS_PROXY = "http://127.0.0.1:7890"
# 可选：跳过本地地址代理（避免影响内网）
$env:NO_PROXY = "localhost,127.0.0.1,::1"

Write-Host "已配置代理环境变量：HTTP/HTTPS -> 127.0.0.1:7890" -ForegroundColor Green
Write-Host ""

# ========== 第二步：切换到目标目录 + 启动 Claude ==========
Set-Location -Path "D:\dev\PW"
Write-Host "已切换到目录：D:\dev\PW" -ForegroundColor Cyan
Write-Host ""

# 启动 Claude Code（强制用当前环境变量的代理）
Write-Host "启动 Claude Code..." -ForegroundColor Yellow
& "C:\Users\s0840\.local\bin\claude.exe"

# 可选：保留窗口（Claude 退出后不关闭）
pause
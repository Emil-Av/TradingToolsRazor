# VPS Configuration 3ADyrg5eRlBE
$VPS_IP = "217.154.9.117"
$VPS_USER = "root"
$APP_PATH = "/var/www/tradingtools"
$PROJECT_NAME = "TradingToolsRazor"
$SERVICE_NAME = "tradingtools"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Deploying TradingTools to VPS" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Step 1: Build the application
Write-Host "[1/7] Building application..." -ForegroundColor Green
dotnet publish $PROJECT_NAME -c Release -r linux-x64 --self-contained false -o ./publish

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}
Write-Host "✓ Build successful" -ForegroundColor Green
Write-Host ""

# Step 2: Remove excluded folders from publish directory
Write-Host "[2/7] Removing excluded folders from publish..." -ForegroundColor Green
$foldersRemoved = 0

if (Test-Path "./publish/wwwroot/Screenshots") {
    Remove-Item -Recurse -Force ./publish/wwwroot/Screenshots
    Write-Host "✓ Screenshots removed" -ForegroundColor Green
    $foldersRemoved++
}
if (Test-Path "./publish/wwwroot/ScreenshotsDev") {
    Remove-Item -Recurse -Force ./publish/wwwroot/ScreenshotsDev
    Write-Host "✓ ScreenshotsDev removed" -ForegroundColor Green
    $foldersRemoved++
}
if (Test-Path "./publish/wwwroot/vendor/fontawesome-free") {
    Remove-Item -Recurse -Force ./publish/wwwroot/vendor/fontawesome-free
    Write-Host "✓ fontawesome-free removed" -ForegroundColor Green
    $foldersRemoved++
}

if ($foldersRemoved -eq 0) {
    Write-Host "✓ No excluded folders found in publish" -ForegroundColor Green
} else {
    Write-Host "✓ $foldersRemoved folder(s) removed from publish" -ForegroundColor Green
}
Write-Host ""

# Step 3: Stop the service on VPS
Write-Host "[3/7] Stopping application service..." -ForegroundColor Green
ssh ${VPS_USER}@${VPS_IP} "sudo systemctl stop $SERVICE_NAME 2>/dev/null || true"
Write-Host "✓ Service stopped" -ForegroundColor Green
Write-Host ""

# Step 4: Backup Screenshots and fontawesome-free on server
Write-Host "[4/7] Protecting Screenshots and fontawesome-free on server..." -ForegroundColor Green
ssh ${VPS_USER}@${VPS_IP} @"
    if [ -d ${APP_PATH}/wwwroot/Screenshots ]; then
        sudo mv ${APP_PATH}/wwwroot/Screenshots /tmp/screenshots_backup_temp
        echo 'Screenshots moved to temp'
    fi
    if [ -d ${APP_PATH}/wwwroot/vendor/fontawesome-free ]; then
        sudo mv ${APP_PATH}/wwwroot/vendor/fontawesome-free /tmp/fontawesome_backup_temp
        echo 'fontawesome-free moved to temp'
    fi
"@
Write-Host "✓ Protected folders backed up" -ForegroundColor Green
Write-Host ""

# Step 5: Upload files to VPS
Write-Host "[5/7] Uploading application files to VPS..." -ForegroundColor Green
ssh ${VPS_USER}@${VPS_IP} "sudo rm -rf ${APP_PATH}/* && sudo mkdir -p ${APP_PATH}"

scp -r ./publish/* ${VPS_USER}@${VPS_IP}:${APP_PATH}/

if ($LASTEXITCODE -ne 0) {
    Write-Host "File upload failed!" -ForegroundColor Red
    # Restore protected folders
    ssh ${VPS_USER}@${VPS_IP} @"
        [ -d /tmp/screenshots_backup_temp ] && sudo mv /tmp/screenshots_backup_temp ${APP_PATH}/wwwroot/Screenshots
        [ -d /tmp/fontawesome_backup_temp ] && sudo mv /tmp/fontawesome_backup_temp ${APP_PATH}/wwwroot/vendor/fontawesome-free
"@
    exit 1
}
Write-Host "✓ Files uploaded" -ForegroundColor Green
Write-Host ""

# Step 6: Restore protected folders
Write-Host "[6/7] Restoring protected folders..." -ForegroundColor Green
ssh ${VPS_USER}@${VPS_IP} @"
    if [ -d /tmp/screenshots_backup_temp ]; then
        sudo mkdir -p ${APP_PATH}/wwwroot
        sudo mv /tmp/screenshots_backup_temp ${APP_PATH}/wwwroot/Screenshots
        echo 'Screenshots restored'
    else
        sudo mkdir -p ${APP_PATH}/wwwroot/Screenshots
        echo 'Screenshots folder created'
    fi
    
    if [ -d /tmp/fontawesome_backup_temp ]; then
        sudo mkdir -p ${APP_PATH}/wwwroot/vendor
        sudo mv /tmp/fontawesome_backup_temp ${APP_PATH}/wwwroot/vendor/fontawesome-free
        echo 'fontawesome-free restored'
    else
        sudo mkdir -p ${APP_PATH}/wwwroot/vendor/fontawesome-free
        echo 'fontawesome-free folder created'
    fi
    
    sudo chown -R ${VPS_USER}:${VPS_USER} ${APP_PATH}
    echo 'Permissions set'
"@
Write-Host "✓ Protected folders restored" -ForegroundColor Green
Write-Host ""

# Step 7: Start service
Write-Host "[7/7] Starting service..." -ForegroundColor Green
ssh ${VPS_USER}@${VPS_IP} @"
    cd $APP_PATH
    sudo systemctl daemon-reload
    sudo systemctl enable $SERVICE_NAME
    sudo systemctl start $SERVICE_NAME
    sleep 3
    sudo systemctl status $SERVICE_NAME --no-pager
"@

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Deployment Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Application URL: http://$VPS_IP" -ForegroundColor Yellow
Write-Host "Migrations will run automatically on startup" -ForegroundColor Yellow
Write-Host ""
Write-Host "Protected folders (NOT deployed):" -ForegroundColor White
Write-Host "  ✓ Screenshots - Preserved on server" -ForegroundColor Gray
Write-Host "  ✓ ScreenshotsDev - Never deployed" -ForegroundColor Gray
Write-Host "  ✓ fontawesome-free - Preserved on server" -ForegroundColor Gray
Write-Host ""
Write-Host "Useful commands:" -ForegroundColor White
Write-Host "  Check logs: ssh ${VPS_USER}@${VPS_IP} 'sudo journalctl -u $SERVICE_NAME -f'" -ForegroundColor Gray
Write-Host "  Restart app: ssh ${VPS_USER}@${VPS_IP} 'sudo systemctl restart $SERVICE_NAME'" -ForegroundColor Gray
Write-Host "  Stop app: ssh ${VPS_USER}@${VPS_IP} 'sudo systemctl stop $SERVICE_NAME'" -ForegroundColor Gray
Write-Host ""

# Clean up local publish folder
Remove-Item -Recurse -Force ./publish
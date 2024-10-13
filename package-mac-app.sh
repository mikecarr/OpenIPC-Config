#!/bin/bash

APP_NAME="OpenIPC-Config"
PUBLISH_DIR="./bin/Release/net8.0/osx-arm64/publish"
APP_DIR="${APP_NAME}.app"

# Create the app bundle structure
mkdir -p "${APP_DIR}/Contents/MacOS"
mkdir -p "${APP_DIR}/Contents/Resources"

# Copy the published binaries
cp -R "${PUBLISH_DIR}/"* "${APP_DIR}/Contents/MacOS/"

# Copy the icon file (icns)
cp ./Assets/OpenIPC.icns "${APP_DIR}/Contents/Resources/${APP_NAME}.icns"

# Create the Info.plist file
cat << EOF > "${APP_DIR}/Contents/Info.plist"
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
  <key>CFBundleName</key>
  <string>${APP_NAME}</string>
  <key>CFBundleExecutable</key>
  <string>OpenIPC-Config</string>
  <key>CFBundleIdentifier</key>
  <string>com.openipc.${APP_NAME}</string>
  <key>CFBundleVersion</key>
  <string>0.1</string>
  <key>CFBundlePackageType</key>
  <string>APPL</string>
  <key>LSMinimumSystemVersion</key>
  <string>10.13</string>
  <key>CFBundleIconFile</key>
  <string>${APP_NAME}.icns</string>
</dict>
</plist>
EOF

# Set executable permissions for the main binary
chmod +x "${APP_DIR}/Contents/MacOS/OpenIPC-Config"

echo "Packaged ${APP_NAME}.app successfully."
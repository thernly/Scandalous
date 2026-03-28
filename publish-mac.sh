#!/bin/bash
# Publishes Scandalous.Avalonia as a macOS .app bundle.
# Usage: ./publish-mac.sh [osx-arm64|osx-x64]
# Default runtime: osx-arm64

set -e

RUNTIME="${1:-osx-arm64}"
PROJECT="Scandalous.Avalonia/Scandalous.Avalonia.csproj"
PUBLISH_DIR="publish/mac-tmp"
APP_NAME="Scandalous"
APP_BUNDLE="publish/${APP_NAME}.app"

echo "Publishing for $RUNTIME..."
dotnet publish "$PROJECT" -c Release -r "$RUNTIME" --self-contained true -o "$PUBLISH_DIR"

echo "Creating .app bundle..."
rm -rf "$APP_BUNDLE"
mkdir -p "$APP_BUNDLE/Contents/MacOS"
mkdir -p "$APP_BUNDLE/Contents/Resources"

cp -r "$PUBLISH_DIR/"* "$APP_BUNDLE/Contents/MacOS/"
cp "Scandalous.Avalonia/Info.plist" "$APP_BUNDLE/Contents/"
chmod +x "$APP_BUNDLE/Contents/MacOS/$APP_NAME"

rm -rf "$PUBLISH_DIR"

echo "Done: $APP_BUNDLE"

#!/bin/bash
# Publishes Scandalous.Avalonia as a macOS .app bundle.
# Usage: ./publish-mac.sh [osx-arm64|osx-x64]
# Default runtime: osx-arm64
#
# Signing: ad-hoc by default, which is enough for Gatekeeper to launch the app
# locally. Set CODESIGN_IDENTITY to a Developer ID to produce a distributable,
# notarizable bundle:
#   CODESIGN_IDENTITY="Developer ID Application: Name (TEAMID)" ./publish-mac.sh

set -euo pipefail

RUNTIME="${1:-osx-arm64}"
PROJECT="Scandalous.Avalonia/Scandalous.Avalonia.csproj"
PUBLISH_DIR="publish/mac-tmp"
APP_NAME="Scandalous"
APP_BUNDLE="publish/${APP_NAME}.app"
CODESIGN_IDENTITY="${CODESIGN_IDENTITY:--}"

VERSION="$(dotnet msbuild "$PROJECT" -getProperty:Version -nologo | tr -d '\r' | tail -n 1)"
if [ -z "$VERSION" ]; then
  echo "Could not resolve the Version property from $PROJECT" >&2
  exit 1
fi

echo "Publishing $APP_NAME $VERSION for $RUNTIME..."
dotnet publish "$PROJECT" -c Release -r "$RUNTIME" --self-contained true -o "$PUBLISH_DIR"

echo "Creating .app bundle..."
rm -rf "$APP_BUNDLE"
mkdir -p "$APP_BUNDLE/Contents/MacOS"
mkdir -p "$APP_BUNDLE/Contents/Resources"

cp -r "$PUBLISH_DIR/"* "$APP_BUNDLE/Contents/MacOS/"
cp "Scandalous.Avalonia/Info.plist" "$APP_BUNDLE/Contents/"
cp "Scandalous.Avalonia/Assets/Scandalous.icns" "$APP_BUNDLE/Contents/Resources/"
chmod +x "$APP_BUNDLE/Contents/MacOS/$APP_NAME"

rm -rf "$PUBLISH_DIR"

echo "Stamping bundle version $VERSION..."
/usr/libexec/PlistBuddy \
  -c "Set :CFBundleShortVersionString $VERSION" \
  -c "Set :CFBundleVersion $VERSION" \
  "$APP_BUNDLE/Contents/Info.plist"

# --deep is required because the self-contained publish ships its own native
# dylibs (Avalonia, NAPS2, Tesseract) that each need a signature.
#
# The hardened runtime and a secure timestamp are prerequisites for
# notarization, but neither is available to an ad-hoc signature, so they are
# applied only when a real identity is supplied.
SIGN_ARGS=(--force --deep --sign "$CODESIGN_IDENTITY")
if [ "$CODESIGN_IDENTITY" = "-" ]; then
  echo "Signing (ad-hoc; launches locally, not distributable)..."
  SIGN_ARGS+=(--timestamp=none)
else
  echo "Signing as $CODESIGN_IDENTITY..."
  SIGN_ARGS+=(
    --timestamp
    --options runtime
    --entitlements "Scandalous.Avalonia/Scandalous.entitlements"
  )
fi

codesign "${SIGN_ARGS[@]}" "$APP_BUNDLE"
codesign --verify --deep --strict "$APP_BUNDLE"

echo "Done: $APP_BUNDLE ($VERSION, $RUNTIME)"

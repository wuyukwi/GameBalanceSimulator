#!/usr/bin/env bash
set -e

PROJECT="src/GameBalanceSimulator/GameBalanceSimulator.csproj"
RUNTIME="${1:-win-x64}"
OUTPUT="${2:-publish}"

dotnet publish "$PROJECT" \
    -c Release \
    -r "$RUNTIME" \
    --self-contained true \
    -o "$OUTPUT" \
    /p:PublishSingleFile=true \
    /p:IncludeNativeLibrariesForSelfExtract=true

if [[ "$RUNTIME" == win-* ]]; then
    echo ""
    echo "Published successfully to: $OUTPUT/GameBalanceSimulator.exe"
else
    echo ""
    echo "Published successfully to: $OUTPUT/GameBalanceSimulator"
fi

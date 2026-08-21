#!/usr/bin/env bash
set -e

PROJECT="src/GameBalanceSimulator/GameBalanceSimulator.csproj"
dotnet run --project "$PROJECT"

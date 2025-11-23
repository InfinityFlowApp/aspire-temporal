#!/bin/bash

# Only run in remote environments
if [ "$CLAUDE_CODE_REMOTE" != "true" ]; then
  exit 0
fi

echo "Installing .NET SDK in remote environment..."

# Install .NET SDK using official Microsoft script
# This installs the latest .NET SDK
curl -sSL https://dot.net/v1/dotnet-install.sh -o dotnet-install.sh
bash dotnet-install.sh --channel 9.0
rm dotnet-install.sh

# Add dotnet to PATH for current session
export DOTNET_ROOT=$HOME/.dotnet
export PATH=$PATH:$DOTNET_ROOT:$DOTNET_ROOT/tools

# Persist environment variables for subsequent bash commands
if [ -n "$CLAUDE_ENV_FILE" ]; then
  echo "export DOTNET_ROOT=$HOME/.dotnet" >> "$CLAUDE_ENV_FILE"
  echo "export PATH=\$PATH:\$DOTNET_ROOT:\$DOTNET_ROOT/tools" >> "$CLAUDE_ENV_FILE"
fi

echo ".NET SDK installation complete"
dotnet --version

exit 0

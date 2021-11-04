# Example-Genesis-Plugins-UPM

## Introduction

This project demonstrates a way to develop a Genesis plugin as a Unity package.

## Genesis

This project use a modified version of Genesis
https://github.com/laicasaane/Genesis/tree/unity-package-plugin

## Requirements

- Unity 2021.2 and above
- OpenUPM

## Limitations

- UPM packages are currently built against .NET Framework 4 profile, even though the API Compatibility Level is set to .NET Standard 2.1. Some functionality of .NET Standard 2.1 should not be used, otherwise exceptions might be thrown when running Genesis.CLI process.
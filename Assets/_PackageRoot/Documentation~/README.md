# UBuilder - Unity command line builder

![npm](https://img.shields.io/npm/v/extensions.unity.ubuilder) [![openupm](https://img.shields.io/npm/v/extensions.unity.ubuilder?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/extensions.unity.ubuilder/) ![License](https://img.shields.io/github/license/IvanMurzak/UBuilder) [![Stand With Ukraine](https://raw.githubusercontent.com/vshymanskyy/StandWithUkraine/main/badges/StandWithUkraine.svg)](https://stand-with-ukraine.pp.ua)

Unity command line builder. Flexible setup through Environment variables in combination with command line variables. Well combined with Fastlane.

All operation systems are supported for running the build process:

- :white_check_mark: Windows
- :white_check_mark: MacOS
- :white_check_mark: Linux

Supported platforms:

- :white_check_mark: Standalone (Windows / Mac / Linux)
- :white_check_mark: iOS
- :white_check_mark: Android
- :yellow_circle: other (supported but not tested)

# How to install - Option 1 (RECOMMENDED)

- [Install OpenUPM-CLI](https://github.com/openupm/openupm-cli#installation)
- Open command line in Unity project folder
- `openupm add extensions.unity.ubuilder`

# How to install - Option 2

- Add this code to `/Packages/manifest.json`

```json
{
  "dependencies": {
    "extensions.unity.ubuilder": "1.2.0",
  },
  "scopedRegistries": [
    {
      "name": "Unity Extensions",
      "url": "https://registry.npmjs.org",
      "scopes": [
        "extensions.unity"
      ]
    }
  ]
}
```

# Variables

Any variable can be passed as argument in command line such as `output=./Builds/Destination` or it could be set as environment variable in command line or in operation system or even in docker.
In command line you can do this by this command

`SET output=./Builds/Destination` on Windows

`output=./Builds/Destination` on MacOS and Linux

## Global variables

| Variable           | Type    | Default         | Description                                                                                       |
| ------------------ | ------- | --------------- | ------------------------------------------------------------------------------------------------- |
| `output`           | string  | `null`          | absolute or relevant path to build destination                                                    |
| `buildNumber`      | integer | `timestamp`     | integer value of build number                                                                     |
| `buildVersion`     | string  | current version | build version, should contains only numbers and dots, last and first characters should be numbers |
| `developmentBuild` | boolean | current value   | true/false, enable or disable Unity project flag - Development Build                              |

## iOS related variables

| Variable                     | Type    | Default       | Description                         |
| ---------------------------- | ------- | ------------- | ----------------------------------- |
| `ios_SigningTeamId`          | string  | current value | Apple signing team ID               |
| `ios_EnableAutomaticSigning` | boolean | current value | true/false, automatic signing build |

## Android related variables

| Variable                    | Type    | Default       | Description                                                                |
| --------------------------- | ------- | ------------- | -------------------------------------------------------------------------- |
| `android_BUILD_APP_BUNDLE`  | string  | current value | true/false, true - compile .aab file for Google Play, false - compile .apk |
| `android_KEYSTORE_PATH`     | boolean | `null`        | [required] path to keystore                                                |
| `android_KEYSTORE_PASSWORD` | boolean | `null`        | [required] keystore password                                               |
| `android_KEYALIAS_NAME`     | boolean | `null`        | [required] key alias                                                       |
| `android_KEYALIAS_PASSWORD` | boolean | `null`        | [required] key alias password                                              |

# How to use in command line (MacOS & Linux)

> Tip: If you don't have Unity Pro license that allow you to run Unity in batchmode (no GUI), you can use Unity Personal license. In this case you should run Unity Editor with GUI and it will build project. Just need to remove `-batchmode -nographics` from command line.

### iOS

```sh
UnityEditor=/Applications/"Unity Editor"/2019.2.1f1/Unity.app/Contents/MacOS/Unity
"$UnityEditor" -projectPath ./ -logFile build.log -executeMethod UBuilder.CommandiOS.Build -quit -accept-apiupdate -quitTimeout 6000 -batchmode -nographics
```

### Android

```sh
UnityEditor=/Applications/"Unity Editor"/2019.2.1f1/Unity.app/Contents/MacOS/Unity
android_KEYSTORE_PATH=***************
android_KEYSTORE_PASSWORD=***************
android_KEYALIAS_NAME=***************
android_KEYALIAS_PASSWORD=***************

"$UnityEditor" -projectPath ./ -logFile build.log -executeMethod UBuilder.CommandAndroid.Build -quit -accept-apiupdate -quitTimeout 6000 -batchmode -nographics
```

### Other

```sh
UnityEditor=/Applications/"Unity Editor"/2019.2.1f1/Unity.app/Contents/MacOS/Unity
"$UnityEditor" -projectPath ./ -logFile build.log -executeMethod UBuilder.Command.Build -quit -accept-apiupdate -quitTimeout 6000 -batchmode -nographics
```

Or create .sh file and put the text inside. Ease way to start build process by single script.

# How to use in command line (Windows)

### Android

```shell
SET UnityEditor=C:/UnityEditor/Unity/2019.2.1f1/Editor/Unity.exe
SET android_BUILD_APP_BUNDLE=false
SET android_KEYSTORE_PATH=***************
SET android_KEYSTORE_PASSWORD=***************
SET android_KEYALIAS_NAME=***************
SET android_KEYALIAS_PASSWORD=***************

%UnityEditor% -projectPath ./ -logFile build.log -executeMethod UBuilder.CommandAndroid.Build -quit -accept-apiupdate -quitTimeout 6000 -batchmode -nographics
```

### Other

```shell
SET UnityEditor=C:/UnityEditor/Unity/2019.2.1f1/Editor/Unity.exe
%UnityEditor% -projectPath ./ -logFile build.log -executeMethod UBuilder.Command.Build -quit -accept-apiupdate -quitTimeout 6000 -batchmode -nographics
```

Or create .bat file and put the text inside. Double click the bat file. Ease way to start build process by single script

# Hot to use in code (any OS)

| Function                           | Description                                                                                 |
| ---------------------------------- | ------------------------------------------------------------------------------------------- |
| `UBuilder.CommandiOS.Build()`      | create XCode project (MacOS required)                                                       |
| `UBuilder.CommandAndroid.Build()`  | create APK (`android_BUILD_APP_BUNDLE=false`) or AAB (`android_BUILD_APP_BUNDLE=true`) file |
| `UBuilder.CommandAndroid.Export()` | create Android Studio project                                                               |
| `UBuilder.Command.Build()`         | build current (from ProjectSettings) platform build                                         |

# How to use in Unity Editor (codeless)

Press `File/UBuilder` and choose needed build options from the list.

![Build Options](https://imgur.com/PvBTMvu.png)

# Customize project before build

Before execute the UBuilder functions, prepare a project to build. You can do that by creating public static class with public static method. In that method you should prepare your project and call needed UBuilder function. In command line call your new public static function instead of UBuilder function.
Example:

```C#
public static class ProjectBuilder
{
    // customize package name (bundle id)
    [MenuItem("File/UBuilder/Build Android custom package")]
    public static void BuildiOSCustomPackage()
    {
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, "my.project.game");
        UBuilder.CommandiOS.Build();
    }
    
    // customize package name (bundle id)
    [MenuItem("File/UBuilder/Build Android+iOS custom package")]
    public static void BuildiOSAndroidCustomPackage()
    {
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, "my.project.game");
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "my.project.game");
        UBuilder.CommandiOS.Build();
        UBuilder.CommandAndroid.Build();
    }
}
```

```Bash
SET UnityEditor=C:/UnityEditor/Unity/2019.2.1f1/Editor/Unity.exe
%UnityEditor% -projectPath ./.. -logFile build.log -executeMethod BuildProject.BuildiOSCustomPackage -quit -accept-apiupdate -quitTimeout 6000 -batchmode -nographics output=Builds/iOS_Custom_Package
```

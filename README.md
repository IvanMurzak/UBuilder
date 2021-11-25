# UBuilder - Unity command line builder
![npm](https://img.shields.io/npm/v/extensions.unity.ubuilder) ![License](https://img.shields.io/github/license/IvanMurzak/UBuilder)

Unity command line builder. Flexible setup through Environment variables in combination with command line variables.

All operation systems are supported for running build process:
- :white_check_mark: Windows
- :white_check_mark: MacOS
- :white_check_mark: Linux

Supported platforms:
- :white_check_mark: Standalone (Desktop)
- :white_check_mark: iOS
- :white_check_mark: Android
- :yellow_circle: other (supported but not tested)



# How to install
- Add this code to <code>/Packages/manifest.json</code>
```json
{
  "dependencies": {
    "extensions.unity.ubuilder": "1.0.1",
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
Any variable can be passed as argument in command line such as <code>output=.\Builds\Destination</code> or it could be set as enviranment variable in command line or in operation system or even in docker. 
In command line you can do this by this command 

<code>SET output=.\Builds\Destination</code> on Windows

<code>output=./Builds\Destination</code> on MacOS and Linux

## Global variables
- <code>output</code> - absolute or relevant path to build destination
- <code>buildNumber</code> - integer value of build number
- <code>buildVersion</code> - build version, should contains only numbers and dots, last and first characters should be numbers
- <code>developmentBuild</code> - true/false, enable or disable Unity project flag - Development Build

## iOS related variables
- <code>ios_SigningTeamId</code> - Apple signing team ID
- <code>ios_EnableAutomaticSigning</code> - true/false, automatic signing build

## Android related variables
- <code>android_BUILD_APP_BUNDLE</code> - true/false, true - compile .aab file for Google Play, false - compile .apk
- <code>android_KEYSTORE_NAME</code> - [required] path to keystore
- <code>android_KEYSTORE_PASSWORD</code> - [required] keystore password
- <code>android_KEYALIAS_NAME</code> - [required] key alias
- <code>android_KEYALIAS_PASSWORD</code> - [required] key alias password




# How to use in command line (Windows)
### iOS
```shell
SET UnityEditor=C:\UnityEditor\Unity\2019.2.1f1\Editor\Unity.exe
%UnityEditor% -projectPath .\ -logFile build.log -executeMethod UBuilder.CommandiOS.Build -quit -batchmode -nographics
```
### Android
```shell
SET UnityEditor=C:\UnityEditor\Unity\2019.2.1f1\Editor\Unity.exe
SET android_BUILD_APP_BUNDLE=false
SET android_KEYSTORE_NAME=***************
SET android_KEYSTORE_PASSWORD=***************
SET android_KEYALIAS_NAME=***************
SET android_KEYALIAS_PASSWORD=***************

%UnityEditor% -projectPath .\ -logFile build.log -executeMethod UBuilder.CommandAndroid.Build -quit -batchmode -nographics
```
### Other
```shell
SET UnityEditor=C:\UnityEditor\Unity\2019.2.1f1\Editor\Unity.exe
%UnityEditor% -projectPath .\ -logFile build.log -executeMethod UBuilder.Command.Build -quit -batchmode -nographics
```
Or create .bat file and put the text inside. Double click the bat file. Ease way to start build process by single script



# How to use in command line (MacOS & Linux)
### iOS
```sh
UnityEditor=/Applications/"Unity Editor"/2019.2.1f1/Unity.app/Contents/MacOS/Unity
"$UnityEditor" -projectPath ./ -logFile build.log -executeMethod UBuilder.CommandiOS.Build -quit -batchmode -nographics
```
### Android
```sh
UnityEditor=/Applications/"Unity Editor"/2019.2.1f1/Unity.app/Contents/MacOS/Unity
android_KEYSTORE_NAME=***************
android_KEYSTORE_PASSWORD=***************
android_KEYALIAS_NAME=***************
android_KEYALIAS_PASSWORD=***************

"$UnityEditor" -projectPath ./ -logFile build.log -executeMethod UBuilder.CommandAndroid.Build -quit -batchmode -nographics
```
### Other
```sh
UnityEditor=/Applications/"Unity Editor"/2019.2.1f1/Unity.app/Contents/MacOS/Unity
"$UnityEditor" -projectPath ./ -logFile build.log -executeMethod UBuilder.Command.Build -quit -batchmode -nographics
```
Or create .sh file and put the text inside. Ease way to start build process by single script.
# Hot to use in code (any OS)

<code>UBuilder.CommandiOS.Build()</code> - create XCode project (MacOS required)

<code>UBuilder.CommandAndroid.Build()</code> - create APK (<code>android_BUILD_APP_BUNDLE=false</code>) or AAB (<code>android_BUILD_APP_BUNDLE=true</code>) file

<code>UBuilder.CommandAndroid.Export()</code> - create Android Studio project

<code>UBuilder.Command.Build()</code> - build current (from ProjectSettings) platform build



# How to use in Unity Editor (codeless)
Press <code>File/UBuilder</code> and choose needed build options from the list.

![Build Options](https://imgur.com/PvBTMvu.png)



# Customize project before build
Before execute the UBuilder functions, prepeare a project to build. You can do that by creating public static class with public static method. In that method you should prepeare your project and call needed UBuilder function. In command line call your new public static function instead of UBuilder function.
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
SET UnityEditor=C:\UnityEditor\Unity\2019.2.1f1\Editor\Unity.exe
%UnityEditor% -projectPath .\.. -logFile build.log -executeMethod BuildProject.BuildiOSCustomPackage -quit -batchmode -nographics output=Builds\iOS_Custom_Package
```

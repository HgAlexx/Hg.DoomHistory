---
#  Hg.DoomHistory has now been superseded by [Hg.SaveHistory](https://github.com/HgAlexx/Hg.SaveHistory)
---

# Hg.DoomHistory

This tool helps generate Doom saved game backup to practice Ultra-Nightmare and/or speedrunning.


## Features

- Hot keys
- Auto detect Doom saved games folder (will try its best)
- Manual backup
- Manual restore
- Manual delete
- Sort backup by slot, map and date
- Automatic backup on checkpoint
- Distinguish between real checkpoint and death checkpoint (on auto only)
- Can take screenshot on automatic backup (only if Doom is in windowed mode)
- Events Sound notifications
- Attach notes to saves
- Import existing saved game backup


## Installation

Just unzip and run executable!

(or you can download the source code, examine and compile it yourself)

## Setup
### Global settings

#### "Saved Games Folder"
Set the path to "Saved Games Folder" using "Browse" or "Auto Detect", to point to the Doom save folder.
Usually something like that:
C:\Users\\<username\>\Saved Games\id Software\DOOM\base\savegame.user\\<some big number\>

#### "Backup Folder"
Set the path to "Backup Folder", this is where the application will backup your saved games files.
You probably want to point to a empty folder.

#### You're done!

## Usage
### Slot configuration

- "Auto Backup": check this to enable auto backup for this slot
- "Include death": auto backup upon death
- "Screenshot": take a screenshot upon auto backup
- "Sound notification on auto backup": play a sound on successful or failed backup
- "Auto select last backup save": Highlight the backup save in the list view

> Note that each slot use its separated configuration !
> Be sure to enable auto backup for the slot you are using !

### Menu
#### File
- "Exit": close the application
#### Settings
- "Hot keys":
  - "Enabled": Active or deactive the hot keys
  - "Sound on restore": Play a sound on successful or failed restoration
  - "Assign hot keys": Open the hot keys settings window
- "Notification mode": this allow you to choose between messagebox or statusbar notification
- "Saves sort order": Sort list view by date ascending or descending
- "Screenshot quality": this allow you to choose between gif, jpg or png format
- "Clear settings": this will reset the global settings (folders, notification mode and screenshot quality)
#### Tools
- "Import existing backups": this allow you to import any saves game you already have into the application
- "Open debug console": for debug purpose only
#### Help
- "Check for update": this will check online for new release
- "About": Provide cuteness, and also some information about the application

## F.A.Q.

#### Why auto-backup is not working?

- Be sure you properly set both the "Saved Game Folder" and the "Backup Folder"
- Be sure that you checked the box for the slot you are playing on, this settings is per slot.
- Auto back can fail to start in rare occasion when you start a game on a empty slot, wait for the first checkpoint, restart the application and check the auto backup box again.

#### Why are my screenshots black or showing the Doom escape menu?

The screenshot feature works only if you play the game in windowed mode (with border) due to technical limitation.

#### Why are my screenshots blurry?

To improve the quality of the screenshots, decrease the quality on the settings "Motion blur" in Doom Video settings.

## About

~~This is not the prettiest source code in the world, I've made this software over the course of a week and there is still a lot of room for improvements.~~ I put some more time in and refactor and cleanup a lot of the code.

I got the idea while watching [IAmAllstin](https://www.twitch.tv/iamallstin) streaming doom. He was spending a lot of time copy/pasting save files to be able to pratice part of the game for his Ultra-Nightmare run.

Since I was also planning to do Doom on Ultra-Nightmare, I was going to need a way to back my save files too.

The first beta tester was [RedW4rr10r](https://www.twitch.tv/redw4rr10r) who found the first bug and made good improvements suggestions.

[x_ByteMe_x](https://www.twitch.tv/x_byteme_x) who also made good improvements suggestions (global hot keys).


# Version History

## v1.3.2

- \* Fix few issues with auto backup
- \+ Add more hotkeys
- \* Improve error reporting system (non-blocking)
- \* Code cleanup and Optimizations


## v1.3.1

- \* Fix a potential issue with auto backup
- \* Fix maps list: Lazarus was shown two times
- \+ Add optional error sound if an auto backup fail

## v1.3.0

- \+ New Feature: Hot Keys!
- \+ New Feature: Sound Notification
- \* Bug Fixes
- \* Lots of Improvements
- \* Code Refactoring and Cleanup

## v1.2.1

- \* Fix a issue with auto backup not starting if the save folder does not exists
- \* Other fixes
- \* Code Cleanup

## v1.2

- \+ Add check for update
- \+ Add screenshot quality settings
- \+ Add exception handler and a report form in case of error
- \* Add level number in from of map folder name
- \* Various bugs fixes and improvements

## v1.1

- \+ New feature: import existing backup folder

## v1.0

- \* First public release

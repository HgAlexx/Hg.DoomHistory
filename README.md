# Hg.DoomHistory

This tool helps generate Doom saved game backup to practice Ultra-Nightmare and/or speedrunning.

## Features

- Auto detect Doom saved games folder (will try its best)
- Manual backup
- Manual restore
- Manual delete
- Sort backup by slot, map and date
- Automatic backup on checkpoint
- Distinguish between real checkpoint and death checkpoint (only on auto)
- Take screenshot on automatic backup (only if Doom is in windowed mode)
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
C:\Users\<username>\Saved Games\id Software\DOOM\base\savegame.user\<some big number>

#### "Backup Folder"
Set the path to "Backup Folder", this is where the application will backup your saved games files.
You probably want to point to a empty folder.

#### You're done!

## Usage
### Slot configuration

- Auto Backup: check this to enable auto backup for this slot
- Include death: auto backup upon death
- Screenshot: take a screenshot upon auto backup

> Note that each slot use its separated configuration !
> Be sure to enable auto backup for the slot you are using !

### Menu
#### File
- "Exit": close the application
#### Tools
- "Import existing backups": this allow you to import any saves game you already have into the application
- "Notification mode": this allow you to choose between messagebox or statusbar notification
- "Screenshot quality": this allow you to choose between gif, jpg or png format
- "Clear settings": this will reset the global settings (folders, notification mode and screenshot quality)
- "Open debug console": for debug purpose only
#### Help
- "Check for update": this will check online for new release
- "About": Provide cuteness, and also some information about the application

## About

This is not the prettiest source code in the world, I've made this software over the course of a week and there is still a lot of room for improvements.

I got the idea while watching [IAmAllstin](https://www.twitch.tv/iamallstin) streaming doom. He was spending a lot of time copy/pasting save files to be able to pratice part of the game for his Ultra-Nightmare run.

Since I was also planning to do Doom on Ultra-Nightmare, I was going to need a way to back my save files too.

# Version History

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

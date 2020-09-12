# Retrogame-Handler

![](http://timeonline.se/RGHandler/images/TimeOnlineLogoV3.png)



Easy to use handler for Retro handheld consoles with built in FTP. 
I have done this project firstly for my own needs so please have patience. You use the program at your own risk.
If you have some free time and like to help with coding or testing or have sugestions, please contact me on [reddit](https://www.reddit.com/user/nikryd/)

### For manual go to the [WiKi](https://github.com/nikryden/Retrogame-Handler/wiki) (under construct)

### Pland Features
1. Save game images to the computer (planded for 1.0.1)
2. Save game meta info like (description, date of release and publisher) to text file (planded for 1.0.1).
3. Scraping images and data for Emulationstation (planded for 1.0.1).
4. Easy update of the program (planded for 1.0.1).
5. Autosync folders. Add or remove games in folder on PC and when connect to console update folder on console (planded for 1.0.1 maybe 1.0.2).

**If you have any features you like to se in the program please add a issue.**


### [v1.0.0 Production](https://github.com/nikryden/Retrogame-Handler/releases/tag/v1.0.0)

This is the production version.

1. Lots of bugs is fixed.
2. Add about page
3. Add link to wiki


### [v0.0.10.1-Beta Fix](https://github.com/nikryden/Retrogame-Handler/tree/0.0.10.1-beta)
### Warning! This is a Beta build and it is used at your own risk.
  Fix activation bug that some times prevent activation of the scraper.
  
### [v0.0.10-Beta Beter and improved features in the scraping control](https://github.com/nikryden/Retrogame-Handler/releases/tag/0.0.10-beta)

## Scraping ##

It use The games DB as database as source [THEGAMESDB](https://thegamesdb.net/)
1. Search is now more accurate. The database now have 66817 games with images in it (For my setup it found almost 95% at first try and the missing one had bad filenames so when clening up it found about 99%)
2. Posibility to skip searching for images that allready in the consoles selected games media folder.
3. Fix the error 500  when search.
4. Manual search for games images.(Right click in the result control on the game you like to manual search)
5. Rename the file in the scraper. (Right click in the result control on the game you like to rename)

[v0.0.9-Beta](https://github.com/nikryden/Retrogame-Handler/releases/tag/0.0.9-beta)
### Features in v0.0.9-beta version

### FTP ###
1. Upload/Download file/s or directories
2. Rename file or directory
3. Edit text files and save to console
4. Image previews 
5. .opk info (this is slow)


### Scrap images  ###
1. Using less memory
2. improved error handling 

### New log handler ###
1. Log viewer

[Please backup Existing Firmware](http://wagnerstechtalk.com/rg350tips/#Backup_Existing_Firmware)

[v0.0.8.33 Alfa version](https://github.com/nikryden/Retrogame-Handler/releases/tag/0.0.8.33-Alpha)

### Features in v0.0.8.33-Alfa version
 - [x] Lite weight game scraper for SimpleMenu.
 - [x] Some preformence boost.


[v0.0.7452.36755 Alfa version](https://github.com/nikryden/Retrogame-Handler/releases)

 ### Features in v0.0.7452.36755 Alfa version

- [x] Info about battery status
- [x] Activity icon with battery status
- [x] Shows info about firmware installed
- [x] FTP File manager(this version support file-tree view, preview of imagefiles, multi upload files to console and delete files on console)
- [x] Can connect via WiFi or direct to console via USB
- [x] Can have multiple Ftp Settings
- [x] Links to usefull pages

### Planed features for coming versions

- [X] Liteweight game scraper for SimpleMenu 
- [ ] Improve the error handling. 
- [ ] Fix buggs 
- [ ] Make it .net core so it works on Linux and Mac OS 

- [ ] More functions in the FTP File manager(move files on the console, download files to PC and drag and drop files. ) 
- [ ] Help to find and install application
- [ ] Help to find and install games
- [ ] Backup of the main sd card

- [ ] FTP Support for password

- [ ] More resource links
- [ ] Improved user experience
 
 ### Screenshots. 

![ScrapingWork](https://user-images.githubusercontent.com/7419588/85471994-8c8b4e00-b5b1-11ea-917f-3a50d658af49.png)
![ScrapingF](https://user-images.githubusercontent.com/7419588/85430577-96e12400-b580-11ea-959c-7e68f6454502.png)
![](http://timeonline.se/RGHandler/images/FTPExplorer.png)


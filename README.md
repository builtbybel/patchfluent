
<h1 align="center">
  <br>
  <a href="http://www.builtbybel.com"><img src="https://github.com/builtbybel/patchlady/blob/master/src/Patchlady/patchlady.ico" alt="Patchlady" width="200"></a>
  <br>
  Patchlady
  <br>
</h1>

<h3 align="center">🦄 Manual Windows 10 updates installer</h3>

<p align="center">
<a href="https://github.com/builtbybel/patchlady/releases/latest" target="_blank">
<img alt="Latest GitHub release" src="https://img.shields.io/github/release/builtbybel/patchlady.svg?style=flat-square" />
</a>
	
<a href="https://github.com/builtbybel/patchlady/releases" target="_blank">
<img alt="Downloads on GitHub" src="https://img.shields.io/github/downloads/builtbybel/patchlady/total.svg?style=flat-square" />
</a>

<a href="https://github.com/builtbybel/patchlady/commits/master">
<img src="https://img.shields.io/github/last-commit/builtbybel/patchlady.svg?style=flat-square&logo=github&logoColor=white"
alt="GitHub last commit">
<a href="https://github.com/builtbybel/patchlady/issues">
<img src="https://img.shields.io/github/issues-raw/builtbybel/patchlady.svg?style=flat-square&logo=github&logoColor=white"
alt="GitHub issues">   
  
</p>

<p align="center">
  <a href="#about">About</a> •
  <a href="#about">How-to</a> •
  <a href="#download">Download</a> •
  <a href="#credits">Credits</a>
</p>

![screenshot](https://github.com/builtbybel/patchlady/blob/master/assets/patchlady.png)

## About

I found by chance this cool project from [slavanap](https://github.com/slavanap) called **[Windows10ManualUpdate](https://github.com/slavanap/Windows10ManualUpdate)** a few weeks ago and decided to take a closer look at it. Finally I managed to have a first look at it today and created a 1:1 fork. First I took care of the name and named it **Patchlady**. As an alternative I had PatchPal in mind, but didn't want any trouble with Elon "PayPal" Musk ;).

You are welcome to take a closer look at it and support slavanaps project or this fork. I would be very sorry if the project goes under. It has definitely had to be better valued.

## How-to

Note, this method may not work properly in Windows 10 Home.

**Before running this app you must turn off automatic updates installation**. This could be done via Group Policy Object Editor MMC snap-in or via importing registry files into Windows Registry.

In order to do it via MMC:

- Press Win+R. Type mmc.exe. Hit Enter.
- Navigate menu File -> Add/Remove Snap-in. Select Group Policy Object Editor, click Add to add it to the list on the right. Click Finish, then OK.
- On the left navigate to Local Computer Policy -> Computer Configuration -> Administrative Templates -> Windows Components -> Windows Update. Select that folder.
- In the list select Configure Automatic Updates policy and set it to Disabled. This setting will disable automatic updates check and installation, but if you navigate to Updates in Windows Setting you'll implicitly launch automatic updates check and installation.
- Change other options in MMC, if you really know what you're doing.

Another approach is the Registry files (for downloading **right-click and save one of the .reg files**):

- [Disable automatic updates policy.reg](https://github.com/builtbybel/patchlady/raw/master/src/Patchlady/Disable%20automatic%20updates%20policy.reg)
- [Disable automatic updates & no restarts policies.reg](https://github.com/builtbybel/patchlady/raw/master/src/Patchlady/Disable%20automatic%20updates%20%26%20no%20restarts%20policies.reg)


## Download

- (Latest release) [Download](https://github.com/builtbybel/patchlady/releases)
- (Source Code) [Download](https://github.com/builtbybel/patchlady/releases) 


## Credits

This project is based upon 

- https://github.com/slavanap/Windows10ManualUpdate

This software uses the following packages:

- [Icons8.de](https://icons8.de/)

---

> [builtbybel.com](https://www.builtbybel.com) &nbsp;&middot;&nbsp;
> GitHub [@builtbybel](https://github.com/builtbybel) &nbsp;&middot;&nbsp;
> Twitter [@builtbybel](https://twitter.com/builtbybel)

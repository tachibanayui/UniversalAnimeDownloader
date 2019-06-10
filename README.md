<h1 align="center">Universal Anime Downloader</h1>

<p align="center">
    <img src="https://github.com/quangaming2929/UADInstaller/raw/master/Src/UADInstaller/UADInstaller/Resources/MediumIconUniversalAnimeDownloader.png">
</p>

Table of Content
----------------

1. [What is Universal Anime Downloader?](#what-is-universal-anime-downloader)
2. [Features](#features)
3. [Install](#install)
4. [Build from source](#build-from-source)
5. [How to use UAD](#how-to-use-uad)
    - [Quick guide](#quick-guide)
    - [Detailed guide & videos](#detailed-guide--videos)
6. [Contributing](#contributing)
7. [Coming up in the next release](#coming-up-in-the-next-release)

What is Universal Anime Downloader?
-----------------------------------

Universal Anime Downloader (UAD) is an anime downloader & extractor. It fetches
any anime series of interest and displays it to the user. UAD is meant to make
searching and downloading anime series easier. In addition, the program's
built-in video player has a lot of unique features that can enhance the user's
experience.

![Screenshot 1](Assets/Screenshot1.png)  
![Screenshot 2](Assets/Screenshot2.png)  
![Screenshot 3](Assets/Screenshot3.png)

Features
--------

- Download a complete series of anime with a few clicks
- Search anime series by name or genre
- Anime's information (description, thumbnails, ...) is saved after download
- Download and watch anime series without a web browser (saves computer memory :+1:)
- No advertisement
- Draw on the playback screen of the video player
- Segmented download (which results in better download speed)
- Material Design UI
- Highly customizable
- Sneaky watch :eyes: <!-- TODO: Write more about this -->
- Selective download for anime episodes
- Watch online
- Notify and auto-download new episodes in the Anime Library
- Official API for the custom extractor to be based on
- Suggest anime series based on previous downloads
- Show featured anime series
- Custom playlists

Install
-------

[latest]: https://github.com/quangaming2929/UniversalAnimeDownloader/releases/latest

**Prerequisites**:

- Operating system: Windows 7 or later
- .NET framework version 4.7.2 or later

**Instructions**:

1. Download the latest version [here][latest] (choose the `UADInstaller1.0.exe`
file).
2. Run the EXE file. If Windows Smartscreen displays a warning, proceed by
clicking "Learn more" and then "Run anyway".
3. Click "Install" to install Universal Anime Downloader.

Build from source
-----------------

> **Note**: This section is only for Visual Studio users. If you use another
code editor or IDE, please consult the Internet on how to reproduce these steps
in your favorite program.

**Prerequisites**:

- Operating system: Windows 7 or later
- A text editor or an IDE with support for .NET framework (Visual Studio 2017
and Visual Studio 2019 are recommended)
- .NET desktop development workload

**Instructions**:

1. Clone this repository.
2. Open the repository as a solution using Visual Studio.
3. Press F5 or click Start to build Universal Anime Downloader.

How to use UAD
--------------

### Quick Guide

1. Search for anime series in the _All Anime_ page. Then download the interested
anime series by clicking on the anime "card", and select the episodes you
want to download by clicking on the corresponding check boxes, and then click
_Download all_.

![](Assets/QuickGuide1.png)  
![](Assets/QuickGuide2.png)

2. All downloaded anime series are located in the _My Anime Library_ section,
displayed using rectangular cards. To watch one, just click the play button.

![](Assets/QuickGuide3.png)  
![](Assets/QuickGuide4.png)

### Detailed guide & videos

> **Note**: This is meant for UAD 0.8.0. I will release an updated video
tutorial for UAD 0.9.0 soon. Their functionalities are similar, but the
user interface in UAD 0.9.0 differs from that of UAD 0.8.0.

I've created a video instruction for UAD, so [check it out][en-guide]. A similar
video but is intended for Vietnamese users can be found [here][vi-guide].

Assets used to make the videos are available publicly [here][materials].

[en-guide]: https://drive.google.com/open?id=1-8O5G7YrnI_KLZiXz6BZ0F5LoKYYVSsG
[vi-guide]: https://drive.google.com/open?id=1cwXjiAtqJMBDYsLpmXqHf-o8mZchk2K0
[materials]: https://drive.google.com/open?id=1eHobBKnt9ruD1-Cqc-kKu2RLc8qq6cJT

Contributing
------------

[mail]: uad.apiservices@gmail.com

Currently, I'm working alone on this project. I'm working on major updates for
UAD and the amount of work to get it done is immense, so I'd appreciate your
contributions a lot. Pull requests are welcome. If you'd like to contact me
personally to discuss about the project, [send an e-mail][mail].

If you want to contribute to the source, there are a few things to note:
- **UI development**: The MVVM pattern is implemented in UAD and you must
follow and keep this pattern. If you don't want to use the MVVM
pattern, please specifically state why in your pull request.
- **API development**: Make sure that the interfaces (currently
`UADAPI.IQueryAnimeSeries` and `UADAPI.IAnimeSeriesManager`) aren't
significantly modified.

Coming up in the next release
-----------------------------

- Built-in interactive manual
- HTTP Live Streaming (HLS) implementation
- _Mod Gallery_ section, where you can download mods for UAD
- Support for websites that implement CloudFlare

You can request a new feature by [opening an issue][issues].

[issues]: (https://github.com/quangaming2929/UniversalAnimeDownloader/issues)

<!-- ???

License
-------

-->

<h1 align="center">Universal Anime Downloader</h1>

<!--
    TODO: Insert program's logo, prominent program screenshot, or prominent
    showcase screenshots here.
-->
<img src="https://github.com/quangaming2929/UADInstaller/raw/master/Src/UADInstaller/UADInstaller/Resources/MediumIconUniversalAnimeDownloader.png" width="256" height="256">

What is Universal Anime Downloader?
-----------------------------------

Universal Anime Downloader (UAD) is an anime downloader & extractor tool. It
fetches any anime film of interest and displays it to the user. UAD is meant to
make searching and downloading anime films easier. In addition, the program's
built-in video player has a lot of unique features to enhance the user's
experience.

<!--
    TODO: Insert 1 or 2 program screenshots here.

    "A picture worth a thousand words. READMEs that don't have screenshots are
    the most boring READMEs ever."
        ~ N. H. Duong, https://github.com/dungwinux/helectron/issues/5
-->

<img src="https://github.com/quangaming2929/UniversalAnimeDownloader/raw/master/Assets/Screenshot1.png" >

<img src="https://github.com/quangaming2929/UniversalAnimeDownloader/raw/master/Assets/Screenshot2.png" >

<img src="https://github.com/quangaming2929/UniversalAnimeDownloader/raw/master/Assets/Screenshot3.png" >

Features
--------

- Download a complete series of anime with a few clicks
- Search anime films by name or genre
- Anime's information (description, thumbnails, ...) is saved after download
- Download and watch anime films without a web browser (saves computer memory :+1:)
- No advertisement
- Draw on the playback screen of the video player
- Segmented download (which results in better download speed)
- Material Design UI
- Highly customizable
- Sneaky watch :eyes: <!-- Write about this in e.g. a wiki page and drop a link here -->
- Selective download for anime episodes
- Watch online
- Notify and auto-download new episode in the Anime Library when one is available
- Official API for the custom extractor to be based on
- Suggest anime films based on previous downloads
- Show featured anime films
- Custom playlist

Gettting Started
--------

<h3>Prerequisites</h3>
<h4>For installing UAD </h4>
<ul>
    <li>Window 7, 8, 8.1, 10</li>
    <li>.NET Framework <b>4.7.2</b> or newer</li>
</ul>
<h4>For build UAD on your own </h4>
<ul>
    <li>Window 7, 8, 8.1, 10</li>
    <li>A code editor, IDE support .NET Framework, Visual Studio 2017, 2019 is recommended</li>
    <li>.NET desktop development workload </li>
</ul>
<h3> Installation Guide: </h3>
<ol>
    <li>Download the latest version <a href="https://github.com/quangaming2929/UniversalAnimeDownloader/releases/latest
"> here</a> and choose the first file</li>
    <li>Open the file, sometimes window smartscreen will warn you that we can't verify this file but that ok because we don't have enough reputation to verify our app. Click "Learn more" and Click "Run anyway"</li>
    <li>Click install to install Universal Anime Downloader</li>
</ol>

<h3>Build guide: </h3>
<h4> Note: this section only for Visual Studio users, for Rider or other code editors, IDEs please read look up on the internet how to these step in your code editors, IDEs</h4>
<h4> If you pay attention, you may wonder why you need to remove the project and readd the prebuilt one for <b>SegmentedDownloader</b>. This is due to the fact that I moded these libraries to allow me to add custom header when downloading, so I place this project outside of Git repos folder. When you clone this repos, these projects will failed to load</h4>
<ol>
    <li>Clone this repos using our web browser, GitBash or GitHub desktop app</li>
    <li>Open the solution using Visual Studio </li>
    <li>There might some projects which not reference correctly with the UI projects, to solve this, delete the reference and readding them or redownload them using Nuget Package Manager </li>
    <li>Remove the reference to <b>SegmentedDownloader.Core</b> and <b>SegmentedDownloader.Protocol</b> from project <b>UniversalAnimeDownloader</b> and Readd the prebuild .dll of them when you download UAD</li>
    <li>Remove project <b>SegmentedDownloader.Core</b> and <b>SegmentedDownloader.Protocol</b> from the solution</li>
    <li>Press "F5" or click "Start" to build Universal Anime Downloader</li>
</ol>
    


How to use it
-------------

### Quick Guide

1. Search for anime films in the _All Anime_ page. Then download the interested 
anime film by clicking the click on the anime card, select the episodes you want
to download using the checkbox or textbox then click "_Download all_".  

<img src="https://github.com/quangaming2929/UniversalAnimeDownloader/raw/master/Assets/QuickGuide1.png"/>
<img src="https://github.com/quangaming2929/UniversalAnimeDownloader/raw/master/Assets/QuickGuide2.png"/>

2. All downloaded anime films/series are located inside the _My Anime Library_
section, displayed using rectangular cards. To watch one:  
    1. Click the orange play button located on its card  
    2. Click the orange play button of the interested episode    
    
<img src="https://github.com/quangaming2929/UniversalAnimeDownloader/raw/master/Assets/QuickGuide3.png"/>
<img src="https://github.com/quangaming2929/UniversalAnimeDownloader/raw/master/Assets/QuickGuide4.png"/>

### Detailed guide / Video Tutorials  

>This is meant for v0.8.0  
>I've created a video instruction for UAD, so [check it out][en-guide]. A similar
>video but is intended for Vietnamese users can be found [here][vi-guide].
>
>Assets used to make the videos are available publicly [here][materials].  

<h4>All functionality are simillar for v0.9.0 but difference UIs, new feature will be covered in Video Tutorials</h4>

[en-guide]: https://drive.google.com/open?id=1-8O5G7YrnI_KLZiXz6BZ0F5LoKYYVSsG
[vi-guide]: https://drive.google.com/open?id=1cwXjiAtqJMBDYsLpmXqHf-o8mZchk2K0
[materials]: https://drive.google.com/open?id=1eHobBKnt9ruD1-Cqc-kKu2RLc8qq6cJT

Contributing
------------

Currently, I'm still working alone on this project. I'm working on major updates
for UAD and the amount of work to get it done is immense, so I'd appreciate your
contributions a lot.

Coming up in the next release
-----------------------------
- Built-in interactive manual
- Implementing a wat to play HLS inside the app window and download them without change the code base too much
- Add **Mod gallery** do download and install automatically (If we have people want to make mods for UAD to contribute this project)


You can request a new feature [here](https://github.com/quangaming2929/UniversalAnimeDownloader/issues).

# Development History - Ver 1.0.2
Hello!

Whalesong Park is a playable installation that exists physically in the real world. It’s a unique control and display system that has the potential to inspire some really unique gameplay experiences.
The initial version was developed by [Konglomerate Games](https://konglomerate.games/) in Dundee; friends and alumni of Abertay University.
Some time later the project was taken on for some extra development by an (at the time recent) graduate [Ewan Fisher](https://ewanjams.com/) (of Dare Academy’s [“For Hexposure”](https://yellow-crow.itch.io/for-hexposure)) and myself (Gaz Robinson).

All development has happened in tight time frames and tight budgets, so while we have hoped to open it up for others to build games, the legacy code would have made it *very* difficult for anyone to dive into.
It’s hoped with this update by me, the project is in a place where everything is mostly intuitive and you can just focus on building something fun.
This documentation will outline key things to bare in mind, how to get started, and some top tips.

The refactoring is by no means complete or user friendly at this point in time. So please do [get in touch](mailto:g.robinson@abertay.ac.uk) if you have any questions about how specific things work.
This ReadMe is a little too long right now, so I'll move this into a Wiki eventually.

Happy developing!
- Gaz

# Requirements
The project has recently been updated to Unity version [2023.2.20f1](https://unity.com/releases/editor/whats-new/2023.2.20) which should match the version that is installed on the Abertay University machines (circa Session 24-25). It may work in other versions, but this is not supported.

# Game Overview
## The System
[Whalesong Park](https://scottishgames.net/2021/11/09/new-games-light-up-dundees-wale-song-park/) is a physical game installation based in Dundee, Scotland [next to the V&A Dundee museum](https://maps.app.goo.gl/KMAJ5iX5zMvhJqhDA).

It is a cuboid obelisk of glass and steel comprised of four screens (one on each side) and four steel plinths that house the control mechanisms.
Each control plinth has four buttons: Four central directional buttons and two Action buttons, one on each side.

Abertay University, also based in Dundee, has a functional recreation of the installation for development purposes.

## Game Flow
The **Game** is a collection of **Minigames** for 1-4 players.
After booting up, the Game waits for 1 or more players to join by pressing a buttons. After this, the game loads the first Minigame in the queue and plays the tutorial screen. Players can join up until the end of the tutorial.
Players work together to play the Minigames and win more time to play.
When a **Minigame** is over, each player earns an amount of time that is added to the clock at the top of the screen.
Then the **Game** loads the next **Minigame**, and this repeats until the players run out of time.
## Design Considerations
-	Whalesong Park has four screens with a control set-up for each one. As such, a Minigame will *probably* be something that is playable by 1-4 players. Avoid ***requiring*** more than one player.
-	Players have four directional buttons and two action buttons. The directions aren’t *quite* like a d-pad and as such can be tricky to make quick moves. Play the installation to get a feel for it before diving into designing your twitchy bullet hell shmup.
-	Minigames can either have an objective to complete or can continue until in minigame timer is up. If the Minigame has an objective, the timer is usually the fail state instead.
	-	Failing a Minigame will not end the overall Game, it will simply not extend the Game Timer.
-	Minigames should reward time for completing them successfully. 4 players doing really well at a Minigame should give them *at least* the time back that they spent playing the Minigame.
## Development Considerations
- Each "Screen" of the system is really just one quarter of the render target. Take your monitor and divide it into four - that's all there is to it!
- MinigameBase (see below) has a `Start()` and `Update()` function already. If you want to add a `Start()` or `Update()` to your derived Minigame class you should:
	- Instead use the `OnResetGame()` and `OnUpdate()` overrides to add your functionality.
 	- Or, ensure you call `base.Start()` and `base.Update()`

# Creating a game for Whalesong Park
For clarity: 
- The **Whalesong Park System** (WSP System) is the installation + controls which is capable of running games.
- The **Whalesong Park Game** (WSP Game) is the game which is currently running on the **WSP System** in Dundee.

If you want to make a game for a Whalesong Park system there are, at this time, two main approaches.
- Build a **Minigame** that fits into the current **WSP Game** that is running on the **WSP System**.
- Build your own game that utilises the controls and display of the **WSP System**, but is *not* part of the **WSP Game**.

Building a Minigame is slightly more complex because whatever you build will have to play nice with all the various complexities of the current Whalesong Park Game and the visual style will have to be consistent.

Building your own game is simpler, especially for prototyping, but is unlikely to be used on the real-world installation at this time.

## Making a WSP Minigame
This will require to to align your work with the current landscape of the Whalesong Park Unity project. This is improving every time I get a chance to refactor things, but it is not yet a simple process.
### Project Outline
The project is constructed within a single scene: **DevScene**.

Each minigame consists of a parent GameObject and as many game objects as you want as a child of the parent. With any luck, you should be able to focus your efforts on the minigame and not have to worry too much about what’s going on around it.
#### Key Objects in Scene
-	**Main Camera**
	-	The camera that renders the scene. It’s not really recommended to use other cameras.
	-	The Post Processing object controls the bloom and should be left alone if you want your game to look consistent with other projects.
-	**GameManager**
	-	This object controls the overall logic of the game, the program flow between states, and wrangles all the various managers that have control over specific functionality.
	-	Here you can control things like the overall game time and the colour of the players.
	-	The GameAPI object is a class that can be called from anywhere within the code. It is very handy if you need access to something, so make use of it.
-	**Minigames**
	-	This parent object keeps a hold of and manages all of the Minigames. It is in charge of loading them and unloading them when complete.
	-	Any top level child of the Minigames object should be a Minigame
		-	If you expand the children, you should see two games: Simple Example Game and YOUR GAME HERE
-	**Render System**
	-	This is all the various canvases and layers that build the visuals of the game engine. You shouldn’t need to touch anything in here as they control program flow like start up, initial tutorial, idling, and credits.
-	**Render Scenes**
	-	This contains a few specific camera render tricks that create effects like the transitions you see in game.
-	**Unused**
	-	Some stuff that I don’t think is used but might be important
	-	There’s also another example minigame here that isn’t totally compatible with the current tutorial flow. Game dev!

### Creating a Minigame
To create your minigame modify the **YOUR GAME HERE** gameobject as you like, or create your own minigame.
The gameobject must have a component attached that derives from the base class `MinigameBase`. If this isn’t the case, the Game System won’t see it as a Minigame. There is a file called **“MyWhaleMinigame.cs”** already attached to the **YOUR GAME HERE** object as an example. Feel free rename and use this!
The `MinigameBase` has a few functions that you must implement so that it knows what to do when loading your game. These are already minimally implemented in **MyWhaleMinigame.cs**.
The minigame must have, as a child, a tutorial object with a `TutorialController` script attached. In its simplest form, the tutorial can be a static image that is displayed for a number of seconds as defined in the `TutorialController`.
As long as the Minigame is a child of the “Minigames” object, and has a `MinigameBase` derived component attached, then it will be loaded into the cycle of minigames. The games are played from top to bottom.

Look at the Simple Example Game to see a fully implemented game example. It makes pretty good use of some assorted functionality from across the project that you could use to enhance your game.

## Making your own game for the WSP System
If you simply want to make a game which is *not* part of the WSP Game, but is playable on the WSP System, your life will be much easier!
Make a new Unity Scene and add the `Input Manager` and `PlayerAudioManager` prefabs into the scene. Now build the game as you would if it were any normal Unity Game (played across four sections of the screen), but see below for how to check for input and to play audio.

# How To Code
## Input
By default, input is supplied to the Minigame baseclass through the virtual functions:
- `OnPrimaryFire(int playerIndex);`
- `OnSecondaryFire(int playerIndex);`
- `OnDirectionalInput(int playerIndex, Vector2 direction);`

**If** you don't know what a virtual function or a base class is, then you will probably want to use the below (assuming you have a slightly newer version of the project (from 08/10/2024)). It uses a format much more similar to the built-in Unity (Old) Input system.

- `WhalesongInput.GetButton(int PlayerID, WhaleButton btn)`
	- Is the player no. `PlayerID` currently pressing the button `btn`?
- `WhalesongInput.GetButtonDown(int PlayerID, WhaleButton btn)`
	- Did the player no. `PlayerID` *press* the button `btn` this frame?
- `WhalesongInput.GetButtonUp(int PlayerID, WhaleButton btn)`
	- Did the player no. `PlayerID` *release* the button `btn` this frame?

## Audio
- `WhalesongAudio.PlayOneShot(int playerIndex, AudioClip clip, float volume = 1.0f, float pitch = 1.0f)`
	- Play the sound `clip` at `volume` and `pitch` through Player `playerIndex`'s speaker.
- `WhalesongAudio.PlayGlobalOneShot(AudioClip clip, float volume = 1.0f, float pitch = 1.0f)`
	- Play the sound `clip` at `volume` and `pitch` for all player speakers.
# Building Whalesong Park
The Whalesong Park machine is a PC (yay) that needs certain files in certain places to work (boo) these files should be created on build (yay).
To build Whalesong Park open the Build Settings in Unity (Ctrl + Shift + B or File -> Build Settings).
Ensure that your game scene (**DevScene** by default) is included in the build and click **Build**.
You'll be prompted to choose or create a build folder. It is ***highly*** recommended that you name this folder so that it is identifiable as **A)** A Whalesong Park game and **B)** Your particular version of the game.

For example: **WhalesongPark_GazGame**

When the game builds it will create a few files in the executable folder. You might not need them now, but you should know what they are for when they are referred to later on.
- **Buttons.txt** is a text file used for configuring the button input on the Whalesong Machine and the Demo Machine. It is unlikely you will need to change this.
	- If there is a **Buttons.txt** file in the folder *above* your executable, it will be loaded *instead* of the one in the .exe folder, for reasons that are explained down below.
- **RunBorderless.bat** will launch the game in a borderless window. You might want to use this when testing on your own machine.
- **RunOnTVSetup.bat** will launch the game in a borderless window at the resolution required to work on the TV Demo station in Abertay.
- **RunOnWhalesongSetup.bat** will launch the game in a borderless window at the resolution required to work on the actual Whalesong Machine.

## Running WSP on the TV Demo Machine
Within Abertay University's Kydd Building is a functional recreation of the Whalesong Park Machine using four TVs.
On the machine's Desktop there is a folder called **Whalesong Builds** or similar. It should contain a `Buttons.txt` file and an assortment of folders representing different Whalesong Park builds.
The `Buttons.txt` file has been set up to ensure that the game correctly interprets the signals from the WSP controller, so **do not change it**! This means that all builds on the PC can share the same **correct** `Buttons.txt` file, rather than individually making your own.

Copy your build folder to the WhalesongBuilds folder. The hierarchy should look like this:
 > Desktop/Whalesong Builds/YOUR WHALESONG BUILD FOLDER/WhalesongPark.exe

If this is correct, you can launch the game by double-clicking `RunOnTVSetup.bat`. If you simply run the `WhalesongPark.exe` file, it will not correctly initialise the resolution.

# Miscellaneous
I hope to continue to update this in future. If there are any useful features you would like to see added to the core WSP Template, then you can contact me and I'll add it to the list.

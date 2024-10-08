#Development History
Hello!
Whalesong Park is a playable installation that exists physically in the real world. It’s a unique control and display system that has the potential to inspire some really unique gameplay experiences.
The initial version was developed by Konglomerate Games in Dundee; friends and alumni of Abertay University.
Some time later the project was taken on for some extra development by an (at the time recent) graduate Ewan Fisher (of Dare Academy’s “For Exposure”) and myself (Gaz Robinson).
All development has happened in tight time frames and tight budgets, so while we have wanted to open it up for others to build games, the legacy code would have made it very difficult for anyone to dive into.
It’s hoped with this update by me, the project is in a place where everything is mostly intuitive and you can just focus on building something fun.
This documentation will outline key things to bare in mind, how to get started, and some top tips.
Happy developing!
-	Gaz

#Requirements
The project has recently been updated to Unity version 2023.2.20f1 which should match the version that is installed on the University machines. It may work in other versions, but this is not supported.

#Project Outline
The project is constructed within a single scene: DevScene.
Each minigame consists of a parent GameObject and as many game objects as you want as a child of the parent. With any luck, you should be able to focus your efforts on the minigame and not have to worry too much about what’s going on around it.
##Key Objects in Scene
-	Main Camera
	-	The camera that renders the scene. It’s not really recommended to use other cameras.
	-	The Post Processing object controls the bloom and should be left alone if you want your game to look consistent with other projects.
-	GameManager
	-	This object controls the overall logic of the game, the program flow between states, and wrangles all the various managers that have control over specific functionality.
	-	Here you can control things like the overall game time and the colour of the players.
	-	The GameAPI object is a class that can be called from anywhere within the code. It is very handy if you need access to something, so make use of it.
-	Minigames
	-	This parent object keeps a hold of and manages all of the Minigames. It is in charge of loading them and unloading them when complete.
	-	Any top level child of the Minigames object should be a Minigame
		-	If you expand the children, you should see two games: Simple Example Game and YOUR GAME HERE
-	Render System 
	-	This is all the various canvases and layers that build the visuals of the game engine. You shouldn’t need to touch anything in here as they control program flow like start up, initial tutorial, idling, and credits.
-	Render Scenes
	-	This contains a few specific camera render tricks that create effects like the transitions you see in game.
-	Unused
	-	Some stuff that I don’t think is used but might be important
	-	There’s also another example minigame here that isn’t totally compatible with the current tutorial flow. Game dev!
#Game Overview
##Game Flow
Whalesong Park is a collection of minigames for 1-4 players.
After booting up, the game waits for 1 or more players to join by pressing a buttons. After this, the game loads the first Minigame in the queue and plays the tutorial screen. Players can join up until the end of the tutorial.
Players work together to play the game and win more time to play.
When a Minigame is over, each player earns an amount of time that is added to the clock at the top of the screen.
Then the Game loads the next minigame, and this repeats until the players run out of time.
##Design Considerations
-	Whalesong Park has four screens with a control set-up for each one. As such, your game will probably be something that is playable by 1-4 players. Avoid requiring more than one person.
-	Players have four directional buttons and two action buttons. The directions aren’t like a d-pad and can be tricky to make quick moves. Play the installation to get a feel for it before diving into designing your twitchy bullet hell shump.
-	Games can either have an objective to complete or can continue until in minigame timer is up. If the game has an objective, the timer is usually the fail state instead.
-	Games should reward time for completing them successfully. 4 players doing really well at a game should give them at least the time back that they spent playing the game.
#Getting Started
##Creating a Minigame
To create your minigame modify the YOUR GAME HERE gameobject as you like, or create your own minigame.
The gameobject must have a component attached that derives from the base class MinigameBase. If this isn’t the case, the Game System won’t see it as a Minigame. There is a file called “MyWhaleMinigame.cs” already attached to the YOUR GAME HERE object as an example. Feel free rename and use this!
The MinigameBase has a few functions that you must implement so that it knows what to do when loading your game. These are already minimally implemented in MyWhaleMinigame.cs.
The minigame must have, as a child, a tutorial object with a TutorialController script attached. In its simplest form, the tutorial can be a static image that is displayed for a number of seconds as defined in the TutorialController.
As long as the Minigame is a child of the “Minigames” object, and has a MinigameBase derived component attached, then it will be loaded into the cycle of minigames. The games are played from top to bottom.

Look at the Simple Example Game to see a fully implemented game example. It makes pretty good use of some assorted functionality from across the project that you could use to enhance your game.
#How To
##Input
By default, input is supplied to the Minigame baseclass through the virtual functions:
- OnPrimaryFire(int playerIndex);
- OnSecondaryFire(int playerIndex);
- OnDirectionalInput(int playerIndex, Vector2 direction);

**IF** you don't know what a virtual function or a base class is, then you will probably want to use the below (assuming you have a slightly newer version of the project (from 08/10/2024)). It uses a format much more similar to the built-in Unity (Old) Input system.

- WhalesongInput.GetButton(int PlayerID, WhaleButton btn)
	- Is the player no. **PlayerID** currently pressing the button **btn**?
- WhalesongInput.GetButtonDown(int PlayerID, WhaleButton btn)
	- Did the player no. **PlayerID** *press* the button **btn** this frame?
- WhalesongInput.GetButtonUp(int PlayerID, WhaleButton btn)
	- Did the player no. **PlayerID** *release* the button **btn** this frame?

##Audio
- WhalesongAudio.PlayOneShot(int playerIndex, AudioClip clip, float volume = 1.0f, float pitch = 1.0f)
	- Play the sound **clip** at **volume** and **pitch** through Player <**playerIndex**>'s speaker.
- WhalesongAudio.PlayGlobalOneShot(AudioClip clip, float volume = 1.0f, float pitch = 1.0f)
	- Play the sound **clip** at **volume** and **pitch** for all player speakers.
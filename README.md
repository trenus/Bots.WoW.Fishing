# Bots.WoW.Fishing
Clean, simple, and stand alone WoW fishbot I made for myself after getting tired of all the clunky other ones. No strange DLLs. No audio stuff. No code or memory injection stuff. It works just like Windows would if you were doing it yourself.

I am really only publishing this in the interest of making available all the pertinent knowledge I had to collect over a 6 month period to make something useful for multiple games (Yeah, I have modified versions of this bot for other games. No I won't share them. This has everything you need to get started making your own bots.). I have successfully used this for several years and expansions now and have never had to change it because of something WoW changed. So, now that I am comfortable saying it is stable and I don't play but every few months anymore, I am releasing it for everyone to tinker with.

It uses the Windows API (user32.dll) to execute the same commands your mouse or keyboard does. Just run WoW in windowed mode (I use "windowed - full screen" from the graphics options) and put your fishing skill on keybind #1. If you are at all familiar with code you can tweak to suit your needs.

I originally made this in Visual Studio Express (2010) but have since used VS 2016 Enterprise on it. It should still compile just fine in the older versions though since my changes from the original have been superficial.

I have included some default cursor images in the images folder that should be placed next to the exe when it runs. Otherwise you need to use the menu option to set your default cursors (click the menu item, the game window will be activated, highlight your fishing bobber, and after 3 seconds the bot will save your current cursor as the one it will seek out when running - this overwrites the default cursors saved next to the exe). If you attenpt to run this bot without any default cursors defined then it assumes the first cursor change from the default WoW one is the fishing bobber (not a problem when nothing else moves on your screen, but problematic when anything moves on your screen and causes your cursor to change - like people or monsters).

If you use the Refresh Lure menu option then it will apply whatever is bound to your #2 key every ten minutes.

Adding more options for things (like rafts, hearthstones, or booze) to use is as simple as emulating the way the lure was done in code. I just don't care enough about the extras since I only use this to skill up to max and then do everything the manual way after that.

--------------------------------------------------------------------------------------------

#### Caveats

The big caveat for this whole bot is that it basically hijacks your mouse/cursor and keyboard while it runs. If you try to do anything else with them you get terrible results. (I just ran it while I was asleep, eating, watching TV, or something similar). I would assume that a virtual machine (not desktop) would solve this because of the hardware isolation/emulation of your mouse and keyboard. But do you really need to fish _that_ much?


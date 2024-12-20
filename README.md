The Long Dark - TLD Candlelight - v1.8.2 by Digitalzombie
===========================================================

How to install:
===============
1. Download and install "Melon Loader" by HerpDerpinstine
https://github.com/HerpDerpinstine/MelonLoader/
This creates a new folder named "Mods" in your TLD folder.

2. Extract everything from the ZIP file to your new TLD\Mods folder (overwrite old files)

3. In addition you need to install:
- ModSettings
- ModComponent
- CraftingRevision
- GearSpawner
- ModData

You can find everything on: https://xpazeman.com/tld-mod-list/

5. Start the game! 

===========================
===========================

What does it do?
=================

Adds candles and materials for crafting candles

New items:

- GEAR_Wick -> 1 cloth = 4 wicks
- GEAR_FatRaw (modded) or GEAR_AnimalFat_-> Can be obtained by harvesting meat from carcasses, needs to be crafted to get:
- GEAR_Wax -> 4 pieces of tallow + 1 wick can be crafted into: 
- GEAR_Candle -> Burns for 8 hours (default, adjustable)

Everything can spawn as loot. 

Candles are very sensible to wind and turn off easily. 

Flickering effect can be disabled in the mod settings. There are additional options for customizations.

Candle gives +2°C warmth while burning. Crafting and reading is possible.

How to use candles
=================

Light a candle by placing the candle and then holding a burning match/flare/torch and click the candle with the left or middle (AlternativeAction setting) mouse button. The key binding used to light the candle can be changed in mod settings.

Turn off a candle by targeting the candle and clicking the same button used to light it.

You can also light a torch from a lit candle by holding the torch in your hand then targeting the candle and clicking the same button used to light it. You cannot light a candle from another candle, so if you want to light a new candle you will need to place it down and then light a torch from an already lit candle and then use that torch to light the new candle.

You can right click on a candle to move it around. If the candle is lit it will stay lit while you're moving it. If you pick up a lit candle and put it in your inventory then it will immediately go out.

===========================
===========================

BUGS ARE TO BE EXPECTED!!!
===========================

Keep yourself up to date on the progress on:
https://www.youtube.com/c/DigitalzombieDev

Or on Discord:
https://discord.gg/AqpW9TjUfr

===========================
===========================

Changelog:
==========
1.9.1
- yeeaaah yeaaah

1.9.0
- TLD 2.35 update
- Vanilla fat integration
- Everything in crafting menu now

1.8.2
- TLD 2.29 fix
- Save/Load of on/off state maybe fixed too now

1.7.0
Fixes
- Fixed shader errors when inspecting
- Workaround for error when hovering over a stove spot while in placement mode
- Lit candle allows crafting in the dark again

Additions
- Inspect mode model now the same as ingame model (burned down)
- Small light tweaks
- Optional display of remaining burntime on hover text


1.6.0
- Log message fix
- Maybe floating flame fix
- New "endless" option
- Added wind sensitivity option
- If set to endless burning, burned down state (visual) is getting randomized on new candles
- Longer burntime options
- Light/flame color gets now updated immediately
- Refactored code A LOT
- Added crafting icons for candle & wick

1.5.5
- Added russian localization by TylerSky
- Added turkish localization by Emre || Elderly


1.5.0
- Light fix for TLD > 2.23/2.24
- No more bright (unlit) candles in Pleasent Valley Farmhouse
- New savesystem (burntime, lit state), independant of item "health"
- Small light tweaks (light doesn't shine through thin surfaces below it)
- Burntime adjustment in settings (0-24 hours, 0 burns forever)
- No more synchronized flicker
- Inspectmode shows burned down model
- Candle doesn't turn off on move
- Requires ModData now
- Maybe more, I forgot

Known issues: 
- Candle burntime reset on old savegames (if you got candles already)
- Inspect model reacts strange to light when candle is lit

1.4.0
- Fat harvest fix

1.3.8
- Fixed random errors

1.3.5
- Removed AlternativeAction Utilities dependency
- Added option to assign custom button for lighting/extinguishing candle
- Added option for unlimited burntime
- Added more loose spawnpoints
- Fixed brightness of candle texture in dark places
- Maybe fixed incompatibility with WeightTweaks

1.3.0
- After DLC update

1.2.0
- Fixed bug; candles in inventory don't count as lit lightsource anymore
- Renamed "wax" to "tallow"
- Torch can now be used to light the candles
- Unlit candles are now always displaying inspect view
- Lit candles are always turned off on primary interaction
- Fat and wax/tallow now stackable
- Added crafting of wicks! 1 cloth = 4 wicks

1.0.0 	- First release

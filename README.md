# AI-Overhaul-Patcher
A Synthethis patcher for AI Overhaul SE https://www.nexusmods.com/skyrimspecialedition/mods/21654
- Forwards Packages from AIO, keeps packages added by later loaded mods that were not removed by USEEP or AIO
- Forwards observe and combat package lists from AIO
- Adds AIO added factions to the latest winning override set of factions
- Forwards the maximum Essential/Protected status present at AIO or later loaded plugins
- Forwards minimum confidence level present at AIO or later loaded plugins
- Uses the latest loaded outfits that were not overwritten or removed by AIO or USEEP

Get Synthesis https://github.com/Mutagen-Modding/Synthesis/wiki/Installation

Settings :
-Ignore Identical To LastOverride
	Default = false
	When enabled the patcher will not override NPCs that are already patched.
-IgnorePlayerRecord
	Default = true
	When enabled the patcher will ignore the player record (00000007).
-MaintainHighestProtectionLevel
	Default = true
	When enabled NPCs that are set to Essential or Protected by other mods will maintain the highest protection level.

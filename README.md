# Creating Custom Voice Packs for VoiceChanger

## Overview

VoiceChanger allows you to replace any crew voice line in GHPC with your own audio files. Voice packs are folders containing `.wav` or `.ogg` audio files named after the voice actions they replace.

---

## Folder Structure

Place your voice pack folder directly inside the `Mods/VoiceChanger/` folder:


    /Gunner HEAT PC
      /Bin
        /Mods
          /VoiceChanger
              /MyVoicePack/
                  /combat
                      ontheway.wav
                  /regular
                      gunnersabottank.ogg
                      gunnerheattank.wav
                  /panic
                      theyseeus.wav
        

---

## Assigning a Voice Pack to a Vehicle

Open `UserData/MelonPreferences.cfg` and find the `[GHPCVoiceChanger]` section. Set the vehicle entry to the name of your voice pack folder:

M1 = "MyVoicePack"  
M1IP = "MyVoicePack"  
You can assign the same voice pack to multiple vehicles, or create separate packs for each.  

---

## Mixed Voicelines

Each vehicle has a `_MixedVoicelines` toggle in the config. When set to `true`, any voice lines you haven't replaced will fall back to the vanilla voices. When set to `false`, only your custom files will play — missing lines will be silent.

M1_MixedVoicelines = true
---

## Crew States

The game has three crew states that affect which voice lines play. You can provide different audio for each state by placing files in subfolders:

| Subfolder | When it plays |
|-----------|--------------|
| `combat/` | Crew is slightly stressed|
| `panicked/` | Crew is under a lot of stress|
| `regular/` | Used by vehicles as their calm state |

If no state-specific file is found, the mod falls back to a file of the same name in the root of your voice pack folder. If no file is found at all, the vanilla voice line plays if Mixed Voicelines is enabled.

---

## Multiple Lines per Action

You can provide multiple audio files for a single action and the mod will pick one at random each time. Name them with a number suffix:

ontheway.wav  
ontheway_1.wav  
ontheway_2.wav  
ontheway_3.wav  
This works in state subfolders too.

---

## All Supported File Names

### Commander Target Callout
These play when the commander spots a target and calls out the target type, ammo to load and direction. The file name is built from ammo type + target type:

| File name | What it does |
|-----------|-------------|
| `gunnersabottank.wav` | "Gunner, sabot, tank!" |
| `gunneraptank.wav` | "Gunner, AP, tank!" |
| `gunnerheattank.wav` | "Gunner, HEAT, tank!" |
| `gunnersabotpc.wav` | "Gunner, sabot, APC!" |
| `gunneraptroops.wav` | "Gunner, AP, troops!" |
| `gunnersabotat.wav` | "Gunner, sabot, AT team!" |
| `gunnercoaxtroops.wav` | "Gunner, coax, troops!" |
| `gunnercoaxtruck.wav` | "Gunner, coax, truck!" |
| `gunnercoaxat.wav` | "Gunner, coax, AT team!" |
| `gunnersabotchopper.wav` | "Gunner, sabot, chopper!" |

### Fire Command

| File name | What it does |
|-----------|-------------|
| `fire.wav` | "Fire!" (same ammo already loaded) |
| `firesabot.wav` | "Fire sabot!" (switching to sabot) |
| `fireap.wav` | "Fire AP!" (switching to AP) |
| `fireheat.wav` | "Fire HEAT!" (switching to HEAT) |
| `on.wav` | "On!" (on target) |
| `steady.wav` | "Steady!" (gunner is steady on target) |
| `traverseleft.wav` | "Left!" (traverse left) |
| `traverseright.wav` | "Right!" (traverse right) |
| `obstructed.wav` | Shot is obstructed |
| `relase.wav` | "Re-lase!" (laser rangefinder again) |

### Gunner Lines

| File name | What it does |
|-----------|-------------|
| `ontheway.wav` | "On the way!" after firing |
| `identified.wav` | "Identified!" |
| `id.wav` | Short identification callout |

### Round Sensing

| File name | What it does |
|-----------|-------------|
| `short.wav` | "Short!" round fell short |
| `over.wav` | "Over!" round went over |
| `doubtful.wav` | "Doubtful!" hit unclear |
| `reengage.wav` | "Reengage!" |
| `nexttarget.wav` | "Next target!" |
| `ceasefire.wav` | "Cease fire!" |
| `targetceasefire.wav` | "Target, cease fire!" |
| `ceasetracking.wav` | "Cease tracking!" |
| `tracking.wav` | "Tracking!" (guiding missile) |
| `lost.wav` | "Lost!" (target lost) |

### Loader Lines

| File name | What it does |
|-----------|-------------|
| `up.wav` | "Up!" (round loaded, ready to fire) |
| `sabotup.wav` | "Sabot up!" |
| `apup.wav` | "AP up!" |
| `heatup.wav` | "HEAT up!" |
| `missileup.wav` | "Missile up!" |
| `sabotready.wav` | "Sabot ready!" |
| `identified.wav` | Loader identification callout |
| `empty.wav` | "Empty!" ammo depleted |
| `az.wav` | Autoloader AZ carousel cycling |
| `mz.wav` | Autoloader MZ cycling |

### Last Round Warning

| File name | What it does |
|-----------|-------------|
| `lastsabot.wav` | "Last sabot!" |
| `lastap.wav` | "Last AP!" |
| `lastheat.wav` | "Last HEAT!" |
| `lastmissile.wav` | "Last missile!" |

### Ammo Ready Callouts

| File name | What it does |
|-----------|-------------|
| `readysabot.wav` | "Sabot ready!" |
| `readyap.wav` | "AP ready!" |
| `readyheat.wav` | "HEAT ready!" |
| `readymissile.wav` | "Missile ready!" |
| `readyhe.wav` | "HE ready!" |
| `readycoax.wav` | "Coax ready!" |

### Crew Events

| File name | What it does |
|-----------|-------------|
| `pain.wav` | Crew member wounded |
| `hesdown.wav` | Crew member incapacitated |
| `penetrated.wav` | Vehicle armor penetrated |
| `evac.wav` | Evacuation called (vehicle on fire) |
| `theyseeus.wav` | Dangerous threat detected |

---

## Audio Format Requirements

- Format: `.wav` or `.ogg`
- Recommended encoding: 16-bit PCM for WAV
- Sample rate: 44100 Hz
- Channels: Mono or Stereo

> **Note:** Custom voice lines play slightly quieter than vanilla lines due to how the game processes audio. It is recommended to boost your audio files by around 6–10dB before using them. You can do this with a free tool like [Audacity](https://www.audacityteam.org/) or using ffmpeg:
> ```
> ffmpeg -i input.wav -af "volume=8dB" -acodec pcm_s16le output.wav
> ```

---

## Example Voice Pack Structure

    /Mods
      /VoiceChanger
        /MyAbramsCrew  
          /combat  
            /ontheway.wav  
            /ontheway_1.wav  
            /ontheway_2.wav  
            /gunnersabottank.wav  
            /fire.wav  
            /up.wav  
            /readysabot.wav  
            /nexttarget.wav  
            /ceasefire.wav  
          /panicked  
            /ontheway.wav  
            /fire.wav  
            /up.wav  
            /pain.wav  
            /hesdown.wav  
            /penetrated.wav  
            /evac.wav  
            /theyseeus.wav 
          /regular
            /ontheway.wav

And in `MelonPreferences.cfg`:

M1 = "MyAbramsCrew"  
M1_MixedVoicelines = true  

M1IP = "MyAbramsCrew"  
M1IP_MixedVoicelines = true  

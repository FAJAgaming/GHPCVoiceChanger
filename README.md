# Creating Custom Voice Packs for VoiceChanger

## Overview

VoiceChanger allows you to replace any crew voice line in GHPC with your own audio files. Voice packs are folders containing `.wav` or `.ogg` audio files named after the voice actions they replace.

---

## Planned

-Simpler custom sound management     
-Ability to switch command flows on vehicles to existing ones and hopefully custom ones

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

**American** — commander calls everything in one combined line:

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

**German** — commander calls ammo type then target type as separate lines:

| File name | What it does |
|-----------|-------------|
| `sabot.wav` | "Sabot!" |
| `ap.wav` | "AP!" |
| `heat.wav` | "HEAT!" |
| `he.wav` | "HE!" |
| `coax.wav` | "Coax!" |
| `tank.wav` | "Tank!" |
| `pc.wav` | "APC!" |
| `troops.wav` | "Troops!" |
| `at.wav` | "AT team!" |
| `truck.wav` | "Truck!" |
| `chopper.wav` | "Chopper!" |

**Soviet** — commander calls ammo type, direction, then target type as separate lines:

| File name | What it does |
|-----------|-------------|
| `sabot.wav` | "Sabot!" |
| `ap.wav` | "AP!" |
| `heat.wav` | "HEAT!" |
| `he.wav` | "HE!" |
| `coax.wav` | "Coax!" |
| `tank.wav` | "Tank!" |
| `pc.wav` | "APC!" |
| `troops.wav` | "Troops!" |
| `at.wav` | "AT team!" |
| `truck.wav` | "Truck!" |
| `chopper.wav` | "Chopper!" |
| `direction_30.wav` | Direction 30 (straight ahead) |
| `direction_25.wav` | Direction 25 |
| `direction_35.wav` | Direction 35 |
| `direction_20.wav` | Direction 20 |
| `direction_40.wav` | Direction 40 |
| `direction_15.wav` | Direction 15 |
| `direction_45.wav` | Direction 45 |
| `direction_10.wav` | Direction 10 |
| `direction_50.wav` | Direction 50 |
| `direction_5.wav` | Direction 5 |
| `direction_55.wav` | Direction 55 |
| `direction_60.wav` | Direction 60 (directly behind) |

---

### Fire Command

| File name | What it does |
|-----------|-------------|
| `fire.wav` | "Fire!" (same ammo already loaded) |
| `firesabot.wav` | "Fire sabot!" — American only, when switching to sabot |
| `fireap.wav` | "Fire AP!" — American only, when switching to AP |
| `fireheat.wav` | "Fire HEAT!" — American only, when switching to HEAT |
| `on.wav` | "On!" (on target) — American only |
| `steady.wav` | "Steady!" (gunner is steady on target) — American only |
| `traverseleft.wav` | "Left!" (traverse left) — American only |
| `traverseright.wav` | "Right!" (traverse right) — American only |
| `obstructed.wav` | Shot is obstructed |
| `relase.wav` | "Re-lase!" (laser rangefinder again) — American only |

---

### Gunner Lines

| File name | What it does |
|-----------|-------------|
| `ontheway.wav` | "On the way!" after firing |
| `identified.wav` | "Identified!" after spotting target |
| `id.wav` | Short identification callout — American only |

---

### Round Sensing

| File name | What it does |
|-----------|-------------|
| `target.wav` | "Target!" — plays when a round hits |
| `short.wav` | "Short!" round fell short |
| `over.wav` | "Over!" round went over |
| `doubtful.wav` | "Doubtful!" hit unclear |
| `reengage.wav` | "Reengage!" — plays after a hit if target still alive (American and German only) |
| `nexttarget.wav` | "Next target!" — American and German only |
| `ceasefire.wav` | "Cease fire!" target destroyed |
| `targetceasefire.wav` | "Target, cease fire!" — American only |
| `ceasetracking.wav` | "Cease tracking!" — American only |
| `tracking.wav` | "Tracking!" guiding a missile — American only |
| `lost.wav` | "Lost!" target lost |

---

### Loader Lines

| File name | What it does |
|-----------|-------------|
| `up.wav` | "Up!" round is loaded and ready |
| `sabotup.wav` | "Sabot up!" after reloading sabot — American only |
| `apup.wav` | "AP up!" after reloading AP — American only |
| `heatup.wav` | "HEAT up!" after reloading HEAT — American only |
| `heup.wav` | "HE up!" after reloading HE — American only |
| `missileup.wav` | "Missile up!" after reloading missile — American only |
| `sabotready.wav` | "Sabot ready!" — American only |
| `empty.wav` | "Empty!" no ammo left — German and Soviet only |
| `az.wav` | Autoloader AZ carousel cycling — Soviet only |
| `mz.wav` | Autoloader MZ cycling — Soviet only |

---

### Last Round Warning

| File name | What it does |
|-----------|-------------|
| `lastsabot.wav` | "Last sabot!" — American only |
| `lastap.wav` | "Last AP!" — American only |
| `lastheat.wav` | "Last HEAT!" — American only |
| `lastmissile.wav` | "Last missile!" — American only |

---

### Ammo Ready Callouts

These play at mission start when the crew announces what round is loaded:

| File name | What it does |
|-----------|-------------|
| `readysabot.wav` | "Sabot ready!" |
| `readyap.wav` | "AP ready!" |
| `readyheat.wav` | "HEAT ready!" |
| `readymissile.wav` | "Missile ready!" |
| `readyhe.wav` | "HE ready!" |
| `readycoax.wav` | "Coax ready!" |

---

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

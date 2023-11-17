<div align="center">
  <img src="https://github.com/XKaguya/AutoReconnect-Remastered/blob/main/AutoReconnect-Remastered.png">
</div>

#
AutoReconnect-Remastered is a plugin based on [Exiled](https://github.com/Exiled-Team/EXILED) for SCP:SL. This Plugin allow players who was accidently disconnected from server to spawn what the player was before disconnected.

# How to install

Make sure you've installed the newest version of [Exiled](https://github.com/Exiled-Team/EXILED).

Before Exiled merged this [Pull Request](https://github.com/Exiled-Team/EXILED/pull/2249), you should use the modified version of Exiled.(You may add the public method by yourself and use your own complied `Exiled.API.dll` too.)

1. Download newest version of `AutoReconnect-Remastered.dll` and `Exiled.API.dll` from [Release](https://github.com/XKaguya/AutoReconnect-Remastered/releases)

2. Put `AutoReconnect-Remastered.dll` file at `%AppData%\EXILED\Plugins`. (Press Win + R and input the `%AppData%\EXILED\Plugins` for directly open the Plugins folder.)

3. Put `Exiled.API.dll` at `%AppData%\SCP Secret Laboratory\PluginAPI\plugins\global\dependencies`. (Same as above.)

4. Start the server.

5. Enjoy.


# Features

Automaticly spawn disconnected player as what it was.

The player will have same HP, same Inventory, same Ammo and etc.

Full Auto plugin. No need do anything else than install.

About player who is using *DNT*, that `.accept` or `.deny` is required.

See: 

<div align="center">
  <img src="https://github.com/XKaguya/AutoReconnect-Remastered/blob/main/DNT%20Hint.png">
</div>

```
Restore player's Role.
Restore player's HP.
Restore player's Ammo.
Restore player's Inventory.
Restore player's Effect with Intensity and Duration.
```

# Configuration

```
# Whether or not to restore player's active effect. Player will respawn with same effect strengh and remain time.
recovery_effect: true
# Whether or not to restore player's inventory. Player will respawn with same inventory.
recovery_inventory: true
# Whether or not to restore player's ammo. Player will respawn with same ammo.
recovery_ammo: true
# Whether or not to spawn disconnected player's ragdoll.
spawn_ragdoll: false
# Text that will be shown to the player at reconnect
reconnect_text: 'You have been reconnected. You will be respawn to stored status.'
```


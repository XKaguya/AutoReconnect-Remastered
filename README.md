<div align="center">
  <img src="https://github.com/XKaguya/AutoReconnect-Remastered/blob/main/AutoReconnect-Remastered.png">
</div>

#
AutoReconnect-Remastered is a plugin based on [Exiled](https://github.com/Exiled-Team/EXILED) for SCP:SL. This plugin allows players who were accidentally disconnected from the server to spawn as the same character they were before being disconnected.

# How to install

Make sure you've installed the newest version of [Exiled](https://github.com/Exiled-Team/EXILED).

1. Download newest version of `AutoReconnect-Remastered.dll` from [Release](https://github.com/XKaguya/AutoReconnect-Remastered/releases)

2. Put `AutoReconnect-Remastered.dll` file at `%AppData%\EXILED\Plugins`. (Press Win + R and input the `%AppData%\EXILED\Plugins` for directly open the Plugins folder.)

3. Start the server.

4. Enjoy.


# Features

Automaticly spawn disconnected player as what it was.

The player will have same HP, same Inventory, same Ammo and etc.

```
Restore player's Role.
Restore player's HP.
Restore player's Ammo.
Restore player's Inventory.
Restore player's Effect with Intensity and Duration.
Replace disconnected Scp player to a random spectator player.
```

Full Auto plugin. No need do anything else than install.

About player who is using *DNT*, that `.accept` or `.deny` is required.

See: 

<div align="center">
  <img src="https://github.com/XKaguya/AutoReconnect-Remastered/blob/main/DNT%20Hint.png">
</div>

# ToDo
```
Custom Role/Item support. (Exiled 9)
```

# Configuration

```
# Whether or not this plugin is enabled.
is_enabled: true
# Whether or not to display debug messages in the server console.
debug: true
# How long will the server hold the player's data. Set to 0 to disable time limit.
retention_time: 0
# Whether or not to respawn player. Set to false and it WILL NOT respawn player.
respawn_enabled: true
# Whether or not to restore player's active effect. Player will respawn with same effect strengh and remain time.
recovery_effect: true
# Whether or not to restore player's inventory. Player will respawn with same inventory.
recovery_inventory: true
# Whether or not to restore player's ammo. Player will respawn with same ammo.
recovery_ammo: true
# Whether or not to spawn disconnected player's ragdoll.
spawn_ragdoll: false
# Whether or not to randomly choose a spectator to be the disconnected SCP. While RespawnEnabled is false, this config will take the place of the respawn.
random_spec: false
# Text that will be shown to the player at reconnect.
reconnect_text: 'You have been reconnected. You will be respawn to stored status.'
# Whether or not to enable revive block. If true then player won't be revive in the revive block time.
revive_block: true
# How long the revive will be block.
revive_block_time: 10
# How long the revive timer will delay.
revive_block_delay: 1
# Text that will be shown to the player who enabled DNT.
joined_broadcast:
# The broadcast content
content: |-
  Plugin requires your auth for storing data. Do you want to accept that ?
  If accepted you''re able to enjoy reconnect respawn features.
  Type .accept in console for accept .deny for deny.
# The broadcast duration
duration: 15
# The broadcast type
type: Normal
# Indicates whether the broadcast should be shown or not
show: true
# The message that sent to all players while scp is replaced by spectator.
disconnected_message: '{0} has been replaced by player {1}'
# The way plugin to show for spectator players. Set 1 for Hint, 2 for Broadcast.
disconnected_message_type: 1
# The information used for other plugins to avoid conflicts. Set to default if you have no conflict.
spawn_reason: RemoteAdmin
```


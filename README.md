* Simple plugin that reloads listed plugins after x seconds when server has finished loading in.
* This can be useful incase of loading order requirements or any other issue required to reload a plugin after startup.

## Features :

* Easy configuration file
* Set time (seconds) to run sequence x seconds after server has finished loading in.
* Set time (seconds) between each plugin to be reloaded
*This is still a temp fix for plugins not doing what they need to do after startup contact the developer/maintainer of the plugin and point them to the issue !

## Configuration :

```json
{
  "Plugin settings": {
    "Time After Startup (seconds)": 20.0,
    "Time between Reloads (seconds)": 2.0,
    "Plugins to Reload": [
      "BradleyGuards",
      "PumpkinHead",
      "WalkingDead",
      "ZombieRocks"
    ]
  }
}
```
test for discord webhook

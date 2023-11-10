# CustomItems.API
Plugin/API to facilitate the creation of custom items for Northwood API.  
This is a port of [Exiled custom item API](https://github.com/Exiled-Team/EXILED/tree/master/Exiled.CustomItems) to NWAPI.  
**Note: I am not good at explaining my plugins**
## Commands

Command | Arguments | Permissions | Description
:---: | :---: | :---: | :------
ci give | (custom item name/id) [playerid/All/*] | PlayerPermissions.GivingItems | Gives the specified item to the indicated player. If no player is specified it gives it to the person running the command.
ci spawn | (custom item/id) [SpawnLocationType/PlayerId/Vector3] | PlayerPermissions.GivingItems | Spawn the specified item at the specified spawn location, player or coordinates
ci info | (custom item name/id) | n/a | Prints a more detailed list of info about a specific item, including name, id, description and spawn locations + chances.
ci list | n/a | n/a | Gets a list of all currently registered custom items.

## Spawn locations
The following list of locations are the only ones that are able to be used in the SpawnLocation configs for each item:
(Their names must be typed EXACTLY as they are listed, otherwise you will probably break your item config file)
```cs
Inside330
Inside330Chamber
Inside049Armory
Inside079Secondary
Inside096
Inside173Armory
Inside173Bottom
Inside173Connector
InsideEscapePrimary
InsideEscapeSecondary
InsideIntercom
InsideLczArmory
InsideLczCafe
InsideNukeArmory
InsideSurfaceNuke
Inside079First
Inside173Gate
Inside914
InsideGateA
InsideGateB
InsideGr18
InsideHczArmory
InsideHid
InsideHidLeft
InsideHidRight
InsideLczWc
InsideServersBottom
InsideLocker
```

# Developers
This API when loading custom items will check if the class where ``CustomItem.RegisterCustomItems`` is executed is the EntryPoint of the plugin and if it is it will look if there is a field with the attribute of [CustomItemConfig] is very similar to how the NWAPI configuration is registered, the custom items are loaded from the configuration, this is to facilitate the creation of configurations (exactly the same as Exiled does).

If you want to create your own custom items you need to follow a couple of strict rules
1. ``CustomItem.RegisterItems`` must be executed in the EntryPoint of your plugin.

2. You must create a class where you will store the settings/custom items. This class must be referenced in the EntryPoint as a field and have the attribute of ``[CustomItemConfig]``.

## Example
## The best example a can give its the [CustomItems plugin](https://github.com/NWAPI-CustomItems/CustomItems/tree/main/NWAPI.CustomItems)
**Entry point**
```cs
public class EntryPoint
    {

        /// <summary>
        /// Custom items config class.
        /// </summary>
        [CustomItemConfig]
        public CustomItemConfigs CustomItems = null!;


        [PluginEntryPoint("NWAPI.CustomItems", Version, "This containing custom items for NWAPI", "SrLicht")]
        private void OnLoad()
        {
            Instance = this;

            if (!Config.IsEnabled)
                return;

            HarmonyId = $"NWAPI.CustomItems.{Version}";
            Harmony = new(HarmonyId);

            try
            {
                Harmony.PatchAll();
            }
            catch (HarmonyException e)
            {
                Log.Error($"Error on patching: {e}");
            }

            try
            {
                // Regiter all current items in this.CustomItems
                CustomItem.RegisterItems();
            }
            catch (Exception e)
            {
                Log.Error($"Error on trying to register items: {e.Message}");
            }
        }
    }
```
**CustomItemConfigs value**
```cs
    /// <summary>
    /// All custom items to be registered.
    /// </summary>
    public class CustomItemConfigs
    {
        /// <summary>
        /// Gets the list of escape Coins.
        /// </summary>
        [Description("The list of escape coins.")]
        public List<EscapeCoin> EscapeCoins { get; set; } = new()
        {
            new EscapeCoin(),
        };
   }
```

# Credits
- **Redforce04 was here** | Thanks for the help with the configurations
- **Exiled Team** | some methods are directly extracted from the Exiled repository

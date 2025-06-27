# ScarletBrazier

**ScarletBrazier** is a V Rising mod that makes all braziers burn indefinitely without consuming bones as fuel. The mod provides a convenient way to have perpetual sun protection throughout your castle without the hassle of constantly feeding your braziers with bones.

---

## Support & Donations

<a href="https://www.patreon.com/bePatron?u=30093731" data-patreon-widget-type="become-patron-button"><img height='36' style='border:0px;height:36px;' src='https://i.imgur.com/o12xEqi.png' alt='Become a Patron' /></a>  <a href='https://ko-fi.com/F2F21EWEM7' target='_blank'><img height='36' style='border:0px;height:36px;' src='https://storage.ko-fi.com/cdn/kofi6.png?v=6' alt='Buy Me a Coffee at ko-fi.com' /></a>

---

## How It Works

When enabled globally, all braziers in the world will automatically burn without consuming bones as fuel. They will stay lit indefinitely, creating smoke that blocks sunlight and provides permanent sun protection for vampires in your castle and territories. The mod also handles newly placed braziers, ensuring they are automatically set to burn freely without requiring fuel.

**Note:** Free braziers will automatically light up when the server restarts or when `.brazier enable` or `.brazier force` commands are used.

**Troubleshooting:** If a brazier doesn't light up when clicked, simply add 1 bone to it - the bone will be consumed and the brazier will start working fuel-free again.

## Features

* All braziers burn indefinitely without consuming bones as fuel
* Braziers remain fuel-free even after being manually turned off and on again
* Global enable/disable functionality
* Automatic handling of newly placed braziers
* Individual brazier control with force commands
* Admin-only commands for server management
* Persistent settings that survive server restarts

## Commands

**Note:** All commands require admin privileges.

* `.brazier enable` - Enables free braziers globally (all braziers burn without consuming bones)
* `.brazier disable` - Disables free braziers globally (braziers return to consuming bones normally)
* `.brazier force` - Forces all existing braziers to burn freely, regardless of global setting

### Command Details

**Global Control:**
- When enabled globally, all braziers (existing and newly placed) will burn indefinitely without consuming fuel
- When disabled globally, all braziers return to normal operation, consuming bones as fuel when lit
- Braziers affected by the mod remain fuel-free even when manually turned off and on by players

**Force Command:**
- Use this to manually make all current braziers burn freely, even if global mode is disabled
- Useful for one-time setup or testing purposes

## Installation

### Requirements

This mod requires the following dependencies:

* **[BepInEx](https://wiki.vrisingmods.com/user/bepinex_install.html)**
* **[ScarletCore](https://thunderstore.io/c/v-rising/p/ScarletMods/ScarletCore/)**
* **[VampireCommandFramework](https://thunderstore.io/c/v-rising/p/deca/VampireCommandFramework/)**

Make sure BepInEx is installed and loaded **before** installing ScarletBrazier.

### Manual Installation

1. Download the latest release of **ScarletBrazier**.

2. Extract the contents into your `BepInEx/plugins` folder:

   `<V Rising Server Directory>/BepInEx/plugins/`

   Your folder should now include:

   `BepInEx/plugins/ScarletBrazier.dll`

3. Ensure **ScarletCore** and **VampireCommandFramework** are also installed in the `plugins` folder.
4. Start or restart your server.

## Configuration

The mod creates a configuration file that allows you to set the default behavior:

* **EnableGlobally** (default: true) - If true, all braziers will burn without consuming bones as fuel

## Technical Details

* Braziers are set to burn for approximately 1 year (31,536,000 seconds) when made fuel-free
* The mod uses Harmony patches to handle newly placed braziers automatically
* All operations include proper error handling and logging
* Memory management is handled properly with automatic disposal of entity queries

## This project is made possible by the contributions of the following individuals:

- **cheesasaurus, EduardoG, Helskog, Mitch, SirSaia, Odjit** & the [V Rising Mod Community on Discord](https://vrisingmods.com/discord)

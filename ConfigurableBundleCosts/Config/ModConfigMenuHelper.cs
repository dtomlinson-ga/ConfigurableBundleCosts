// Copyright (C) 2021 Vertigon
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see https://www.gnu.org/licenses/.

using StardewModdingAPI;
using System;

namespace ConfigurableBundleCosts
{
	/// <summary>
	/// Collection of methods to simplify the process of adding Generic Mod Config Menu options.
	/// </summary>
	internal class ModConfigMenuHelper
	{

		private static IGenericModConfigMenuAPI api;
		private static bool suppliedByContentPack = false;

		/// <summary>
		/// Checks to see if GMCM is installed - if so, creates options page with all configurable settings.
		/// </summary>
		/// <returns> <c>True</c> if options page successfully created, <c>False</c> otherwise.</returns>
		public static bool TryLoadModConfigMenu()
		{
			try
			{
				// Check to see if Generic Mod Config Menu is installed
				if (!Globals.Helper.ModRegistry.IsLoaded("spacechase0.GenericModConfigMenu"))
				{
					Globals.Monitor.Log("GenericModConfigMenu not present - skipping mod menu setup");
					return false;
				}

				api = Globals.Helper.ModRegistry.GetApi<IGenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
				api.RegisterModConfig(Globals.Manifest,
					() => Globals.Config = new ModConfig(),
					() => Globals.Helper.WriteConfig(Globals.Config)
				);

				CheckForContentPacks();
				RegisterModOptions();
				return true;
			}
			catch (Exception e)
			{
				Globals.Monitor.Log("Failed to register GMCM menu - skipping mod menu setup", LogLevel.Error);
				Globals.Monitor.Log(e.Message, LogLevel.Error);
				return false;
			}
		}

		public static void CheckForContentPacks()
		{
			foreach (IContentPack contentPack in Globals.Helper.ContentPacks.GetOwned())
			{
				if (!contentPack.HasFile("config.json"))
				{
					Globals.Monitor.Log($"Required file missing in content pack {contentPack.Manifest.Name}: config.json", LogLevel.Warn);
					Globals.Monitor.Log("Skipping content pack", LogLevel.Warn);
				}
				else
				{
					Globals.Monitor.Log($"Located config.json in content pack {contentPack.Manifest.Name} - applying values");

					try
					{
						ModConfig configData = contentPack.ReadJsonFile<ModConfig>("config.json");
						Globals.Config = configData;
						suppliedByContentPack = true;
					}
					catch (Exception ex)
					{
						Globals.Monitor.Log($"Exception encountered while parsing config.json provided by {contentPack.Manifest.Name}: \n{ex}", LogLevel.Error);
						Globals.Monitor.Log("Falling back to default config values", LogLevel.Warn);
						Globals.Config = new ModConfig();
					}
				}
			}
		}

		/// <summary>
		/// Adds all descriptions and options to options page.
		/// </summary>
		public static void RegisterModOptions()
		{
			if (suppliedByContentPack)
			{
				AddParagraph("Note: The current values are supplied by a downloaded content pack for this mod, and changing them may run counter to the creator's desired effect.");
				AddLabel("");
			}

			AddLabel("Joja Bundle Costs");
			AddIntUnclamped("Bus Cost", "Cost to repair the bus to Calico Desert",
				() => Globals.Config.Joja.busCost,
				(int var) =>
					{
						Globals.Config.Joja.busCost = var;
					}
			);

			AddIntUnclamped("Minecarts Cost", "Cost to repair the minecart system around town",
				() => Globals.Config.Joja.minecartsCost,
				(int var) => Globals.Config.Joja.minecartsCost = var
			);

			AddIntUnclamped("Bridge Cost", "Cost to repair the bridge to the quarry",
				() => Globals.Config.Joja.bridgeCost,
				(int var) => Globals.Config.Joja.bridgeCost = var
			);

			AddIntUnclamped("Greenhouse Cost", "Cost to repair the greenhouse on the farm",
				() => Globals.Config.Joja.greenhouseCost,
				(int var) => Globals.Config.Joja.greenhouseCost = var
			);

			AddIntUnclamped("Panning Cost", "Cost to remove the glittering boulder on the mountain",
				() => Globals.Config.Joja.panningCost,
				(int var) => Globals.Config.Joja.panningCost = var
			);

			AddLabel("Vault Bundle Costs");

			AddIntUnclamped("Vault Bundle 1 Cost", "Cost of Vault Bundle 1",
				() => Globals.Config.Vault.bundle1,
				(int var) => Globals.Config.Vault.bundle1 = var
			);

			AddIntUnclamped("Vault Bundle 2 Cost", "Cost of Vault Bundle 2",
				() => Globals.Config.Vault.bundle2,
				(int var) => Globals.Config.Vault.bundle2 = var
			);

			AddIntUnclamped("Vault Bundle 3 Cost", "Cost of Vault Bundle 3",
				() => Globals.Config.Vault.bundle3,
				(int var) => Globals.Config.Vault.bundle3 = var
			);

			AddIntUnclamped("Vault Bundle 4 Cost", "Cost of Vault Bundle 4",
				() => Globals.Config.Vault.bundle4,
				(int var) => Globals.Config.Vault.bundle4 = var
			);

		}

		/// <summary>
		/// Shorthand method to create a Label.
		/// </summary>
		private static void AddLabel(string name, string desc = "")
		{
			api.RegisterLabel(Globals.Manifest, name, desc);
		}

		/// <summary>
		/// Shorthand method to create a Paragraph.
		/// </summary>
		private static void AddParagraph(string text)
		{
			api.RegisterParagraph(Globals.Manifest, text);
		}

		/// <summary>
		/// Shorthand method to create a Dropdown menu.
		/// </summary>
		private static void AddDropdown(string name, string desc, Func<string> get, Action<string> set, string[] choices)
		{
			api.RegisterChoiceOption(Globals.Manifest, name, desc, get, set, choices);
		}

		/// <summary>
		/// Shorthand method to create an Integer input field.
		/// </summary>
		private static void AddIntUnclamped(string name, string desc, Func<int> get, Action<int> set)
		{
			api.RegisterSimpleOption(Globals.Manifest, name, desc, get, set);
		}

		/// <summary>
		/// Shorthand method to create an Integer slider.
		/// </summary>
		private static void AddIntClamped(string name, string desc, Func<int> get, Action<int> set, int min, int max)
		{
			api.RegisterClampedOption(Globals.Manifest, name, desc, get, set, min, max);
		}

		/// <summary>
		/// Shorthand method to create a checkbox.
		/// </summary>
		private static void AddCheckBox(string name, string desc, Func<bool> get, Action<bool> set)
		{
			api.RegisterSimpleOption(Globals.Manifest, name, desc, get, set);
		}
	}
}

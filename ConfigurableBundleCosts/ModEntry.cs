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
using StardewValley;

namespace ConfigurableBundleCosts
{
	/// <summary>The mod entry point.</summary>
	public class ModEntry : Mod
	{
		private AssetEditor modAssetEditor;

		/// <summary>The mod entry point.</summary>
		/// <param name="helper" />
		public override void Entry(IModHelper helper)
		{
			Globals.Config = helper.ReadConfig<ModConfig>();
			Globals.Helper = helper;
			Globals.Monitor = Monitor;
			Globals.Manifest = ModManifest;

			modAssetEditor = new AssetEditor();
			helper.Content.AssetEditors.Add(modAssetEditor);

			helper.Events.GameLoop.GameLaunched += (sender, args) => ModConfigMenuHelper.TryLoadModConfigMenu();
			helper.Events.GameLoop.SaveLoaded += (sender, args) =>
			{
				LoadAssets();
				CheckBundleData();
			};

			if (BundlePricePatch.ApplyHarmonyPatches())
				Monitor.Log("Patches successfully applied");
		}

		private static void LoadAssets()
		{
			if (AssetEditor.LoadAssets()) Globals.Monitor.Log("Loaded assets");
			else Globals.Monitor.Log("Failed to load assets");
		}

		private static void CheckBundleData()
		{
			AssetEditor.InvalidateCache();


			if (Game1.netWorldState?.Value != null)
			{
				Game1.netWorldState.Value.SetBundleData(AssetEditor.bundleData);
			}
		}
	}
}

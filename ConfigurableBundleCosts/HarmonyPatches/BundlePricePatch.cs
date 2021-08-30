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

using Harmony;
using StardewModdingAPI;
using StardewValley.Menus;
using System;

namespace ConfigurableBundleCosts
{
	public class BundlePricePatch
	{

		/// <returns><c>True</c> if successfully patched, <c>False</c> if Exception is encountered.</returns>
		public static bool ApplyHarmonyPatches()
		{
			try
			{
				HarmonyInstance harmony = HarmonyInstance.Create(Globals.Manifest.UniqueID);

				harmony.Patch(
					original: typeof(JojaCDMenu).GetMethod("getPriceFromButtonNumber"),
					prefix: new HarmonyMethod(typeof(BundlePricePatch).GetMethod("GetPriceFromButtonNumber_Prefix"))
				);

				return true;
			}
			catch (Exception e)
			{
				Globals.Monitor.Log(e.ToString(), LogLevel.Error);
				return false;
			}
		}

		public static bool GetPriceFromButtonNumber_Prefix(int buttonNumber, ref int __result)
		{
			try
			{
				switch (buttonNumber)
				{
					case 0:
						__result = Globals.Config.Joja.busCost;
						break;
					case 1:
						__result = Globals.Config.Joja.minecartsCost;
						break;
					case 2:
						__result = Globals.Config.Joja.bridgeCost;
						break;
					case 3:
						__result = Globals.Config.Joja.greenhouseCost;
						break;
					case 4:
						__result = Globals.Config.Joja.panningCost;
						break;
					default:
						__result = 10000;
						Globals.Monitor.Log("Unrecognized button number selected", LogLevel.Warn);
						break;
				}

				return false;
			}
			catch (Exception ex)
			{
				Globals.Monitor.Log($"Failed in {nameof(GetPriceFromButtonNumber_Prefix)}:\n{ex}", LogLevel.Error);
				return true; // run original logic
			}
		}
	}
}

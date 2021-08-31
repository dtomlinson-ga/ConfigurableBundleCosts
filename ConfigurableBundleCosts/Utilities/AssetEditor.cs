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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ConfigurableBundleCosts
{
	class AssetEditor : IAssetEditor
	{
		private static Texture2D cdFont;
		public static Dictionary<string, string> bundleData;

		private static Vector2
			busOnesDigit = new Vector2(130, 59),
			minecartsOnesDigit = new Vector2(301, 59),
			bridgeOnesDigit = new Vector2(131, 89),
			greenhouseOnesDigit = new Vector2(300, 89),
			panningOnesDigit = new Vector2(129, 119),
			panningOnesDigitGerman = new Vector2(130, 127)
		;

		/// <summary>
		/// Attempts to load mod assets.
		/// </summary>
		/// <returns><c>True</c> if successful, <c>False</c> otherwise.</returns>
		public static bool LoadAssets()
		{
			cdFont = Globals.Helper.Content.Load<Texture2D>("assets/cdFont.png");

			// need to initialize bundle data asap so NetWorldState.SetBundleData doesn't break
			Dictionary<string, string> bundleDataAsset = Globals.Helper.Content.Load<Dictionary<string, string>>("Data/Bundles", ContentSource.GameContent);
			bundleData = bundleDataAsset;

			return bundleData != null && cdFont != null;
		}

		/// <summary>
		/// Utilized by SMAPI to determine whether an asset should be edited.
		/// </summary>
		public bool CanEdit<T>(IAssetInfo asset)
		{
			return asset.AssetNameEquals("LooseSprites/JojaCDForm")
				|| asset.AssetNameEquals("Data/Bundles")
				|| asset.AssetNameEquals("Data/ExtraDialogue");
		}

		/// <summary>
		/// Utilized by SMAPI to determine what edits should be made to an asset.
		/// </summary>
		public void Edit<T>(IAssetData asset)
		{
			if (asset.AssetNameEquals("LooseSprites/JojaCDForm")) UpdateCDForm(asset);
			else if (asset.AssetNameEquals("Data/Bundles")) UpdateBundles(asset);
			else if (asset.AssetNameEquals("Data/ExtraDialogue")) UpdateExtraDialogue(asset);

		}

		/// <summary>
		/// This prevents cached values from being used if the player has changed config options.
		/// </summary>
		public static void InvalidateCache()
		{
			Globals.Helper.Content.InvalidateCache("LooseSprites/JojaCDForm");
			Globals.Helper.Content.InvalidateCache("Data/Bundles");
			Globals.Helper.Content.InvalidateCache("Data/ExtraDialogue");
		}

		private static Dictionary<string, string> GetCurrentBundlePrices()
		{
			return new Dictionary<string, string>()
			{
				["bus"] = Globals.Config.Joja.busCost.ToString(),
				["minecarts"] = Globals.Config.Joja.minecartsCost.ToString(),
				["bridge"] = Globals.Config.Joja.bridgeCost.ToString(),
				["greenhouse"] = Globals.Config.Joja.greenhouseCost.ToString(),
				["panning"] = Globals.Config.Joja.panningCost.ToString()
			};
		}

		private static void UpdateCDForm(IAssetData asset)
		{
			IAssetDataForImage cdForm = asset.AsImage();

			Dictionary<string, string> bundlePrices = GetCurrentBundlePrices();

			foreach (KeyValuePair<string, string> pair in bundlePrices)
			{
				// get number of digits in value
				int numDigits = pair.Value.Length;

				Rectangle sourceRect, destRect;
				Vector2 destStartPos = GetDestStartPos(pair.Key);   // position on form to start overwriting from
				int digit, currentPos;

				// need to erase existing numbers
				if (numDigits < 5)
				{
					// start from 5th position
					currentPos = 5;
					// get dots from cdFont.png
					sourceRect = new Rectangle(70, 0, 35, 11);
					// get ten-thousands position of destination - slightly different positions on left and right side of form
					int offset = (pair.Key == "minecarts" || pair.Key == "greenhouse") ? 34 : 33;
					// get position to overwrite using offset
					destRect = new Rectangle((int)destStartPos.X - offset, (int)destStartPos.Y, 35, 11);
					// overwrite with dots
					cdForm.PatchImage(cdFont, sourceRect, destRect);
				}

				// start at last index in pair.Value; decrement until at first index
				for (int k = pair.Value.Length - 1; k >= 0; k--)
				{

					// if number is negative, need to set position of '-' manually on tileset
					if (pair.Value[k] == '-')
					{
						digit = 15;
					}
					// otherwise convert to int from 0-9 to find position on tileset
					else
					{
						// isolate current digit, convert to int
						digit = int.Parse(pair.Value[k].ToString());
					}

					// get current digit position in number
					currentPos = numDigits - (k + 1);

					// create source rectangle by adjusting starting x position by tileset position determined earlier
					sourceRect = new Rectangle(7 * digit, 0, 7, 11);

					// get destination rectangle by adjusting start position by current digit position
					destRect = new Rectangle((int)destStartPos.X - (7 * currentPos), (int)destStartPos.Y, 7, 11);

					// overwrite current position with digit
					cdForm.PatchImage(cdFont, sourceRect, destRect);
				}
			}
		}

		private static void UpdateBundles(IAssetData asset)
		{
			IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
			CultureInfo culture = CultureInfo.GetCultureInfo(Globals.Helper.Translation.Locale);

			int bundle1 = Globals.Config.Vault.bundle1,
				bundle2 = Globals.Config.Vault.bundle2,
				bundle3 = Globals.Config.Vault.bundle3,
				bundle4 = Globals.Config.Vault.bundle4;


			string bundle1String = bundle1.ToString("#,###", culture),
				bundle2String = bundle2.ToString("#,###", culture),
				bundle3String = bundle3.ToString("#,###", culture),
				bundle4String = bundle4.ToString("#,###", culture);

			foreach (string key in data.Keys.ToArray())
			{
				if (key.StartsWith("Vault"))
				{
					string[] tokens = data[key].Split('/');
					string nameToken, valueToken;

					switch (key)
					{
						case "Vault/23":
							{
								nameToken = bundle1String;
								valueToken = $"-1 {bundle1} {bundle1}";
								break;
							}
						case "Vault/24":
							{
								nameToken = bundle2String;
								valueToken = $"-1 {bundle2} {bundle2}";
								break;
							}
						case "Vault/25":
							{
								nameToken = bundle3String;
								valueToken = $"-1 {bundle3} {bundle3}";
								break;
							}
						case "Vault/26":
							{
								nameToken = bundle4String;
								valueToken = $"-1 {bundle4} {bundle4}";
								break;
							}
						default:
							{
								nameToken = "";
								valueToken = "";
								break;
							}
					}

					tokens[0] = nameToken;
					tokens[2] = valueToken;

					data[key] = string.Join("/", tokens);

					bundleData = new Dictionary<string, string>(data);
				}
			}
		}

		private static void UpdateExtraDialogue(IAssetData asset)
		{
			if (Globals.Config.Joja.movieTheaterCost == 500000) return;

			IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
			CultureInfo culture = CultureInfo.GetCultureInfo(Globals.Helper.Translation.Locale);

			string originalValue = 500000.ToString("#,###", culture);
			string newValue = Globals.Config.Joja.movieTheaterCost.ToString("#,###", culture);

			Globals.Monitor.Log($"Original cost string: {originalValue}\nNew cost string: {newValue}");

			data["Morris_BuyMovieTheater"] = data["Morris_BuyMovieTheater"].Replace(originalValue, newValue);
		}

		private static Vector2 GetDestStartPos(string key)
		{
			switch (key)
			{
				case "bus":
					{
						return busOnesDigit;
					}
				case "minecarts":
					{
						return minecartsOnesDigit;
					}
				case "bridge":
					{
						return bridgeOnesDigit;
					}
				case "greenhouse":
					{
						return greenhouseOnesDigit;
					}
				case "panning":
					{
						string locale = Globals.Helper.Translation.Locale;

						Globals.Monitor.Log($"Current culture: {locale}");

						if (locale.ToLower().Equals("de-de"))
							return panningOnesDigitGerman;
						else return panningOnesDigit;
					}
				default:
					{
						Globals.Monitor.Log("No values found for bundle ones digit position", LogLevel.Warn);
						return new Vector2(-1, -1);
					}
			}
		}
	}
}

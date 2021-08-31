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


namespace ConfigurableBundleCosts
{
	/// <summary>
	/// Utilized by SMAPI to store configurable values. Can be modified by hand or by use of GMCM.
	/// </summary>
	public class ModConfig
	{
		public JojaConfig Joja = new JojaConfig();
		public VaultConfig Vault = new VaultConfig();

		public class JojaConfig
		{
			public int busCost = 40000;         // button 0
			public int minecartsCost = 15000;   // button 1
			public int bridgeCost = 25000;      // button 2
			public int greenhouseCost = 35000;  // button 3
			public int panningCost = 20000;     // button 4
			public int movieTheaterCost = 500000;
		}

		public class VaultConfig
		{
			public int bundle1 = 2500;
			public int bundle2 = 5000;
			public int bundle3 = 10000;
			public int bundle4 = 25000;
		}


	}
}
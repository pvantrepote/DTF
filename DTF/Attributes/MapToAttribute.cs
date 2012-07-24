/*  This program is free software: you can redistribute it and/or modify
	it under the terms of the GNU General Public License as published by
	the Free Software Foundation, either version 3 of the License, or
	(at your option) any later version.

	This program is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.

	You should have received a copy of the GNU General Public License
	along with this program.  If not, see <http://www.gnu.org/licenses/>. */

/* V1.0 Pascal Vantrepote (Tamajii) */

using System;

namespace DTF.Attributes
{
	public class MapToAttribute : Attribute
	{

		#region Properties
		
		/// <summary>
		/// Alias of the mapping. 
		/// </summary>
		public string Alias { get; set; }

		/// <summary>
		/// The method to call before setting the value [object GetValue(sourcevalue, sourceproperty)]
		/// </summary>
		public string ValueProcessingMethod { get; set; }

		/// <summary>
		/// Return true if contains an alias
		/// </summary>
		public bool AsAlias
		{
			get { return !string.IsNullOrEmpty(Alias); }
		}

		#endregion

		#region Constructors



		#endregion

	}
}

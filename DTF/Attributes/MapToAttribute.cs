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
	[AttributeUsage(AttributeTargets.Property)]
	public class MapToAttribute : Attribute
	{

		#region Properties

		public string Target { get; set; }
		public Type TargetType { get; set; }
		public string Alias { get; set; }
		public bool AsAlias
		{
			get { return !string.IsNullOrEmpty(Alias); }
		}

		#endregion

		#region Constructors

		public MapToAttribute(string target)
		{
			Target = target;
		}

		public MapToAttribute(string target, Type targetType)
		{
			Target = target;
			TargetType = targetType;
		}

		public MapToAttribute(string alias, string target)
		{
			Alias = alias;
			Target = target;
		}

		#endregion

	}
}

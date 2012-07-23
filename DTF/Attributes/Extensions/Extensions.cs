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
using System.Linq;
using System.Reflection;

namespace DTF.Attributes.Extensions
{
	internal static class Extensions
	{
		public static T[] GetAttributes<T>(this Type type)
			where T: class 
		{
			return (T[]) type.GetCustomAttributes(typeof (T), true);
		}

		public static T[] GetAttributes<T>(this MemberInfo memberInfo)
			where T : class
		{
			return (T[])memberInfo.GetCustomAttributes(typeof(T), true);
		}

		public static T[] GetAttributes<T>(this PropertyInfo propertyInfo)
			where T : class
		{
			return (T[])propertyInfo.GetCustomAttributes(typeof(T), true);
		}

		public static TransformableToAttribute FindAttribute(this TransformableToAttribute[] attributes, MapToAttribute mapToAttribute)
		{
			return attributes.Length == 1 ? attributes[0]
										  : attributes.FirstOrDefault(transformableToAttribute => (transformableToAttribute.Alias != null) &&
																								  (transformableToAttribute.Alias.CompareTo(mapToAttribute.AsAlias) == 0));
		}
	}
}

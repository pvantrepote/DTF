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
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace DTF.Attributes.Extensions
{
	internal static class Extensions
	{
		public static T[] GetAttributes<T>(this Type type)
			where T : Attribute
		{
			var attributes = TypeDescriptor.GetAttributes(type);

			return attributes.Cast<Attribute>().Where(attribute => attribute.GetType() == typeof(T)).Cast<T>().ToArray();
		}

		public static T[] GetAttributes<T>(this MemberInfo memberInfo)
			where T : class
		{
			var attributes = Attribute.GetCustomAttributes(memberInfo, typeof(T), true);
			return attributes as T[];
		}

		public static T[] GetAttributes<T>(this PropertyInfo propertyInfo, Type inType)
			where T : Attribute
		{
			var attributes = Attribute.GetCustomAttributes(propertyInfo, typeof(T), true);
			if (attributes.Length == 0)
			{
				// Try the interfaces
				var interfaces = inType.GetInterfaces();
				foreach (var info in interfaces.Select(type => type.GetProperty(propertyInfo.Name))
													.Where(info => info != null && info != propertyInfo))
				{
					attributes = info.GetAttributes<T>(inType);
					if (attributes.Length > 0)
						return attributes as T[];
				}
			}

			return attributes as T[];
		}

		public static TransformableToAttribute FindAttribute(this TransformableToAttribute[] attributes, MapToAttribute mapToAttribute)
		{
			return attributes.Length == 1 ? attributes[0]
										  : attributes.FirstOrDefault(transformableToAttribute => (transformableToAttribute.Alias != null) &&
																								  (transformableToAttribute.Alias.CompareTo(mapToAttribute.Alias) == 0));
		}

		public static TransformableToAttribute FindType<TOut>(this TransformableToAttribute[] attributes)
			where TOut : class
		{
			var rootType = typeof(TOut);
			foreach (var transformableToAttribute in
				attributes.Where(transformableToAttribute => (transformableToAttribute.Type.IsSubclassOf(rootType)) ||
															 (transformableToAttribute.Type == rootType)))
			{
				return transformableToAttribute;
			}

			return rootType.IsAbstract ? attributes[0] : null;
		}

		public static MapToAttribute FindMapAttribute(this MapToAttribute[] mapToAttributes, TransformableToAttribute transformableToAttribute)
		{
			MapToAttribute found = null;
			foreach (var mapToAttribute in mapToAttributes)
			{
				if (mapToAttribute.DoesHandle(transformableToAttribute) &&
					((found == null) || (found.AsAlias == false)))
				{
					found = mapToAttribute;

					// Stop if the attribute has an alias
					if (found.AsAlias) return found;
				}
			}

			// Return the found on
			return found;
		}

		public static bool DoesHandle(this MapToAttribute mapToAttribute, TransformableToAttribute transformableToAttribute)
		{
			return mapToAttribute.AsAlias ? transformableToAttribute.Alias.CompareTo(mapToAttribute.Alias) == 0 : true;
		}
	}
}

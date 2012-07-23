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
using System.Reflection;
using DTF.Attributes;
using DTF.Attributes.Extensions;

namespace DTF
{
	public static class Transform
	{
		public static TOut Tranform<TOut>(this ITransformable objectToTransform)
			where TOut : class, new()
		{
			// Get the attributes
			Type inType = objectToTransform.GetType();
			var attributes = inType.GetAttributes<TransformableToAttribute>();
			if (attributes.Length == 0) return null;

			// Init results
			Type outType = typeof(TOut);
			var transformedObject = new TOut();

			// For all properties
			InitTarget(objectToTransform, outType, transformedObject, inType, attributes);

			return transformedObject;
		}

		private static TOut InitTarget<TOut>(object objectToTransform, Type outType, TOut transformedObject, Type inType,
										   TransformableToAttribute[] attributes) where TOut : class, new()
		{
			// For each property
			var properties = inType.GetProperties();
			foreach (var property in properties)
			{
				// Get the mapping attributes, if no attributes, move on to the next property
				var mapAttributes = property.GetAttributes<MapToAttribute>();
				if (mapAttributes.Length == 0) continue;

				foreach (var mapToAttribute in mapAttributes)
				{
					// Find the TransformableToAttribute, if not continue to the next one
					var transformableToAttribute = attributes.FindAttribute(mapToAttribute);
					if (transformableToAttribute == null) continue;

					// Get the target property, if not continue to the next one
					var targetProperty = outType.GetProperty(mapToAttribute.Target);
					if (targetProperty == null) continue;

					// Get the value and set the target property
					var value = GetPropertyValue(mapToAttribute, property, objectToTransform);
					targetProperty.SetValue(transformedObject, value, null);
				}
			}

			return transformedObject;
		}

		private static object GetPropertyValue(MapToAttribute mapAttribute, PropertyInfo sourceProperty, object sourceObject)
		{
			var sourcePropertyValue = sourceProperty.GetValue(sourceObject, null);
			
			// Get attributes from the source value type if not null otherwise use the return type
			Type returnType = sourcePropertyValue != null ? sourcePropertyValue.GetType() 
													 : sourceProperty.GetGetMethod().ReturnType;

			var attributes = returnType.GetAttributes<TransformableToAttribute>();

			// No attributes, just return the value
			if (attributes.Length == 0) return sourceProperty.GetValue(sourceObject, null);
			
			// Create the target instance
			Type outType;
			if (mapAttribute.TargetType != null)
			{
				outType = mapAttribute.TargetType;
			}
			else
			{
				TransformableToAttribute attribute = attributes.FindAttribute(mapAttribute);
				outType = attribute.TargetType;
			}

			return outType.IsEnum ? GetInstance(sourceProperty, sourceObject, returnType, outType)
								  : GetInstance(sourceProperty, sourceObject, returnType, outType, attributes);
		}

		private static object GetInstance(PropertyInfo sourceProperty, object sourceObject, Type returnType, Type outType, TransformableToAttribute[] attributes)
		{
			return InitTarget(sourceProperty.GetValue(sourceObject, null), outType, Activator.CreateInstance(outType), returnType, attributes);
		}

		private static object GetInstance(PropertyInfo sourceProperty, object sourceObject, Type returnType, Type outType)
		{
			object value = sourceProperty.GetValue(sourceObject, null);
			var member = returnType.GetMember(value.ToString());
			var enumMapToAttributes = member[0].GetAttributes<EnumMapToAttribute>();
			return enumMapToAttributes.Length == 0 ? Activator.CreateInstance(outType) : enumMapToAttributes[0].TargetValue;
		}
	}
}

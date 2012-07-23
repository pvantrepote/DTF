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
		public static TOut Tranform<TOut>(this object objectToTransform)
			where TOut : class
		{
			// Get the attributes
			Type inType = objectToTransform.GetType();
			var attributes = inType.GetAttributes<TransformableToAttribute>();
			if (attributes.Length == 0) return null;

			// Use the attribute first
			TransformableToAttribute transformableToAttribute = attributes.FindType<TOut>();
			var transformedObject = Activator.CreateInstance(transformableToAttribute.TargetType);

			// Setup the targer
			SetTargetProperties(objectToTransform, transformableToAttribute.TargetType, transformedObject, inType, transformableToAttribute);

			// Ok its done, now check if the user request to be called 
			if (transformableToAttribute.DefaultValueProvider != null)
			{
				var method = objectToTransform.GetType().GetMethod(transformableToAttribute.DefaultValueProvider);
				if (method != null)
				{
					method.Invoke(objectToTransform, new[] { transformedObject });			
				}
			}
	
			return transformedObject as TOut;
		}

		private static TOut SetTargetProperties<TOut>(object objectToTransform, Type outType, TOut transformedObject, Type inType,
										   TransformableToAttribute attribute) where TOut : class, new()
		{
			// For each property
			var properties = inType.GetProperties();
			foreach (var property in properties)
			{
				// Get the mapping attributes, if no attributes, move on to the next property
				var mapAttributes = property.GetAttributes<MapToAttribute>(inType);
				if (mapAttributes.Length == 0) continue;

				// Get the proper mapping attribute from the Transformable attribute
				var mapToAttribute = mapAttributes.FindMapAttribute(attribute);
				if (mapToAttribute == null) continue;

				// Get the target property, if not continue to the next one
				var targetProperty = outType.GetProperty(mapToAttribute.Target);
				if (targetProperty == null) continue;

				// Get the value and set the target property
				var value = GetPropertyValue(mapToAttribute, property, objectToTransform);
				targetProperty.SetValue(transformedObject, value, null);
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
			TransformableToAttribute attribute = null;
			if (mapAttribute.TargetType != null)
			{
				outType = mapAttribute.TargetType;
			}
			else
			{
				attribute = attributes.FindAttribute(mapAttribute);
				outType = attribute.TargetType;
			}

			return GetInstance(sourceProperty, sourceObject, returnType, outType, attribute);
		}

		private static object GetInstance(PropertyInfo sourceProperty, object sourceObject, Type returnType, Type outType, TransformableToAttribute attribute)
		{
			return returnType.IsEnum ? GetEnumInstance(sourceProperty, sourceObject, returnType, outType) 
									 : SetTargetProperties(sourceProperty.GetValue(sourceObject, null), outType, Activator.CreateInstance(outType), returnType, attribute);
		}

		private static object GetEnumInstance(PropertyInfo sourceProperty, object sourceObject, Type returnType, Type outType)
		{
			object value = sourceProperty.GetValue(sourceObject, null);
			var member = returnType.GetMember(value.ToString());
			var enumMapToAttributes = member[0].GetAttributes<EnumMapToAttribute>();
			return enumMapToAttributes.Length == 0 ? Activator.CreateInstance(outType) : enumMapToAttributes[0].TargetValue;
		}
	}
}

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
			var transformedObject = Activator.CreateInstance(transformableToAttribute.Type);

			// Setup the targer
			SetTargetProperties(inType, objectToTransform, transformableToAttribute.Type, transformedObject, transformableToAttribute);

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

		private static TOut SetTargetProperties<TOut>(Type objectType, object objectToTransform, 
													  Type targetReturnType, TOut transformedObject,
													  TransformableToAttribute attribute) where TOut : class, new()
		{
			// For each property
			var properties = objectType.GetProperties();
			foreach (var property in properties)
			{
				// Get the mapping attributes, if no attributes, move on to the next property
				var mapAttributes = property.GetAttributes<MapToAttribute>(objectType);
				if (mapAttributes.Length == 0) continue;

				// Get the proper mapping attribute from the Transformable attribute
				var mapToAttribute = mapAttributes.FindMapAttribute(attribute);
				if (mapToAttribute == null) continue;

				// For now we support only MapToProperty
				if ((mapToAttribute is MapToPropertyAttribute) == false)
					continue;

				// Get the target property, if not continue to the next one
				var mapToPropertyAttribute = (MapToPropertyAttribute) mapToAttribute;
				var target = mapToPropertyAttribute.Name;

				// If no name is provided, use the same as the source property
				var targetProperty = (string.IsNullOrEmpty(target)) ? targetReturnType.GetProperty(property.Name) 
																	: targetReturnType.GetProperty(target);
				if (targetProperty == null) continue;

				// Get the value and set the target property
				var value = GetPropertyValue(mapToAttribute, property, objectToTransform);

				// Check if the user specified a processing method
				if (string.IsNullOrEmpty(mapToAttribute.ValueProcessingMethod) == false)
				{
					var methodInfo = objectType.GetMethod(mapToAttribute.ValueProcessingMethod);
					if (methodInfo != null)
					{
						value = methodInfo.Invoke(objectToTransform, new[] {value, property});
					}
				}

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
			if (attributes.Length == 0)
			{
				return (mapAttribute is MapToValueAttribute) ? ((MapToValueAttribute) mapAttribute).Value 
															 : sourcePropertyValue;
			}
				
			// Create the target instance
			Type targetPropertyType;
			TransformableToAttribute attribute = null;
			if (mapAttribute is MapToClassAttribute)
			{
				targetPropertyType = (mapAttribute as MapToClassAttribute).TargetType;
			}
			else
			{
				attribute = attributes.FindAttribute(mapAttribute);
				targetPropertyType = attribute.Type;
			}

			if (mapAttribute is MapToValueAttribute)
			{
				return ((MapToValueAttribute) mapAttribute).Value;
			}

			// Special for enum (for now, may be more later)
			return returnType.IsEnum ? GetEnumValue(sourcePropertyValue, returnType, targetPropertyType) 
									 : GetInstance(returnType, sourcePropertyValue, targetPropertyType, attribute);
		}

		private static object GetInstance(Type sourceReturnType, object sourcePropertyValue, Type targetPropertyType, TransformableToAttribute attribute)
		{
			return SetTargetProperties(sourceReturnType, sourcePropertyValue, targetPropertyType, Activator.CreateInstance(targetPropertyType), attribute);
		}

		private static object GetEnumValue(object sourcePropertyValue, Type sourcePropertyReturnType, Type targetPropertyReturnType)
		{
			var member = sourcePropertyReturnType.GetMember(sourcePropertyValue.ToString());
			var enumMapToAttributes = member[0].GetAttributes<MapToValueAttribute>();
			return enumMapToAttributes.Length == 0 ? Activator.CreateInstance(targetPropertyReturnType) : enumMapToAttributes[0].Value;
		}
	}
}

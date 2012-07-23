using System;
using System.Linq;
using System.Reflection;
using DTF.Attributes;

namespace DTF
{
	public class Transform
	{
		public static TOut Tranform<TOut>(object objectToTransform)
			where TOut : class, new()
		{
			// Get the attributes
			Type inType = objectToTransform.GetType();
			var attributes = (TransformableToAttribute[])inType.GetCustomAttributes(typeof(TransformableToAttribute), true);
			if (attributes.Length == 0) return null;

			// Init results
			Type outType = typeof(TOut);
			var transformedObject = new TOut();

			// For all properties
			InitTarget(objectToTransform, outType, transformedObject, inType, attributes);

			return transformedObject;
		}

		private static void InitTarget<TOut>(object objectToTransform, Type outType, TOut transformedObject, Type inType,
		                                   TransformableToAttribute[] attributes) where TOut : class, new()
		{
			var properties = inType.GetProperties();
			foreach (var property in properties)
			{
				var mapAttributes = (MapToAttribute[]) property.GetCustomAttributes(typeof (MapToAttribute), true);
				if (mapAttributes.Length == 0) continue;

				foreach (var mapToAttribute in mapAttributes)
				{
					// Find the TransformableToAttribute, if not continue to the next one
					var transformableToAttribute = FindTransformable(attributes, mapToAttribute);
					if (transformableToAttribute == null) continue;

					// Get the target property, if not continue to the next one
					var targetProperty = outType.GetProperty(mapToAttribute.Target);
					if (targetProperty == null) continue;

					// Get the value and set the target property
					var value = GetPropertyValue(mapToAttribute, property, objectToTransform);
					targetProperty.SetValue(transformedObject, value, null);
				}
			}
		}

		private static object GetPropertyValue(MapToAttribute mapAttribute, PropertyInfo sourceProperty, object sourceObject)
		{
			Type returnType = sourceProperty.GetGetMethod().ReturnType;

			// Get the attribute
			var attributes = (TransformableToAttribute[])returnType.GetCustomAttributes(typeof(TransformableToAttribute), true);
			if (attributes.Length == 0) return sourceProperty.GetValue(sourceObject, null);

			// Create the target instance
			Type outType;
			if (mapAttribute.TargetType != null)
			{
				outType = mapAttribute.TargetType;
			}
			else
			{
				TransformableToAttribute attribute = FindTransformable(attributes, mapAttribute);
				outType = attribute.TargetType;
			}

			object instance;

			// Enum
			if (outType.IsEnum)
			{
				object value = sourceProperty.GetValue(sourceObject, null);
				var member = returnType.GetMember(value.ToString());
				var enumMapToAttributes = (EnumMapToAttribute[])member[0].GetCustomAttributes(typeof(EnumMapToAttribute), true);
				instance = enumMapToAttributes.Length == 0 ? Activator.CreateInstance(outType) : enumMapToAttributes[0].TargetValue;
			}
			else
			{
				instance = Activator.CreateInstance(outType);

				// Init it
				InitTarget(sourceProperty.GetValue(sourceObject, null), outType, instance, returnType, attributes);				
			}


			return instance;
		}

		private static TransformableToAttribute FindTransformable(TransformableToAttribute[] attributes, MapToAttribute mapToAttribute)
		{
			return attributes.Length == 1 ? attributes[0] 
										  : attributes.FirstOrDefault(transformableToAttribute => (transformableToAttribute.Alias != null) && 
																								  (transformableToAttribute.Alias.CompareTo(mapToAttribute.AsAlias) == 0));
		}
	}
}

using System;

namespace DTF.Attributes
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
	public class MapToPropertyAttribute : MapToAttribute
	{
		/// <summary>
		/// The target value or property name
		/// </summary>
		public string Name { get; set; }
	}
}

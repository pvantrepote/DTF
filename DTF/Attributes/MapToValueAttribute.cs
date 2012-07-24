using System;

namespace DTF.Attributes
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
	public class MapToValueAttribute : MapToPropertyAttribute
	{
		#region Properties

		/// <summary>
		/// Target enumeration value
		/// </summary>
		public object Value { get; set; }

		#endregion
	}
}

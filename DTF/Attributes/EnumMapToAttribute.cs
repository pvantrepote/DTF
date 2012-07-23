using System;

namespace DTF.Attributes
{

	[AttributeUsage(AttributeTargets.Field)]
	public class EnumMapToAttribute : Attribute
	{
		#region Properties

		public object TargetValue { get; set; }
		public string Alias { get; set; }
		public bool AsAlias
		{
			get { return !string.IsNullOrEmpty(Alias); }
		}

		#endregion

		#region Constructors

		public EnumMapToAttribute(object targetValue)
		{
			TargetValue = targetValue;
		}

		public EnumMapToAttribute(string alias, object targetValue)
		{
			Alias = alias;
			TargetValue = targetValue;
		}

		#endregion
	}
}

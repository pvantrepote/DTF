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

		public MapToAttribute(string alias, string target)
		{
			Alias = alias;
			Target = target;
		}

		#endregion

	}
}

using System;

namespace DTF.Attributes
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Struct)]
	public class TransformableToAttribute : Attribute
	{
		#region Properties

		public Type TargetType { get; set; }
		public String Alias { get; set; }

		#endregion

		#region Constructors

		public TransformableToAttribute(Type targetType)
			: this(targetType, targetType.Name)
		{
			
		}

		public TransformableToAttribute(Type targetType, String alias)
		{
			Alias = alias;
			TargetType = targetType;
		}
	
		#endregion
		 

	}
}

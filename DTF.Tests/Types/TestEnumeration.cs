
using DTF.Attributes;

namespace DTF.Tests.Types
{
	[TransformableTo(Type = typeof(TargetEnum))]
	public enum TestEnumeration
	{
		[MapToValue(Value = TargetEnum.Four)]
		One,
		[MapToValue(Value = TargetEnum.Five)]
		Two,
		[MapToValue(Value = TargetEnum.Six)]
		Three,
	}

	public enum TargetEnum
	{
		Four,
		Five,
		Six,
	}
}

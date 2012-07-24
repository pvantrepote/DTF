
using DTF.Attributes;

namespace DTF.Tests.Types
{
	[TransformableTo(typeof(TargetEnum))]
	public enum TestEnumeration
	{
		[MapTo(Target = TargetEnum.Four)]
		One,
		[MapTo(Target = TargetEnum.Five)]
		Two,
		[MapTo(Target = TargetEnum.Six)]
		Three,
	}

	public enum TargetEnum
	{
		Four,
		Five,
		Six,
	}
}

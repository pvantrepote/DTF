using DTF.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DTF.Tests
{
	[TransformableTo(typeof(EnumType2))]
	public enum EnumType1
	{
		[EnumMapTo(EnumType2.Four)]
		One,
		[EnumMapTo(EnumType2.Five)]
		Two,
		[EnumMapTo(EnumType2.Six)]
		Three,
	}

	[TransformableTo(typeof(Type2))]
	public class Type1
	{
		[MapTo("Field1")]
		public string Field2 { get; set; }

		[MapTo("Field2")]
		public string Field1 { get; set; }

		[MapTo("NType2")]
		public NestedType1 NType1 { get; set; }

		[MapTo("Simple2")]
		public int Simple { get; set; }

		[MapTo("Enum2")]
		public EnumType1 Enum1 { get; set; }
	}

	[TransformableTo(typeof(NestedType2))]
	public class NestedType1
	{
		[MapTo("Field1")]
		public string Field1 { get; set; }
		
		[MapTo("Field2")]
		public string Field2 { get; set; }
	}

	public class Type1B : Type1
	{
	}

	public class Type2
	{
		public string Field1 { get; set; }
		public string Field2 { get; set; }
		public NestedType2 NType2 { get; set; }
		public EnumType2 Enum2 { get; set; }
		public int Simple2 { get; set; }
	}

	public class NestedType2
	{
		public string Field1 { get; set; }
		public string Field2 { get; set; }
	}

	public class NestedType2B : NestedType2
	{
	}

	public enum EnumType2
	{
		Four,
		Five,
		Six,
	}

	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void TestMethod1()
		{

			Type2 res = Transform.Tranform<Type2>(new Type1B
			                                      	{
			                                      		Field1 = "AAA", 
														Field2 = "BBB",
														NType1 = new NestedType1
														         	{
														         		Field1 = "NT1Field1",
																		Field2 = "NT2Field2"
														         	},
														Enum1 = EnumType1.Two,
														Simple =  12
			                                      	});

		}
	}
}

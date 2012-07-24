﻿/*  This program is free software: you can redistribute it and/or modify
	it under the terms of the GNU General Public License as published by
	the Free Software Foundation, either version 3 of the License, or
	(at your option) any later version.

	This program is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.

	You should have received a copy of the GNU General Public License
	along with this program.  If not, see <http://www.gnu.org/licenses/>. */

using System;
using DTF.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DTF.Tests
{
	[TransformableTo(typeof(EnumType2))]
	public enum EnumType1
	{
		[MapTo(Target = EnumType2.Four)]
		One,
		[MapTo(Target = EnumType2.Five)]
		Two,
		[MapTo(Target = EnumType2.Six)]
		Three,
	}

	[TransformableTo(typeof(Type2))]
	public interface IType1
	{
		[MapTo(Target = "Bla")]
		string Field2 { get; set; }
	}

	public interface IType1B : IType1
	{}
	
	public class Type1 : IType1
	{
		public string Field2 { get; set; }

		[MapTo(Target = "Field2")]
		public string Field1 { get; set; }

		[MapTo(Target = "NType2")]
		public NestedType1 NType1 { get; set; }

		[MapTo(Target = "NType2B")]
		public virtual NestedType1 NType1B { get; set; }

		[MapTo(Target = "Simple2")]
		public int Simple { get; set; }

		[MapTo(Target = "Enum2")]
		public EnumType1 Enum1 { get; set; }
	}

	[TransformableTo(typeof(NestedType2))]
	public class NestedType1
	{
		[MapTo(Target = "Field1")]
		public string Field1 { get; set; }

		[MapTo(Target = "Field2")]
		public string Field2 { get; set; }
	}

	[TransformableTo(typeof(NestedType2B))]
	public class NestedType1B : NestedType1{}

	[TransformableTo(typeof(Type2B), DefaultValueProvider = "Test")]
	public class Type1B : Type1, IType1B
	{
		public static void Test(Type2B instance)
		{
			Console.WriteLine("...");
		}
	}

	public class Type2
	{
		public string Field1 { get; set; }
		public string Field2 { get; set; }
		public NestedType2 NType2 { get; set; }
		public NestedType2 NType2B { get; set; }
		public EnumType2 Enum2 { get; set; }
		public int Simple2 { get; set; }
	}

	public class Type2B : Type2 {}

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
			var instance = new Type1B
			                  	{
			                  		Field1 = "AAA",
			                  		Field2 = "BBB",
			                  		NType1 = new NestedType1
			                  		         	{
			                  		         		Field1 = "NT1Field1",
			                  		         		Field2 = "NT2Field2"
			                  		         	},
									NType1B = new NestedType1B
									          	{
									          		Field1 = "NTB1Field1",
			                  		         		Field2 = "NTB2Field2"
									          	},
			                  		Enum1 = EnumType1.Two,
			                  		Simple = 12
			                  	};

			var res = instance.Tranform<Type2>();

		}
	}
}

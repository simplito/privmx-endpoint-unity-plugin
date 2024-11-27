// Module name: SimplitoPrivmx
// File name: Union.cs
// Last edit: 2024-11-24 21:29
// Copyright (c) Simplito sp. z o.o.
// 
// This file is part of Simplito PrivMX Unity plugin under MIT License.

#nullable enable
using System;

namespace Simplito.Utils
{
	// [StructLayout(LayoutKind.Explicit)]
	public readonly struct Union<T1, T2>
	{
		private const byte IsFirstType = 1;

		private const byte IsSecondType = 2;

		// [FieldOffset(0)]
		private readonly byte _elementSet;

		// [FieldOffset(1)]
		private readonly T1 _first;

		// [FieldOffset(1)]
		private readonly T2 _second;

		public Type? ElementType
		{
			get
			{
				switch (_elementSet)
				{
					case IsFirstType:
						return typeof(T1);
					case IsSecondType:
						return typeof(T2);
					default:
						return null;
				}
			}
		}

		public Union(T1 element)
		{
			_elementSet = IsFirstType;
			_second = default!;
			_first = element;
		}

		public Union(T2 element)
		{
			_elementSet = IsSecondType;
			_first = default!;
			_second = element;
		}

		public bool Is<T>()
		{
			switch (typeof(T))
			{
				case T1:
					return _elementSet == IsFirstType;
				case T2:
					return _elementSet == IsSecondType;
				default:
					return false;
			}
		}

		public void Match(Action<T1>? first = null, Action<T2>? second = null)
		{
			switch (_elementSet)
			{
				case IsFirstType:
					first?.Invoke(_first);
					break;
				case IsSecondType:
					second?.Invoke(_second);
					break;
				default:
					throw new InvalidCastException("Union was not initialized and doesn't hold any value.");
			}
		}

		public T Match<T>(Func<T1, T> first, Func<T2, T> second)
		{
			switch (_elementSet)
			{
				case IsFirstType:
					return first(_first);
				case IsSecondType:
					return second(_second);
				default:
					throw new InvalidCastException("Union was not initialized and doesn't hold any value.");
			}
		}

		public static explicit operator T1(Union<T1, T2> union)
		{
			if (union._elementSet == IsFirstType)
				return union._first;
			throw new InvalidCastException(
				$"Union doesn't hold a value of type {typeof(T1)} but of type {union.ElementType} instead.");
		}

		public static explicit operator T2(Union<T1, T2> union)
		{
			if (union._elementSet == IsFirstType)
				return union._second;
			throw new InvalidCastException(
				$"Union doesn't hold a value of type {typeof(T2)} but of type {union.ElementType} instead.");
		}
	}

	// [StructLayout(LayoutKind.Explicit)]
	public readonly struct Union<T1, T2, T3>
	{
		private const byte IsFirstType = 1;
		private const byte IsSecondType = 2;

		private const byte IsThirdType = 3;

		// [FieldOffset(0)]
		private readonly byte _elementSet;

		// [FieldOffset(1)]
		private readonly T1 _first;

		// [FieldOffset(1)]
		private readonly T2 _second;

		/// [FieldOffset(1)]
		private readonly T3 _third;

		public Type? ElementType
		{
			get
			{
				switch (_elementSet)
				{
					case IsFirstType:
						return typeof(T1);
					case IsSecondType:
						return typeof(T2);
					case IsThirdType:
						return typeof(T3);
					default:
						return null;
				}
			}
		}

		public Union(T1 element)
		{
			_elementSet = IsFirstType;
			_second = default!;
			_third = default!;
			_first = element;
		}

		public Union(T2 element)
		{
			_elementSet = IsSecondType;
			_third = default!;
			_first = default!;
			_second = element;
		}

		public Union(T3 element)
		{
			_elementSet = IsThirdType;
			_first = default!;
			_second = default!;
			_third = element;
		}

		public bool Is<T>()
		{
			switch (typeof(T))
			{
				case T1:
					return _elementSet == IsFirstType;
				case T2:
					return _elementSet == IsSecondType;
				case T3:
					return _elementSet == IsThirdType;
				default:
					return false;
			}
		}

		public void Match(Action<T1>? first = null, Action<T2>? second = null, Action<T3>? third = null)
		{
			switch (_elementSet)
			{
				case IsFirstType:
					first?.Invoke(_first);
					break;
				case IsSecondType:
					second?.Invoke(_second);
					break;
				case IsThirdType:
					third?.Invoke(_third);
					break;
				default:
					throw new InvalidCastException("Union was not initialized and doesn't hold any value.");
			}
		}

		public T Match<T>(Func<T1, T> first, Func<T2, T> second, Func<T3, T> third)
		{
			switch (_elementSet)
			{
				case IsFirstType:
					return first(_first);
				case IsSecondType:
					return second(_second);
				case IsThirdType:
					return third(_third);
				default:
					throw new InvalidCastException("Union was not initialized and doesn't hold any value.");
			}
		}

		public static explicit operator T1(Union<T1, T2, T3> union)
		{
			if (union._elementSet == IsFirstType)
				return union._first;
			throw new InvalidCastException(
				$"Union doesn't hold a value of type {typeof(T1)} but of type {union.ElementType} instead.");
		}

		public static explicit operator T2(Union<T1, T2, T3> union)
		{
			if (union._elementSet == IsFirstType)
				return union._second;
			throw new InvalidCastException(
				$"Union doesn't hold a value of type {typeof(T2)} but of type {union.ElementType} instead.");
		}

		public static explicit operator T3(Union<T1, T2, T3> union)
		{
			if (union._elementSet == IsFirstType)
				return union._third;
			throw new InvalidCastException(
				$"Union doesn't hold a value of type {typeof(T3)} but of type {union.ElementType} instead.");
		}
	}
}
// Created by Martin M
using System;

namespace SaveSystem
{
	[Flags]
	public enum TransformSaveKind
	{
		None	 = 0,
		Position = 1 << 0,
		Rotation = 1 << 1,
		Scale    =  1 << 2
	}
}
// Made by Martin M
using System;

[Serializable]
public enum JournalItemType
{
	Artifact = 1 << 0,
	Story    = 1 << 1,
	Song     = 1 << 2,
	Npc      = 1 << 3
}
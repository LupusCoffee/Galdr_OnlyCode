using System;
using UnityEngine;

namespace Journal.Attributes
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class JournalVisibleAttribute : PropertyAttribute
	{
		public JournalItemType JournalItemType { get; }
		
		public JournalVisibleAttribute(JournalItemType journalItemType)
		{
			JournalItemType = journalItemType;
		}
	}
}
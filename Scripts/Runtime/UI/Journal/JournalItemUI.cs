// Made by Martin M
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class JournalItemUI : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI _titleText;
	[SerializeField] private TextMeshProUGUI _descriptionText;
	[SerializeField] private Image _iconImage;
	[SerializeField] private Sprite _unknownIcon;
	[SerializeField] private Sprite _lockedIcon;
	[SerializeField] private Image _highlightImage; 

	[SerializeField] private JournalItem _associatedJournalItem;
	private bool _isCollected;
	private bool _isLatest;
	private bool _isUnlocked;

	private void Start()
	{
		SetCollected(false);
	}

	private void FixedUpdate()
	{
		// Check if journal item is null
		if (_associatedJournalItem == null) return;
		
		// Check if journal item is collected
		if (_associatedJournalItem.IsCollected() != _isCollected)
		{
			// Update journal item UI
			_isCollected = _associatedJournalItem.IsCollected();
			SetCollected(_isCollected);
		}
		
		// Check if journal item is latest
		if (_associatedJournalItem.IsLatest() != _isLatest)
		{
			// Update journal item UI
			_isLatest = _associatedJournalItem.IsLatest();
			SetHighlight(_isLatest);
		}
		
		// Check if journal item is unlocked
		if (_associatedJournalItem.IsUnlocked() != _isUnlocked)
		{
			// Update journal item UI
			_isUnlocked = _associatedJournalItem.IsUnlocked();
			SetUnlocked(_isUnlocked);
		}
	}

	private void SetCollected(bool isUnlocked)
	{
		SetUnknown(!isUnlocked);
	}
	
	private void SetUnlocked(bool isUnlocked)
	{
	}
	
	private void SetUnknown(bool isUnknown)
	{
		_titleText.text = isUnknown 
			? LanguageEncrypter.EncryptText(_associatedJournalItem.Title) 
			: _associatedJournalItem.Title;
		
		_descriptionText.text = isUnknown
			? LanguageEncrypter.EncryptText(_associatedJournalItem.Description)
			: _associatedJournalItem.Description;
		
		_iconImage.sprite = isUnknown 
			? _unknownIcon 
			: _associatedJournalItem.SpriteIcon;
		
		SetHighlight(!isUnknown && _isLatest);
	}

	private void SetHighlight(bool isHighlighted)
	{
		if (_highlightImage == null) return;
		_highlightImage.enabled = isHighlighted;
	}

	public void SetJournalItem(JournalItem journalItem)
	{
		// Set journal item data
		switch (journalItem.JournalType)
		{
			case JournalItemType.Song:
				_titleText.text = journalItem.SongData.Spell.GetName();
				_descriptionText.text = journalItem.Description; 
				break;
			case JournalItemType.Artifact:
				_titleText.text = journalItem.Title;
				_descriptionText.text = journalItem.Description;
				break;
		}
		
		if (_iconImage != null)
		{
			_iconImage.color = Color.white;
			_iconImage.sprite = journalItem.SpriteIcon;
		}
		
		_associatedJournalItem = journalItem;
	}
}
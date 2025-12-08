// Created by Martin M
using System;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SaveSystem
{
	public sealed class UniqueTransformIdentifier : MonoBehaviour
	{
		public string Id => _useManualId ? _manualId : _generatedId;
		
		[SerializeField]
		public string _generatedId;
		[SerializeField]
		public bool _useManualId;
		[SerializeField]
		public string _manualId;
		
		
	#if UNITY_EDITOR
		private void OnValidate()
		{
			if (string.IsNullOrEmpty(_generatedId))
			{
				GenerateNewGuid();
			}
		}

		public void GenerateNewGuid()
		{
			_generatedId = new StringBuilder()
				.Append(Guid.NewGuid().ToString("N"))
				.ToString();
		}
	#endif
	}
}
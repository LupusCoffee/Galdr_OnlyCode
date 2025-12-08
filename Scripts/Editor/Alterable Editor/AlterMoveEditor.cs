using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Alter_Move))]
public class AlterMoveEditor : Editor
{
    // Serialized properties for conditional fields
    private SerializedProperty alterPosition;
    private SerializedProperty alterRotation;
    private SerializedProperty alterScale;
    private SerializedProperty alternatePositionOffset;
    private SerializedProperty alternateRotationOffset;
    private SerializedProperty alternateScaleOffset;
    private SerializedProperty moveDuration;
    private SerializedProperty useScreenShake;
    private SerializedProperty isAdditive;
    private SerializedProperty onlyMoveOnce;
    private SerializedProperty enableColliderOnActivation;
    private SerializedProperty colliderToEnable;

    // Serialized properties for sounds
    private SerializedProperty soundEmitter;
    private SerializedProperty oneTimeEventOnInteraction;
    private SerializedProperty initialMoveSound;
    private SerializedProperty revertMoveSound;
    private SerializedProperty constantInitialMoveSoundPlay;
    private SerializedProperty constantRevertMoveSoundPlay;
    private SerializedProperty constantInitialMoveSoundStop;
    private SerializedProperty constantRevertMoveSoundStop;

    // State for foldout
    private bool showSounds = false;

    private void OnEnable()
    {
        // Link serialized properties
        alterPosition = serializedObject.FindProperty("alterPosition");
        alterRotation = serializedObject.FindProperty("alterRotation");
        alterScale = serializedObject.FindProperty("alterScale");
        alternatePositionOffset = serializedObject.FindProperty("alternatePositionOffset");
        alternateRotationOffset = serializedObject.FindProperty("alternateRotationOffset");
        alternateScaleOffset = serializedObject.FindProperty("alternateScaleOffset");
        moveDuration = serializedObject.FindProperty("moveDuration");
        useScreenShake = serializedObject.FindProperty("useScreenShake");
        isAdditive = serializedObject.FindProperty("isAdditive");
        onlyMoveOnce = serializedObject.FindProperty("onlyMoveOnce");
        enableColliderOnActivation = serializedObject.FindProperty("enableColliderOnActivation");
        colliderToEnable = serializedObject.FindProperty("colliderToEnable");

        soundEmitter = serializedObject.FindProperty("soundEmitter");
        oneTimeEventOnInteraction = serializedObject.FindProperty("oneTimeEventOnInteraction");
        initialMoveSound = serializedObject.FindProperty("initialMoveSound");
        revertMoveSound = serializedObject.FindProperty("revertMoveSound");
        constantInitialMoveSoundPlay = serializedObject.FindProperty("sound_moveLoopStart");
        constantRevertMoveSoundPlay = serializedObject.FindProperty("sound_revertLoopStart");
        constantInitialMoveSoundStop = serializedObject.FindProperty("sound_moveLoopEnd");
        constantRevertMoveSoundStop = serializedObject.FindProperty("sound_revertLoopEnd");
    }

    public override void OnInspectorGUI()
    {
        // Update the serialized object to reflect changes
        serializedObject.Update();

        // Options Section
        EditorGUILayout.LabelField("Options", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(alterPosition);
        EditorGUILayout.PropertyField(alterRotation);
        EditorGUILayout.PropertyField(alterScale);

        if(alterPosition.boolValue || alterRotation.boolValue || alterScale.boolValue)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Offsets", EditorStyles.boldLabel);
        }

        if (alterPosition.boolValue)
        {
            EditorGUILayout.PropertyField(alternatePositionOffset);
        }

        if (alterRotation.boolValue)
        {
            EditorGUILayout.PropertyField(alternateRotationOffset);
        }

        if (alterScale.boolValue)
        {
            EditorGUILayout.PropertyField(alternateScaleOffset);
        }

        // Extras Section
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Extras", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(moveDuration);
        EditorGUILayout.PropertyField(useScreenShake);
        EditorGUILayout.PropertyField(isAdditive);
        EditorGUILayout.PropertyField(onlyMoveOnce);

        // Collider Section
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Collider", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(enableColliderOnActivation);
        EditorGUILayout.PropertyField(colliderToEnable);

        // Sounds Section with Foldout
        EditorGUILayout.Space();
        showSounds = EditorGUILayout.Foldout(showSounds, "Sounds");
        if (showSounds)
        {
            EditorGUILayout.PropertyField(soundEmitter, new GUIContent("Sound Emitter"));
            EditorGUILayout.PropertyField(oneTimeEventOnInteraction, new GUIContent("One Time Event On Interaction"));
            EditorGUILayout.PropertyField(initialMoveSound, new GUIContent("Move Sound"));
            EditorGUILayout.PropertyField(revertMoveSound, new GUIContent("Revert Sound"));
            EditorGUILayout.PropertyField(constantInitialMoveSoundPlay, new GUIContent("Looping Move Sound Play"));
            EditorGUILayout.PropertyField(constantRevertMoveSoundPlay, new GUIContent("Looping Revert Sound Play"));
            EditorGUILayout.PropertyField(constantInitialMoveSoundStop, new GUIContent("Looping Move Sound Stop"));
            EditorGUILayout.PropertyField(constantRevertMoveSoundStop, new GUIContent("Looping Revert Sound Stop"));
        }

        // Apply modified properties
        serializedObject.ApplyModifiedProperties();
    }
}

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CharacterController))]
public class CharacterControllerInspector : Editor {


	public override void OnInspectorGUI()
	{
		CharacterController controller = (CharacterController)target;

		controller.isComponentsExpanded = EditorGUILayout.Foldout(controller.isComponentsExpanded, "Required Components");

		//Seperator
		GUILayout.Box("", new GUILayoutOption[]{GUILayout.ExpandWidth(true), GUILayout.Height(1)});

		if (controller.isComponentsExpanded)
		{
			controller.Components.animator  = (Animator)EditorGUILayout.ObjectField("Animator", controller.Components.animator, typeof(Animator), true);
			controller.Components.rigidBody = (Rigidbody)EditorGUILayout.ObjectField("Rigidbody", controller.Components.rigidBody, typeof(Rigidbody), true);
			controller.Components.rotationVectors  = (RotationVectors)EditorGUILayout.ObjectField("Rotation Vectors", controller.Components.rotationVectors, typeof(RotationVectors), true);
		}

		controller.MovementType = (CharacterController.MovementTypes) EditorGUILayout.EnumPopup("Type of Movement", controller.MovementType);

		controller.isPMovementExpanded = EditorGUILayout.Foldout(controller.isPMovementExpanded, "Movement Properties");
		if (controller.isPMovementExpanded)
		{
			switch (controller.MovementType)
			{
				case CharacterController.MovementTypes.Walker:
					controller.PMovement.horizontalSpeed = EditorGUILayout.FloatField("Horizontal Speed", controller.PMovement.horizontalSpeed);
					break;

				case CharacterController.MovementTypes.Flier:
					controller.PMovement.horizontalSpeed = EditorGUILayout.FloatField("Horizontal Speed", controller.PMovement.horizontalSpeed);
					controller.PMovement.verticalSpeed = EditorGUILayout.FloatField("Vertical Speed", controller.PMovement.verticalSpeed);
					break;
			}
		}

		if ( !controller.PJump.canJump )
			controller.PJump.canJump = EditorGUILayout.Toggle("Can Jump", controller.PJump.canJump);
		else
		{
			controller.isPJumpExpanded = EditorGUILayout.Foldout(controller.isPJumpExpanded, "Jump Properties");
			if ( controller.isPJumpExpanded )
			{
				controller.PJump.canJump = EditorGUILayout.Toggle("Can Jump", controller.PJump.canJump);
				controller.PJump.maxConsecutiveJumps = EditorGUILayout.IntField("Max Consecutive Jumps", controller.PJump.maxConsecutiveJumps);
				controller.PJump.jumpHeight = EditorGUILayout.FloatField("Jump Height", controller.PJump.jumpHeight);
			}
		}
		
	}


}

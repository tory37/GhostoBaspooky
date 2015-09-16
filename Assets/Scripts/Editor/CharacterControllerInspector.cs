using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CharacterController), true)]
public class CharacterControllerInspector : Editor {


	public override void OnInspectorGUI()
	{
		CharacterController controller = (CharacterController)target;

		EditorGUILayout.LabelField("Character Controller");

		controller.isComponentsExpanded = EditorGUILayout.Foldout(controller.isComponentsExpanded, "Required Components");

		if (controller.isComponentsExpanded)
		{
			controller.Components.animator  = (Animator)EditorGUILayout.ObjectField("Animator", controller.Components.animator, typeof(Animator), true);
			controller.Components.rigidBody = (Rigidbody)EditorGUILayout.ObjectField("Rigidbody", controller.Components.rigidBody, typeof(Rigidbody), true);
			controller.Components.rotationVectors  = (RotationVectors)EditorGUILayout.ObjectField("Rotation Vectors", controller.Components.rotationVectors, typeof(RotationVectors), true);
		}

		controller.MovementType = (CharacterController.MovementTypes) EditorGUILayout.EnumPopup("Type of Movement", controller.MovementType);

		switch ( controller.MovementType )
		{
			case CharacterController.MovementTypes.Hoverer:
			case CharacterController.MovementTypes.Jetpack:
			case CharacterController.MovementTypes.Walker:

				controller.isPMovementExpanded = EditorGUILayout.Foldout(controller.isPMovementExpanded, "Movement Properties");
				if ( controller.isPMovementExpanded )
				{
					controller.PMovement.horizontalSpeed = EditorGUILayout.FloatField("Horizontal Speed", controller.PMovement.horizontalSpeed);
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
						controller.PJump.jumpHeight = EditorGUILayout.IntField("Jump Height", controller.PJump.jumpHeight);
					}
				}

				break;

			case CharacterController.MovementTypes.Flier:

				controller.isPMovementExpanded = EditorGUILayout.Foldout(controller.isPMovementExpanded, "Movement Properties");
				if ( controller.isPMovementExpanded )
				{
					controller.PMovement.horizontalSpeed = EditorGUILayout.FloatField("Horizontal Speed", controller.PMovement.horizontalSpeed);
					controller.PMovement.verticalSpeed = EditorGUILayout.FloatField("Vertical Speed", controller.PMovement.verticalSpeed);
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
						controller.PJump.jumpHeight = EditorGUILayout.IntField("Jump Height", controller.PJump.jumpHeight);
					}
				}

				break;
		}

		controller.isPAnimationExpanded = EditorGUILayout.Foldout(controller.isPAnimationExpanded, "Animation Properties");

		EditorGUI.indentLevel++;

		if ( controller.isPAnimationExpanded )
		{
			foreach (CharacterController.AnimParamInfo item in controller.PAnimation.animationChecks)
			{
				item.isExpanded = EditorGUILayout.Foldout(item.isExpanded, item.paramDescription);
				if (item.isExpanded)
				{
					EditorGUI.indentLevel++;

					item.hasParam = EditorGUILayout.Toggle("Has Parameter?", item.hasParam);
					if ( item.hasParam )
						item.paramName = EditorGUILayout.TextField("Parameter Name", item.paramName);

					EditorGUI.indentLevel--;
				}
			}
		}

		EditorGUI.indentLevel--;

	}


}

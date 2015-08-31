using UnityEngine;
using System.Collections;

public class RotationVectors : MonoBehaviour {

    [Header("Rotation")]
    [Tooltip("The rotation of the character when pacing in positive x direction.")]
    public Vector3 positiveXRotation;
    [Tooltip("The rotation of the character when pacing in negative x direction.")]
    public Vector3 negativeXRoation;
    [Tooltip("The rotation of the character when pacing in positive y direction.")]
    public Vector3 positiveYRotation;
    [Tooltip("The rotation of the character when pacing in negative y direction.")]
    public Vector3 negativeYRotaion;
    [Tooltip("The rotation of the character when pacing in positive z direction.")]
    public Vector3 positiveZRotation;
    [Tooltip("The rotation of the character when pacing in negative z direction.")]
    public Vector3 negativeZRotaion;
    [Tooltip("The speed at which the character rotates towards the correct rotation.")]
    public float rotationVelocity;

}

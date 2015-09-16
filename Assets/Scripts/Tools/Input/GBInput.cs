using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GBInput : MonoBehaviour
{

	public static GBInput Instance
	{ get { return instance; } }
	private static GBInput instance;

	public List<string> inputNames;

	public Image dPad;
	public Image dPadCenter;

	private Dictionary<string, bool> buttonDown;
	private Dictionary<string, bool> buttonStay;
	private Dictionary<string, bool> buttonUp;

	private float vertical;
	private float horizontal;

	public void Start ()
	{
		instance = this;

		buttonDown = new Dictionary<string, bool>();
		buttonStay = new Dictionary<string, bool>();
		buttonUp = new Dictionary<string, bool>();

		for ( int i = 0; i < inputNames.Count; i++ )
		{
			buttonDown.Add( inputNames[ i ], false );
			buttonStay.Add( inputNames[ i ], false );
			buttonUp.Add( inputNames[ i ], false );
		}
	}

    /// <summary>
    /// Returns the value of the virtual axis identified by /axis/
    /// </summary>
    /// <param name="axis">The name of the axis</param>
    /// <returns></returns>
	public static float GetAxis ( string axis )
	{
		if ( axis == "Vertical" )
			return Instance.vertical;
		else if ( axis == "Horizontal" )
			return Instance.horizontal;

		Debug.LogError( "Axis " + "'" + axis + "' is not setup." );
		return 0f;
	}

    /// <summary>
    /// Returns true while the virtual button identified by /buttonName/ is held down
    /// </summary>
    /// <param name="buttonName"></param>
    /// <returns></returns>
	public static bool GetButton ( string buttonName )
	{
		try
		{
			if ( !Instance.buttonStay[ buttonName ] )
				return Input.GetButton( buttonName );
			else
				return Instance.buttonStay[ buttonName ];
		}
		catch
		{
			Debug.LogError( "Input " + "'" + buttonName + "' is not setup." );
			return false;
		}
	}

    /// <summary>
    /// Returns true the frame the user presses the button identified by /buttonName/
    /// </summary>
    /// <param name="buttonName"></param>
    /// <returns></returns>
	public static bool GetButtonDown ( string buttonName )
	{
		try
		{
			if ( !Instance.buttonDown[ buttonName ] )
				return Input.GetButtonDown( buttonName );
			else
				return Instance.buttonDown[ buttonName ];
		}
		catch
		{
			Debug.LogError( "Input " + "'" + buttonName + "' is not setup." );
			return false;
		}
	}

    /// <summary>
    /// Returns true the frame the user releases the button identified by /buttonName/
    /// </summary>
    /// <param name="buttonName"></param>
    /// <returns></returns>
	public static bool GetButtonUp ( string buttonName )
	{
		try
		{
			if ( !Instance.buttonUp[ buttonName ] )
				return Input.GetButtonUp( buttonName );
			else
				return Instance.buttonUp[ buttonName ];
		}
		catch
		{
			Debug.LogError( "Input " + "'" + buttonName + "' is not setup." );
			return false;
		}
	}

    /// <summary>
    /// Called by input not handled by Unity Input class, like touchscreen buttons. Sets the GetButtonDown to true.
    /// </summary>
    /// <param name="inputName"></param>
	public static void SetButtonDown ( string inputName )
	{
		try
		{
			Instance.buttonDown[ inputName ] = true;
			Instance.buttonStay[ inputName ] = true;
			Instance.StartCoroutine( CancelUpAndDown( inputName ) );
		}
		catch
		{
			Debug.LogError( "Input " + "'" + inputName + "' is not setup." );
		}
	}

    /// <summary>
    /// Called by input not handled by Unity Input class, like touchscreen buttons. Sets the GetButtonUp to true.
    /// </summary>
    /// <param name="inputName"></param>
	public static void SetButtonUp ( string inputName )
	{
		try
		{
			Instance.buttonUp[ inputName ] = true;
			Instance.buttonStay[ inputName ] = false;
			Instance.StartCoroutine( CancelUpAndDown( inputName ) );
		}
		catch
		{
			Debug.LogError( "Input " + "'" + inputName + "' is not setup." );
		}
	}

    /// <summary>
    /// Sets buttonUp and buttonDown to only be true for one frame;
    /// </summary>
    /// <param name="inputName"></param>
	public static IEnumerator CancelUpAndDown ( string inputName )
	{
		yield return null;

		Instance.buttonDown[ inputName ] = false;
		Instance.buttonUp[ inputName ] = false;
	}

	void Update ()
	{
		vertical = Input.GetAxis( "Vertical" );
		horizontal = Input.GetAxis( "Horizontal" );

		if ( Input.touchCount > 0 )
		{
			for ( int i = 0; i < Input.touches.Length; i++ )
			{
				if ( ( Input.touches[ i ].position.x < Screen.width / 2f ) )
				{
					Vector3 position;

					if ( Input.touches[ i ].phase == TouchPhase.Began )
					{
						RectTransformUtility.ScreenPointToWorldPointInRectangle( dPad.rectTransform, Input.touches[ i ].position, null, out position );

						dPad.transform.position = position;

						dPad.color = new Color( dPad.color.r, dPad.color.g, dPad.color.b, 175f );
						dPadCenter.color = new Color( dPadCenter.color.r, dPadCenter.color.g, dPadCenter.color.b, 175f );
						Debug.Log( "Touch Began. Position: " + position );
					}
					else if ( Input.touches[ i ].phase == TouchPhase.Ended )
					{
						dPad.color = new Color( dPad.color.r, dPad.color.g, dPad.color.b, 0f );
						dPadCenter.color = new Color( dPadCenter.color.r, dPadCenter.color.g, dPadCenter.color.b, 0f );
					}
					else if ( Input.touches[ i ].phase == TouchPhase.Moved )
					{
						RectTransformUtility.ScreenPointToWorldPointInRectangle( dPad.rectTransform, Input.touches[ i ].position, null, out position );
						dPadCenter.transform.position = position;
					}

					horizontal = Mathf.Clamp( ( Input.touches[ i ].position.x - RectTransformUtility.WorldToScreenPoint( null, dPad.transform.position ).x ) / 80, -1f, 1f );
					vertical = Mathf.Clamp( ( Input.touches[ i ].position.y - RectTransformUtility.WorldToScreenPoint( null, dPad.transform.position ).y ) / 80, -1f, 1f );
					//Debug.Log("Vertical = " + vertical + ", Horizontal = " + horizontal);
				}
			}
		}
	}
}

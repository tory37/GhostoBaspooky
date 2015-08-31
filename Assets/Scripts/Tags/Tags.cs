using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tags : MonoBehaviour
{

	[EnumFlag( "Tags" ), Tooltip( "The tags associated with this object" )]
	public TagSystem tags;

	Tags()
	{
		tags = new TagSystem();
	}

	///// <summary>
	///// A dictionary of specific tags and they're necessary components;
	///// </summary>
	//Dictionary<TagSystem, string> tagComponentDictionary = new Dictionary<TagSystem, string>()
	//{
	//	{TagSystem.HasController, "BaseController"}
	//};

	//Checks certain tags for components
	void Start()
	{
		if (transform.HasTag(TagSystem.HasController))
		{
			if (!GetComponent<BaseController>())
			{
				Debug.LogError( "There is no Base Controller component attached to " + transform + ", but it has tag HasController. Breaking.", transform );
				Debug.Break();
			}
		}
	}
}

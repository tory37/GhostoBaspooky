/// <summary>
/// A set of tags that any item that needs a tag uses.
/// These tags are used instead of Unity's built in tag system because 
/// any object can have none, any, or all of the tags at one time.
/// DO NOT DELETE ANY ITEMS. ADD NEW TO LIST, COMMENT OUT UNUSED.
/// </summary>
[System.Flags]
public enum TagSystem
{
    AIMovementNeedsTarget   = 1 << 0,
    Player                  = 1 << 1,
    Enemy                   = 1 << 2,
    Ghost                   = 1 << 3,
    Environment             = 1 << 4,
	AttackObject			= 1 << 5,
	Jumpable				= 1 << 6,
	HasController			= 1 << 7,
	DestructibleObject		= 1 << 8
}

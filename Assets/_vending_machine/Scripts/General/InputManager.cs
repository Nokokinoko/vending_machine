using UnityEngine;

public static class InputManager
{
	public static ENUM_TOUCH GetTouch()
	{
		if (Application.isEditor)
		{
			if (Input.GetMouseButtonDown(0)) { return ENUM_TOUCH.TOUCH_BEGAN; }
			if (Input.GetMouseButton(0)) { return ENUM_TOUCH.TOUCH_MOVED; }
			if (Input.GetMouseButtonUp(0)) { return ENUM_TOUCH.TOUCH_ENDED; }
		}
		else
		{
			if (0 < Input.touchCount)
			{
				return (ENUM_TOUCH)(int)Input.GetTouch(0).phase;
			}
		}
		return ENUM_TOUCH.TOUCH_NONE;
	}

	public static Vector2 GetPosition()
	{
		if (Application.isEditor)
		{
			if (GetTouch() != ENUM_TOUCH.TOUCH_NONE) { return Input.mousePosition; }
		}
		else
		{
			if (0 < Input.touchCount)
			{
				Touch _Touch = Input.GetTouch(0);
				return _Touch.position;
			}
		}
		return Vector2.zero;
	}
}

public enum ENUM_TOUCH
{
	TOUCH_NONE = -1,
	TOUCH_BEGAN = 0,
	TOUCH_MOVED = 1,
	TOUCH_STATIONARY = 2,
	TOUCH_ENDED = 3,
	TOUCH_CANCELED = 4,
}

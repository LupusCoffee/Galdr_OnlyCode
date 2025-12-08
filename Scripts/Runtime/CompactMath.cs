using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class CompactMath
{
    #region AbsRange
    [Tooltip("Returns a random float between -range and range.")]
    public static float AbsRange(float range)
    {
        float chosenNumber = UnityEngine.Random.Range(-range, range);
        return chosenNumber;
    }

    [Tooltip("Returns a random float between -range and range, but not less than min.")]
    public static float AbsRange(float range, float minimum)
    {
        float chosenNumber = UnityEngine.Random.Range(-range, range);
        while (Mathf.Abs(chosenNumber) < minimum)
        {
            chosenNumber = UnityEngine.Random.Range(-range, range);
        }

        return chosenNumber;
    }

    [Tooltip("Returns a random float between -range and range, but not less than min or in the exclude list.")]
    public static float AbsRange(float range, float minimum, List<float> exclude)
    {
        float chosenNumber = UnityEngine.Random.Range(-range, range);
        while (Mathf.Abs(chosenNumber) < minimum || exclude.Contains(chosenNumber))
        {
            chosenNumber = UnityEngine.Random.Range(-range, range);
        }

        return chosenNumber;
    }
    #endregion

    [Tooltip("Returns a random float between 0 and range.")]
    public static float UpTo(float range)
    {
        float chosenNumber = UnityEngine.Random.Range(0, range);
        return chosenNumber;
    }

    [Tooltip("Converts a float to a time string in the format MM:SS.")]
    public static string ConvertToTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time - minutes * 60);
        return string.Format("{00:00}:{01:00}", minutes, seconds);
    }

    [Tooltip("Returns true if the value is approximately equal to the target value within a certain range.")]
    public static bool IsApproximately(float value, float target, float maxDiff)
    {
        return Mathf.Abs(value - target) < maxDiff;
    }

    [Tooltip("Returns true if the value is approximately equal to the target value within a certain range.")]
    public static bool IsApproximately(Vector3 value, Vector3 target, float maxDiff)
    {
        return Vector3.Distance(value, target) < maxDiff;
    }

    [Tooltip("Lerps three colours together, allowing transitioning between them.")]
    public static Color MultiColorLerp(Color[] targetColours, float percent)
    {
        float divPercent = percent / 100;

        Color color = Color.white;

        int indents = 1 / targetColours.Length - 1;

        for (int i = 0; i < targetColours.Length; i++)
        {
            if (divPercent < indents * (i + 1))
            {
                color = Color.Lerp(targetColours[i], targetColours[i + 1], (divPercent - indents * i) / indents);
                break;
            }
        }

        return color;
    }

    [Tooltip("% Chance of being succesful, returns true if so.")]
    public static bool GetLucky(float chance)
    {
        return UnityEngine.Random.Range(0, 100) < chance;
    }

    [Tooltip("Gets a vector3 in the direction AWAY from the target")]
    public static Vector3 AwayFrom(Vector3 self, Vector3 target, bool nullifyY = false)
    {
        Vector3 temp = target - self;
        if (nullifyY) temp.y = 0;
        temp.Normalize();
        return temp;
    }

    #region Pos In Range
    [Tooltip("Gets a random position within a range of the origin, excluding a list of positions and a minimum distance.")]
    public static Vector3 RandomPosInRange(Vector3 origin, float range, List<Vector3> exclude = null, float minDistance = 0)
    {
        int maxAttempts = 10;
        Vector3 newPos = origin + new Vector3(AbsRange(range), 0, AbsRange(range));

        if (exclude == null) exclude = new List<Vector3>();

        while (Vector3.Distance(newPos, origin) < minDistance || IsPosInList(newPos, exclude, minDistance / 2) && maxAttempts > 0)
        {
            newPos = origin + new Vector3(AbsRange(range), 0, AbsRange(range));
            maxAttempts--;
        }

        return newPos;
    }

    private static bool IsPosInList(Vector3 pos, List<Vector3> list, float minDist)
    {
        foreach (Vector3 item in list)
        {
            if (Vector3.Distance(pos, item) < minDist)
            {
                return true;
            }
        }

        return false;
    }
    #endregion


    [Tooltip("Get a string suffix from a number.")]
    public static string GetSuffix(int number)
    {
        if (number >= 11 && number <= 13)
            return "th";

        switch (number % 10)
        {
            case 1: return "st";
            case 2: return "nd";
            case 3: return "rd";
            default: return "th";
        }
    }

    [Tooltip("Convert an integer into a string, such as 1 into first")]
    public static string ConvertIntToText(int number)
    {
        string[] numbers = new string[] { "Zeroth", "First", "Second", "Third", "Fourth", "Fifth", "Sixth", "Seventh", "Eighth", "Ninth" };
        return numbers[number];
    }

    [Tooltip("Get the item's rotation based on the total number of items and its index.")]
    public static float GetRotationFromWhole(int index, int totalCount, float totalAngle = 360)
    {
        return totalAngle / totalCount * index;
    }

    [Tooltip("Get a random object in the overlap sphere with matching tag")]
    public static GameObject RandomInOverlapSphere(Vector3 origin, float range, string tag)
    {
        // get random in overlapsphere with tag
        Collider[] colliders = Physics.OverlapSphere(origin, range);
        List<GameObject> objects = new List<GameObject>();

        foreach (Collider col in colliders)
        {
            if (col.gameObject.CompareTag(tag))
            {
                objects.Add(col.gameObject);
            }
        }

        if (objects.Count == 0) return null;
        return objects[0];
    }

    [Tooltip("Get all objects in the overlap sphere with matching tag")]
    public static List<GameObject> AllInOverlapSphere(Vector3 origin, float range, string tag, int maxCount = 0)
    {
        Debug.Log("Looking for objects with tag " + tag);
        int count = 0;
        // get random in overlapsphere with tag
        Collider[] colliders = Physics.OverlapSphere(origin, range);
        List<GameObject> objects = new List<GameObject>();

        foreach (Collider col in colliders)
        {
            if (col.gameObject.CompareTag(tag))
            {
                objects.Add(col.gameObject);
                count++;
            }

            if (maxCount != 0 && count >= maxCount) break;
        }

        Debug.Log("Found " + objects.Count + " objects with tag " + tag);
        if (objects.Count == 0) return null;
        return objects;
    }

    [Tooltip("Convert seconds to milliseconds.")]
    public static int SecondsToMilli(float seconds)
    {
        return Mathf.FloorToInt(seconds * 1000);
    }

    #region Flattening
    [Tooltip("Flatten the y value of a Vector3")]
    public static Vector3 FlattenPos(Vector3 pos)
    {
        return new Vector3(pos.x, 0, pos.z);
    }

    [Tooltip("Flatten the y value of a Transform's position")]
    public static Vector3 FlattenPos(Transform target)
    {
        return new Vector3(target.position.x, 0, target.position.z);
    }
    #endregion

    [Tooltip("Make a curve between start and end position based on distance and time.")]
    public static Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
    {
        Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

        var mid = Vector3.Lerp(start, end, t);
        return new Vector3(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t), mid.z);
    }

    public static float GetAngleFromScreenCenter(Vector2 vector2)
    {
        return Mathf.Atan2(vector2.y, vector2.x) * Mathf.Rad2Deg;
    }

    public static float GetCursorAngleFromCenter(Vector2 center)
    {
        Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        Vector2 cursorPos = new Vector2(center.x - screenCenter.x, center.y - screenCenter.y);
        return Mathf.Atan2(cursorPos.y, cursorPos.x) * Mathf.Rad2Deg;
    }

    [Tooltip("Get the selected index on a circle based on the rotation angle.")]
    public static int GetIndexFromCircle(int totalAvailable, float rotationAngle)
    {
        float angle = rotationAngle - (360 / totalAvailable * 2);
        if (angle < 0) angle = 360 - Mathf.Abs(angle);
        angle = 360 - angle;

        float anglePerItem = 360 / totalAvailable;
        int index = Mathf.FloorToInt(angle / anglePerItem);

        return index;
    }

    public static Vector3 OverrideChildScale(Vector3 parentScale, Vector3 ownScale)
    {
        return new Vector3(ownScale.x / parentScale.x, ownScale.y / parentScale.y, ownScale.z / parentScale.z);
    }

    public static Vector3 AbsVector3(float magnitude, float min = 0)
    {
        Vector3 temp = new Vector3(UnityEngine.Random.Range(-magnitude, magnitude), UnityEngine.Random.Range(-magnitude, magnitude), UnityEngine.Random.Range(-magnitude, magnitude));
        while (temp.magnitude < min)
        {
            temp = new Vector3(UnityEngine.Random.Range(-magnitude, magnitude), UnityEngine.Random.Range(-magnitude, magnitude), UnityEngine.Random.Range(-magnitude, magnitude));
        }

        return temp;
    }
}

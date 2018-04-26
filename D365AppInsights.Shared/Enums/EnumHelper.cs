using System;

public class EnumHelper
{
    public static T ParseEnum<T>(object o)
    {
        try
        {
            return (T)Enum.Parse(typeof(T), o.ToString());
        }
        catch (Exception)
        {
            return default(T); //Information
        }
    }
}
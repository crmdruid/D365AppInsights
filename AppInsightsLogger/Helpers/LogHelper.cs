using System;
using System.Reflection;

public class LogHelper
{
    public static string GetFullMethodName(MethodBase methodBase)
    {
        string operationName = "";

        if (methodBase != null)
        {
            operationName = methodBase.DeclaringType != null ?
                $"{methodBase.DeclaringType.FullName}.{methodBase.Name}" :
                $"{methodBase.Name}";
        }

        return operationName;
    }

    public static string GetMethodName(MethodBase methodBase)
    {
        string methodName = "";

        if (methodBase != null)
            methodName = methodBase.Name;

        return methodName;
    }

    public static string GenerateNewId()
    {
        const string base64Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

        var stringChars = new char[5];
        var random = new Random();

        for (int i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = base64Chars[random.Next(base64Chars.Length)];
        }

        return new string(stringChars);
    }
}
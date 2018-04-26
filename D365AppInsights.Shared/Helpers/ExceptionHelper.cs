using System;
using System.Collections.Generic;
using System.Diagnostics;

public class ExceptionHelper
{
    public static List<AiParsedStack> GetParsedStacked(Exception e)
    {
        if (string.IsNullOrEmpty(e.StackTrace))
            return null;

        List<AiParsedStack> parsedStacks = new List<AiParsedStack>();

        Exception currentException = e;
        while (currentException != null)
        {
            AiParsedStack parsedStack = ParseStackTrace(e);
            parsedStacks.Add(parsedStack);

            currentException = currentException.InnerException;
        }

        return parsedStacks;
    }

    private static AiParsedStack ParseStackTrace(Exception e)
    {
        StackTrace stackTrace = new StackTrace(e);
        StackFrame stackFrame = stackTrace.GetFrame(0);
        AiParsedStack aiParsedStack = new AiParsedStack

        {
            Method = stackFrame.GetMethod().Name,
            FileName = stackFrame.GetFileName(),
            Line = stackFrame.GetFileLineNumber()
        };

        return aiParsedStack;
    }
}
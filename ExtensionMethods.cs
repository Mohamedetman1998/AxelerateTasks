using System;

public static class ExtensionMethods
{
	public static CurveLoop AppendRange(List<Line> listOfLines)
	{
        CurveLoop curveLoop = new CurveLoop();
        foreach (var line in linesGiven)
        {
            curveLoop.Append(line);
        }
        return curveLoop;
    }
}

namespace MathsMurderSpike.core.Utility;

public static class Utilities
{
    public static string FormatAsTwoDigitString(int number)
    {
        var formattedString = number.ToString();
        if (formattedString.Length < 2)
            formattedString = "0" + formattedString;
        
        return formattedString;
    }
}
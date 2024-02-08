using System.Text;

namespace ECSFramework;

public struct ValueString
{
    private Memory<char> chars;
    private int stringLength;
    private StringBuilder stringBuilder;

    public ValueString()
    {
        int initialSize = 50;
        chars = new char[initialSize];
        stringLength = 0;
        stringBuilder = new StringBuilder();
    }

    public void Set(string value)
    {
        if (value.Length > chars.Length)
        {
            chars = new char[value.Length + 1];
        }

        var charSpan = chars.Span;
        for (int i = 0; i < value.Length; i++)
        {
            charSpan[i] = value[i];
        }
        stringLength = charSpan.Length;
    }

    public void Append(string value)
    {
        if (stringLength + value.Length > chars.Length)
        {
            chars = new char[stringLength + value.Length - 1];
        }

        var charSpan = chars.Span;
        for (int i = 0; i < value.Length; i++)
        {
            charSpan[stringLength + i] = value[i];
        }
        stringLength = charSpan.Length;
    }

    public override string ToString()
    {
        var span = chars.Span;
        for (int i = 0; i < stringLength; i++)
        {
            stringBuilder.Append(span[i]);
        }

        return stringBuilder.ToString();
    }

}
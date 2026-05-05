using System;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        float num = 12.375f;

        string ieee = FloatToIEEE754(num, true);
        Console.WriteLine(ieee);

        float back = IEEE754ToFloat(ieee);
        Console.WriteLine(back);

        int a = int.MaxValue;
        Console.WriteLine(unchecked(a + 1));

        long b = long.MaxValue;
        Console.WriteLine(unchecked(b + 1));

        Console.WriteLine(Add("9999", "1"));
        Console.WriteLine(Subtract("10000", "1"));
        Console.WriteLine(Multiply("123", "456"));
    }

    static string FloatToIEEE754(float num, bool pretty = false)
    {
        if (num == 0)
            return pretty ? "0 | 00000000 | 00000000000000000000000"
                          : "00000000000000000000000000000000";

        int sign = num < 0 ? 1 : 0;
        num = Math.Abs(num);

        int exponent = 0;

        while (num >= 2)
        {
            num /= 2;
            exponent++;
        }

        while (num < 1)
        {
            num *= 2;
            exponent--;
        }

        int biasedExp = exponent + 127;

        double fraction = num - 1;
        StringBuilder mantissa = new StringBuilder();

        for (int i = 0; i < 23; i++)
        {
            fraction *= 2;

            if (fraction >= 1)
            {
                mantissa.Append('1');
                fraction -= 1;
            }
            else
            {
                mantissa.Append('0');
            }
        }

        string expBinary = ToBinary(biasedExp, 8);

        if (pretty)
            return $"{sign} | {expBinary} | {mantissa}";

        return $"{sign}{expBinary}{mantissa}";
    }

    static float IEEE754ToFloat(string input)
    {
        input = input.Replace(" ", "").Replace("|", "");

        if (input.Length != 32)
            return 0;

        int sign = input[0] - '0';

        int exp = 0;
        for (int i = 1; i <= 8; i++)
        {
            exp = exp * 2 + (input[i] - '0');
        }
        exp -= 127;

        double mantissa = 1.0;
        double frac = 0.5;

        for (int i = 9; i < 32; i++)
        {
            if (input[i] == '1')
                mantissa += frac;

            frac /= 2;
        }

        double result = mantissa * Math.Pow(2, exp);

        if (sign == 1)
            result = -result;

        return (float)result;
    }

    static string ToBinary(int value, int bits)
    {
        char[] result = new char[bits];

        for (int i = bits - 1; i >= 0; i--)
        {
            result[i] = (value % 2 == 1) ? '1' : '0';
            value /= 2;
        }

        return new string(result);
    }

    static string Add(string a, string b)
    {
        if (!IsValid(a)) return "0";
        if (!IsValid(b)) return "0";

        int i = a.Length - 1;
        int j = b.Length - 1;
        int carry = 0;

        StringBuilder result = new StringBuilder();

        while (i >= 0 || j >= 0 || carry > 0)
        {
            int da = i >= 0 ? a[i] - '0' : 0;
            int db = j >= 0 ? b[j] - '0' : 0;

            int sum = da + db + carry;

            result.Insert(0, (char)(sum % 10 + '0'));
            carry = sum / 10;

            i--;
            j--;
        }

        return result.ToString();
    }

    static string Subtract(string a, string b)
    {
        if (!IsValid(a) || !IsValid(b)) return "0";

        int i = a.Length - 1;
        int j = b.Length - 1;
        int borrow = 0;

        StringBuilder result = new StringBuilder();

        while (i >= 0)
        {
            int da = a[i] - '0' - borrow;
            int db = j >= 0 ? b[j] - '0' : 0;

            if (da < db)
            {
                da += 10;
                borrow = 1;
            }
            else
            {
                borrow = 0;
            }

            result.Insert(0, (char)(da - db + '0'));

            i--;
            j--;
        }

        return TrimLeadingZeros(result.ToString());
    }

    static string Multiply(string a, string b)
    {
        if (!IsValid(a) || !IsValid(b)) return "0";

        int n = a.Length;
        int m = b.Length;

        int[] result = new int[n + m];

        for (int i = n - 1; i >= 0; i--)
        {
            for (int j = m - 1; j >= 0; j--)
            {
                int mul = (a[i] - '0') * (b[j] - '0');
                int sum = mul + result[i + j + 1];

                result[i + j + 1] = sum % 10;
                result[i + j] += sum / 10;
            }
        }

        StringBuilder sb = new StringBuilder();
        foreach (int digit in result)
        {
            if (!(sb.Length == 0 && digit == 0))
                sb.Append(digit);
        }

        return sb.Length == 0 ? "0" : sb.ToString();
    }

    static bool IsValid(string s)
    {
        if (string.IsNullOrEmpty(s)) return false;

        foreach (char c in s)
        {
            if (c < '0' || c > '9')
                return false;
        }

        return true;
    }

    static string TrimLeadingZeros(string s)
    {
        int i = 0;
        while (i < s.Length - 1 && s[i] == '0')
            i++;

        return s.Substring(i);
    }
}
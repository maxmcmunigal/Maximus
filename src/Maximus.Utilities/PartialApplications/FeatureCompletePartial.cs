namespace SharpETL
{
    using System;
    using System.Text.RegularExpressions;   

    class Program
    {
        static readonly Regex _regex = AdvancedRegexMatcher();

        static void Main() => RegexTest();

        static void RegexTest()
        {
            Console.WriteLine(Format("{0}\t{0,-10}\t{0:C2}\t{0,10:N3}", 42).Replace(' ', '_'));
            System.Console.WriteLine("-----");
            var template = "{0} {2:yyyy-MM-dd} {1}";

            var expectedFinal = string.Format(template, 42, TimeSpan.FromSeconds(50), DateTime.Now);
            
            // first partial application
            var result = Format(template, 42);
            Console.WriteLine("Pass 1: " + result);
            
            // second partial application
            result = Format(result, TimeSpan.FromSeconds(50));
            Console.WriteLine("Pass 2: " + result);
            
            // third partial application
            result = Format(result, DateTime.Now);
            Console.WriteLine("Pass 3: " + result);

            var resSinglePass = Format(template, 42, TimeSpan.FromSeconds(50), DateTime.Now);

            System.Console.WriteLine("Single Pass Result: " + resSinglePass);

            System.Console.WriteLine("\n\nExpected Final Result: {0}", expectedFinal);
        }

        static string Format(string template, params object[] args)
        {
            if (string.IsNullOrWhiteSpace(template)) return string.Empty;
            if (args.Length < 1) return template;

            return _regex.Replace(template, match => {
                
                if (!int.TryParse(match.Groups["i"].Value, out int ix))
                {
                    throw new ArgumentException("invalid formatter string.");
                }

                if (ix < 0) 
                {
                    throw new ArgumentOutOfRangeException(nameof(ix));
                }

                string fmt = match.Groups["f"].Value;
                string spc = match.Groups["s"].Value;
                
                if (ix < args.Length) 
                {
                    string t = "{0";
                    if (!spc.IsNullOrEmpty()) t += "," + spc;
                    if (!fmt.IsNullOrEmpty()) t += ":" + fmt;
                    t += '}';

                    return string.Format(t, args[ix]);
                }

                
                
                string res = "{" + (ix-args.Length);
                if (!spc.IsNullOrEmpty()) res += "," + spc;
                if (!fmt.IsNullOrEmpty()) res += ":" + fmt;
                res += "}";
                return res;                
            });
        }
        
        static Regex AdvancedRegexMatcher()
        {
            // this Regular Expression should match all valid string.Format placeholder tokens
            // example: {0}, {1:C2}, {1,10}, {1,-15:yyyy-MM-dd}
            // Groups:
            //    i: The 0 based, unsigned integer index of the argument referenced by this token
            //    s: *Optional* a signed integer value controlling the spacing of the resultant formatted value
            //                  Spacing is to be perfomed on the result of the call to ToString (including the optional format string)
            //                  A negative number indicates left-aligned text, while a positive number indicates right aligned text
            //                  The integer value indicates the final whitespace padded string length
            //                  example: Given the value 42 and the spacing argument  10 the result will be: "        42"
            //                  example: Given the value 42 and the spacing argument -10 the result will be: "42        "                                   
            //    f: *Optional* the format string that should be used when calling ToString on the argument referenced by <i>
            // Notes:
            //    - This regular expression is intended to be re-used so the RegexOption.Compiled flag is specified
            //    - This regular expression is culture invariant so the RegexOption.CultureInvariant flag is specified
            //    - Supports multi-line matches (RegexOption.Multiline flag)
            //    - Case insensitive (RegexOption.IgnoreCase flag)
            // TODO:
            //    - Properly handle escaped braces ({{}})
            //    - Benchmark performance characteristics to ensure reasonable runtime performance
            string pattern = @"{(?<i>[\d]+)(?:\,(?<s>\-?[\d]+))?(?:\:(?<f>[^}\r\n]+))?}";
            RegexOptions opts =
                  RegexOptions.CultureInvariant
                | RegexOptions.Multiline
                | RegexOptions.IgnoreCase
                | RegexOptions.Compiled;
            return new Regex(pattern, opts);
        }
    }

    internal static class Extensions
    {
        public static bool IsNullOrEmpty(this string source) => string.IsNullOrEmpty(source);
    }
}

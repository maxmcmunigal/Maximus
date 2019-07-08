using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Maximus.Utilities
{
    /// <summary>
    /// Convenience extension methods on the <see cref="String"/> type.
    /// </summary>
    public static class StringExtensions
    {
        public static bool IsNullOrWhiteSpace(this string source) => string.IsNullOrWhiteSpace(source);

        public static bool IsNullOrEmpty(this string source) => string.IsNullOrEmpty(source);


        private static readonly Regex _placeholderPattern
            = new Regex(@"\{[\d]\}", RegexOptions.CultureInvariant | RegexOptions.Compiled);

        public static string FormatWith(this string template, params object[] args)
        {
            // return empty string for empty source to prevent null ref exception
            if (string.IsNullOrWhiteSpace(template)) return string.Empty;

            // return source string if user didn't provide any format args
            if (args.Length < 1) return template;

            // MatchCollection containing all of the string.format style placeholders
            var matches = _placeholderPattern.Matches(template);

            // ignore extra args
            if (matches.Count <= args.Length) return string.Format(template, args);

            var buffer = new StringBuilder(template);


            var extraArgsIndex = 0;
            for (var i = 0; i < matches.Count; i++)
            {
                Match match = matches[i];

                if (i < args.Length)
                {
                    // apply arg for placeholder
                    buffer.Replace(match.Value, args[i].ToString());
                    extraArgsIndex++;
                }
                else
                {
                    // no arg for this index, re-assign the placeholder
                    buffer.Replace(match.Value, "{" + extraArgsIndex + "}");
                    extraArgsIndex++;
                }
            }

            return buffer.ToString();
        }

        static readonly Regex _regex = AdvancedRegexMatcher();

        public static string Format(this string template, params object[] args)
        {
            // return empty string if arg passed is null or blank
            if (string.IsNullOrWhiteSpace(template)) return string.Empty;
            
            // return template back to user if no args are passed
            if (args.Length < 1) return template;

            // processing if at least one args passed is valid 
            return _regex.Replace(template, match => {
                
                // check to see if match(s) count can be converted to int
                // if not throw exception
                if (!int.TryParse(match.Groups["i"].Value, out int ix))
                {
                    throw new ArgumentException("invalid formatter string.");
                }

                // verify that match(s) do not create negative int from TryParse
                if (ix < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(ix));
                }

                // store "f" and "s" match groups as string
                string fmt = match.Groups["f"].Value;
                string spc = match.Groups["s"].Value;

                // if number of "i" match(s) is less than the number of args
                // then modify value in braces
                if (ix < args.Length)
                {
                    string t = "{0";
                    if (!spc.IsNullOrEmpty()) t += "," + spc;
                    if (!fmt.IsNullOrEmpty()) t += ":" + fmt;
                    t += '}';

                    return string.Format(t, args[ix]);
                }

                // if number of "i" match(s) are greater than the number of args
                // then modify value in braces
                string res = "{" + (ix - args.Length);
                if (!spc.IsNullOrEmpty()) res += "," + spc;
                if (!fmt.IsNullOrEmpty()) res += ":" + fmt;
                res += "}";
                return res;
            });
        }

        public static string ReFormat(this string template, params object[] args)
        {
            return _regex.Replace(template, match => {

                // check to see if match(s) count can be converted to int
                // if not throw exception
                if (!int.TryParse(match.Groups["i"].Value, out int ix))
                {
                    throw new ArgumentException("invalid formatter string.");
                }

                // verify that match(s) do not create negative int from TryParse
                if (ix < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(ix));
                }

                // store "f" and "s" match groups as string
                string fmt = match.Groups["f"].Value;
                string spc = match.Groups["s"].Value;

                // if number of "i" match(s) is less than the number of args
                // then modify value in braces
                if (ix < args.Length)
                {
                    string t = "{0";
                    if (!spc.IsNullOrEmpty()) t += "," + spc;
                    if (!fmt.IsNullOrEmpty()) t += ":" + fmt;
                    t += '}';

                    return string.Format(t, args[ix]);
                }

                // if number of "i" match(s) are greater than the number of args
                // then modify value in braces
                string res = "{" + (ix - args.Length);
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

            // {(?<index>[\d]+)(?:\,(?<spacing>\-?[\d]+))?(?:\:(?<format>[^}\r\n]+))?}
            string pattern = @"{(?<i>[\d]+)(?:\,(?<s>\-?[\d]+))?(?:\:(?<f>[^}\r\n]+))?}";
            RegexOptions opts =
                  RegexOptions.CultureInvariant
                | RegexOptions.Multiline
                | RegexOptions.Compiled;
            return new Regex(pattern, opts);
        }

    }
}


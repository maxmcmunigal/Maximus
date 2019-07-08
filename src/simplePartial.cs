using System;
using System.Text;
using System.Text.RegularExpressions;

namespace SharpFormat
{
    class Program
    {
        static void Main()
        {
            
            var test1 = "{0} {1}".FormatWith(42, "Cake", "taco");

            System.Console.WriteLine(test1);

            var template = "{0} {1} {2}";
            var args = new object[] {42};
            var result = template.FormatWith(args);
            Console.WriteLine(result);

            var nextResult = result.FormatWith("Cake");
            System.Console.WriteLine(nextResult);
            nextResult = nextResult.FormatWith(DateTime.Today);
            System.Console.WriteLine(nextResult);

        }
    }


    public static class StringFormatExtensions
    {   
        // used to match string.Format style placeholders
        // ex. {0}
        // Note: does not support sizing operators (ie. {0,2})
        // Note: does not support custom format strings (ie. {0:C2})
        private static readonly Regex _placeholderPattern 
            = new Regex(@"\{[\d]+\}", RegexOptions.CultureInvariant | RegexOptions.Compiled);

        // This is an extremely naive implementation that doesn't even begin
        // to address the vast number of edge cases and special handling that
        // a method of this nature would require. This serves only as a minimum
        // viable proof-of-concept.
        public static string FormatWith(this string template, params object[] args)
        {
            // return "" for empty template to prevent null reference exceptions
            if (string.IsNullOrWhiteSpace(template)) return string.Empty;
            
            // return the template string if the user did not provide any format args;
            if (args.Length < 1) return template;
            
            // MatchCollection containing all of the string.format style placeholders
            // ex. {0}
            var matches = _placeholderPattern.Matches(template);
            
            // simple case - no special treatment needed
            // ignore extra args, only need special handling if there are more placeholders
            // than arguments.
            if (matches.Count <= args.Length) 
            {
                return string.Format(template, args);
            }

            // use a string builder to minimize allocations due to the
            // inability to predict the number of mutations required.
            var buffer = new StringBuilder(template);
            // used to track placeholder indexing for placeholders
            // beyond the length of the arguments collection.
            // they need to be renumbered starting at 0 for partial application to work.
            // see examples.
            var extraArgsIndex = 0;
            for (var i = 0; i < matches.Count; i++)
            {
                Match match = matches[i];

                if (i < args.Length) 
                {
                    // we have an arg for this placeholder, so apply it
                    buffer.Replace(match.Value, args[i].ToString());
                } 
                else 
                {
                    // we didn't have any arg for this index, so 
                    // re-assign the placeholder with the zero-based value                    
                    buffer.Replace(match.Value, "{" + extraArgsIndex + "}");
                    
                    // increment the overflow placeholder index.
                    extraArgsIndex++;
                }
            }
            
            // return the manually built string.
            return buffer.ToString();
        }
    }
}

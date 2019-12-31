﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Data.Parsing.Converting.Abstract;

namespace Data.Parsing.Converting
{
    public class DefaultUrlConverter : IConverter<Url>
    {
        private const string Pattern = @"^(http|https|ftp)://(\w+\.\w+)(/)?(.[^\?]+)(\?(.*))?$";

        public Url Convert(string line)
        {
            if (line == null)
            {
                throw new ArgumentNullException(nameof(line));
            }

            try
            {
                var regex = new Regex(Pattern);
                var groups = regex.Match(line).Groups;

                if (groups.Count != 7)
                {
                    throw new ArgumentException();
                }

                var scheme = groups[1].ToString();
                var host = groups[2].ToString();
                var segments = groups[4].ToString();
                var parameters = groups[6].ToString();
                return new Url
                {
                    Scheme = scheme,
                    Host = host,
                    Segments = this.ParseSegments(segments),
                    Parameters = this.ParseParameters(parameters),
                };
            }
            catch (ArgumentException)
            {
                throw new ArgumentException($"Wrong url {line}.");
            }
        }

        private List<string> ParseSegments(string segments)
        {
            if (segments.Length == 0)
            {
                return new List<string>();
            }

            var segmentsArray = segments.Trim().Split('/');
            return segmentsArray.Where(s => s.Length != 0).ToList();
        }

        private Dictionary<string, string> ParseParameters(string parameters)
        {
            if (parameters.Length == 0)
            {
                return new Dictionary<string, string>();
            }

            var result = new Dictionary<string, string>();
            var paramArray = parameters.Trim().Split(' ');
            foreach (var pair in paramArray)
            {
                var keyValue = pair.Split('=');
                if (keyValue.Length != 2)
                {
                    throw new ArgumentException();
                }

                result.Add(keyValue[0], keyValue[1]);
            }

            return result;
        }
    }
}
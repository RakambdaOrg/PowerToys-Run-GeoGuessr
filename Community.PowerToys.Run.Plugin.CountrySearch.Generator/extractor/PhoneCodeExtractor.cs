﻿using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace PowerToys_Run_CountrySearch_Generator.extractor;

public partial class PhoneCodeExtractor : IExtractor
{
    private static readonly Regex PhoneCodeRegex = PhoneCodeRegexGenerator();
    
    [GeneratedRegex(@"^\+(?<code>\d+)(\s+\d+)* - (?<country>(.+))$")]
    private static partial Regex PhoneCodeRegexGenerator();

    public string[] GetJsonPath()
    {
        return ["phone", "code"];
    }

    public Dictionary<string, string> Extract()
    {
        var values = new Dictionary<string, string>();

        var web = new HtmlWeb();
        var doc = web.Load("https://geohints.com/PhoneNumbers");

        var content = doc.DocumentNode.InnerText;
        var lines = content.Split('\n');

        var codes = lines
            .Select(line => line.Trim())
            .Select(line => PhoneCodeRegex.Match(line))
            .Where(match => match.Success)
            .SelectMany(match => match.Groups["country"].Value.Split(","), (match, country) => (country.Trim(), match.Groups["code"].Value));

        foreach (var (country, code) in codes)
        {
            values.TryAdd(country, code);
        }

        return values;
    }
}
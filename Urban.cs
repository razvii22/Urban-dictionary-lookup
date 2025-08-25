using System;
using System.Text.RegularExpressions;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;


public static class StringExt
{
    public static string Truncate(this string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return value.Length <= maxLength ? value : value.Substring(0, maxLength)+"..."; 
    }
    
}

public class List
{
    public string definition { get; set; }

    public string permalink { get; set; }

    public int thumbs_up { get; set; }

    public List<string> sound_urls { get; set; }

    public string author { get; set; }

    public string word { get; set; }

    public int defid { get; set; }

    public string current_vote { get; set; }

    public DateTime written_on { get; set; }

    public string example { get; set; }

    public int thumbs_down { get; set; }
}

public class Root
{
    public List<List> list { get; set; }
}
public class CPHInline
{
    public bool Execute()
    {
        string BadWordsPattern = "";
        if (!CPH.TryGetArg("badWordsFile", out string badWordsFile))
        {
            CPH.LogError("badWordsFile argument not found, please assign the file path.");
            CPH.ShowToastNotification("Berry error", "Urban dictionary action is missing a \"badWordsFile\" argument pointing to the dictionary. ");
            return false;
        }
        try
        {
            BadWordsPattern = File.ReadAllText(badWordsFile);
        }
        catch (Exception ex)
        {
            CPH.LogError(ex.Message);
            CPH.ShowToastNotification("Berry Error", $"Urban dictionary action error: {ex.Message}");
            return false;
        }
        Random random = new Random();
        string[] safeWords = {"meow","mrrow","mew","ono","OWO","[bad word]", "nono", "twitch","nya","beep","papaya","kiwi" };
        var word = args.ContainsKey("rawInputUrlEncoded") ? args["rawInputUrlEncoded"].ToString() : null;

        string randomUrban = null;

        if (String.IsNullOrEmpty(word) || String.IsNullOrWhiteSpace(word)) // no search word, pick random.
        {
            var urbanUrl = "https://api.urbandictionary.com/v0/random";
            using (WebClient client = new WebClient())
            {
                try
                {
                    client.Encoding = Encoding.UTF8;
                    randomUrban = client.DownloadString(urbanUrl);
                }
                catch (WebException ex) // web exception handling
                {
                    CPH.SendMessage("Something went wrong... tell razvii about it!");
                    CPH.LogError($"Webexception: {ex.Message}");
                    return false;
                }
                catch (Exception ex) // general exception
                {
                    CPH.SendMessage("Something seriously went wrong... tell razvii about it!");
                    CPH.LogError($"Unhandled exception: {ex.Message}");
                    return false;
                }
            }
        } else
        {
            var urbanUrl = "https://api.urbandictionary.com/v0/define?term=" + word;
            using (WebClient client = new WebClient())
            {
                try
                {
                    client.Encoding = Encoding.UTF8;
                    randomUrban = client.DownloadString(urbanUrl);
                }
                catch (WebException ex) // web exception handling
                {
                    CPH.SendMessage("Something went wrong... tell razvii about it!");
                    CPH.LogError($"Webexception: {ex.Message}");
                    return false;
                }
                catch (Exception ex) // general exception
                {
                    CPH.SendMessage("Something seriously went wrong... tell razvii about it!");
                    CPH.LogError($"Unhandled exception: {ex.Message}");
                    return false;
                }
            }       
        }
        Root root = JsonConvert.DeserializeObject<Root>(randomUrban);
        var randomNum = random.Next(0, root.list.Count);
        var definition = root.list[randomNum].definition;
        var newword = root.list[randomNum].word;
        var wordLink = root.list[randomNum].permalink;
        CPH.LogInfo($@"{newword} ; {definition}");
        string def = definition.Replace("\n", "").Replace("\r", "").Replace("[", "").Replace("]", "").Truncate(350);
        bool replacement = false; // initial bool
        string CleanDefinition = Regex.Replace(def, BadWordsPattern, match =>
        {
            replacement = true; // sets this to true if a replacement happens, this is to not include the url link in the message, might get twitch mad.
            return safeWords[random.Next(safeWords.Length)];
        }, RegexOptions.IgnoreCase);
        string CleanWord = Regex.Replace(newword, BadWordsPattern, match =>
        {
            replacement = true; // sets this to true if a replacement happens, this is to not include the url link in the message, might get twitch mad.
            return safeWords[random.Next(safeWords.Length)];
        }, RegexOptions.IgnoreCase);
        string message = $"";
        if (replacement)
        {
            message = $"{CleanWord} : {CleanDefinition}";
        }
        else
        {
            message = $"{CleanWord} : {CleanDefinition} ({wordLink})";
        }
        CPH.SetArgument("definition", def);
        CPH.SetArgument("word", newword);
        CPH.SetArgument("link", wordLink);
        CPH.SetArgument("cleanDefinition",CleanDefinition);
        CPH.SetArgument("cleanWord", CleanWord);
        CPH.SetArgument("responseMessage", message);
        CPH.SetArgument("hadBadWord", replacement);
        return true;
    }
}
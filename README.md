# Urban dictionary lookup
When imported into streamerbot as code, it will look up a random definition from Urban's API unless given a `rawInputUrlEncoded` argument, at which point it will try to find a definition for that.

# Usage

## Import

Head over to the [releases](https://github.com/razvii22/Urban-dictionary-lookup/releases) page for the recent import string.
Note: The code in the most recent commit CAN be used separately as seen below, but you may have to modify arguments and passed variables, or provide extra functionality in some way.
For a working build that you can just import, see aforementioned [releases](https://github.com/razvii22/Urban-dictionary-lookup/releases) page for the import string inside the provided .sb file.

## Custom usage

The script will run on its own and only really needs the path to a regex text file including bad words to censor.

Once ran, with or without a word to search, it will create the following arguments in the action argument list:

### `responseMessage` (Probably the one you want)

Which contains the formatted message in the format:
```
{word} : {definition} ({urban link})
```
OR
```
{word} : {definition} // if the definition contains a blacklisted word
```
If any word in the definition matches the [blacklist](#blacklist) and is subsequently censored out, the link will not be provided to avoid any issues with twitch TOS by linking to potentially unsafe material.

### `hadBadWord`
Returns `True` or `False` based on whether the message provided had to be censored previously, and by proxy whether the message contains any word that matched the [blacklist](#blacklist).

### `cleanDefinition`
The censored definition body by itself, for custom formatting.

### `cleanWord`
The censored word itself, for custom formatting.

### `link` POTENTIALLY UNSAFE
The link to the definition on Urban Dictionary.
The link is potentially unsafe as it contains the term itself.
Included for brevity and in case you want to find the link in the action queue history, this will appear as a variable whether or not the definition is censored.
The link cannot be obfuscated because for some reason, Urban doesn't allow you to link by `defid` alone.

### `definition` UNSAFE
The uncensored definition body, for custom formatting.
Do not use this directly as slurs and questionable material is common on Urban dictionary.

### `word` UNSAFE
The uncensored word itself, for custom formatting.
Do not use this directly as slurs and questionable material is common on Urban dictionary.

# Blacklist

The blacklist has to be given to the script in order to censor out words from Urban.  
A lot of definition contains words that might get someone banned on twitch.  
The file path has to be given in the `badWordsFile` argument as a simple string.  

## File structure

The file blacklist has to be using the following regex syntax:

```
(bad_word|bad_word|bad word)
```
Where the entire string is surrounded by parenthesis and every separate word or entry have to be separated by the vertical pipe `|` character.  
You can use spaces between the words to filter out whole phrases, or just words, any other special characters may have a specific meaning in regex and should be checked beforehand.  
The file is interpreted verbatim in the code so you can use any custom regex pattern.  
An example list is provided on the repository and is updated with every release as I find new words, but you can easily add your own and probably should as my particular use cases may differ from yours.  

## Censoring

Currently, any word that matches the blacklist will be automatically censored with a randomly picked nonsense word from a list, that list may become external eventually, but for now it is an array in the code.  
Any matched pattern from the provided file(in the case of the example, the whole word or phrase between pipe `|` characters) will be replaced with a nonsense word and raise the [HadBadWord](#HadBadWord) flag.  

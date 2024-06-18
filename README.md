![banner](https://github.com/SebastianBallesterosVargas/DiscordGenieOwl/assets/166555946/8ae6aa18-81b0-48ce-8944-1500d980fc21)

### ◯ **.NET Discord Bot Sample Integrated To SteamAPI And Perplexity GPT**

This bot is built using Discord.Net allows querying a Steam game or application and using GPT to obtain a guide for any selected achievement.
It makes use of the Perplexity API to search for guides on the web and generate a summary of the guide.

Do you need DiscordBot key, SteamAPI key and PerplexityAPI key, you can obtain them by following the official guides attached in the documentation of this post.

Create a new appsettings.json file and attach the following object, replacing the API's key with the ones you obtained from the documentation.

```JSON
{
  "Discord": {
    "Token": "[YourDiscordBotKey]",
    "CommandPrefix": "!"
  },
  "Steam": {
    "Key": "[YourSteamAPIKey]"
  },
  "Perplexity": {
    "Api": "https://api.perplexity.ai/chat/completions",
    "Key": "[YourPerplexityAPIKey]",
    "Model": {
      "Gpt3": "llama-3-sonar-small-32k-online",
      "Gpt4": "llama-3-sonar-large-32k-online"
    }
  }
}
```

To finish, you just need to add the path to appsettings.json in the Program file.

```ruby
var configuration = new ConfigurationBuilder()
    .AddUserSecrets(Assembly.GetExecutingAssembly())
    .AddJsonFile("[Your appsettings.json route]", optional: false)
    .Build();
```


### ◯ **About the official client libraries**

### **Steam Web API Documentation**
**Official Doc:** https://steamcommunity.com/dev <br />
**Services Doc:** https://steamapi.xpaw.me/#IStoreService <br />
**Valve Comunity** Doc: https://developer.valvesoftware.com/wiki/Steam_Web_API#Interfaces_and_method <br />

### **Discord .Net Library Documentation**
**GitHub Doc:** https://github.com/discord-net <br />
**Official Doc:** https://docs.discordnet.dev/guides/int_basics/message-components/intro.html <br />

### **Perplexity Labs Documentation**
**Official Perplexity Doc:** https://docs.perplexity.ai/docs/getting-started <br />


### ◯ Example Commands 

To get an app or game:
**!game [App or game name].**
**!g [App or game name].**

To get an app or game with spanish response:
**!game-es [App or game name].**
**!g-es [App or game name].**

**English response**
![image](https://github.com/SebastianBallesterosVargas/DiscordGenieOwl/assets/166555946/7d5a1537-44f2-4ec0-9911-6d384a9e9443)

**Spanish response**
![image](https://github.com/SebastianBallesterosVargas/DiscordGenieOwl/assets/166555946/3f34099f-9d0c-481d-96e6-e78294e1cebe)

# Dalton

[![.NET](https://github.com/MattTheDev/Dalton/actions/workflows/dotnet.yml/badge.svg)](https://github.com/MattTheDev/Dalton/actions/workflows/dotnet.yml)

## Setting The Bot Up

1. Clone the Source.
2. Publish it locally.
3. Create a appsettings.json in the runtime directory.
4. Insert the following, with the contents customized to your settings:

    ```json
    {
      "Settings": {
        "Prefix": "@",
        "BotToken": "DISCORD_BOT_TOKEN",
        "AllowedCharacters":  "!@#$%^&*()[]-=:;\"\\/<>,.?-_=+'"
      }
    }
    ```

      a. Prefix: The prefix for your bot commands, ie: @ping would output "Pong!"

      b. BotToken: Bot token provided by creation of your bot application at https://discord.com/developers/applications/

      c. AllowCharacters: These are additional allowed characters during the renaming process.

5. Run Dalton.Bot.exe

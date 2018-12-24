﻿using System;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Args;
using AIMLbot;
using System.Linq;
using System.Net;

namespace Awesome
{
    class Program
    {
        static ITelegramBotClient botClient;
        static readonly Bot AI = new Bot(); // This defines the object "AI" To hold the bot's infomation
        static User myUser;

        static void Main()
        {
            AI.loadSettings(); // This loads the settings from the config folder
            AI.loadAIMLFromFiles(); // This loads the AIML files from the aiml folder
            AI.isAcceptingUserInput = false; // This swithes off the bot to stop the user entering input while the bot is loading
            myUser = new User("Username", AI); // This creates a new User called "Username", using the object "AI"'s information.
            AI.isAcceptingUserInput = true; // This swithces the user input back on

            WebProxy myproxy = new WebProxy("189.90.248.75", 8080);
            botClient = new TelegramBotClient("token", myproxy);

            var me = botClient.GetMeAsync().Result;
            Console.WriteLine(
              $"Hello, World! I am user {me.Id} and my name is {me.FirstName}."
            );

            botClient.OnMessage += Bot_OnMessage;
            botClient.StartReceiving();
            Thread.Sleep(int.MaxValue);
        }

        static async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            if (e.Message.Text != null)
            {
                bool mention = e.Message.EntityValues != null && e.Message.EntityValues.Contains("@team99_bot");
                bool reply = e.Message.ReplyToMessage != null && e.Message.ReplyToMessage.From.Username == "team99_bot";
                if (!mention && !reply)
                    return;

                Console.WriteLine($"Received a text message in chat {e.Message.Chat.Id}.");

                var input = e.Message.Text;
                if (mention)
                    input = input.Remove(0, 12); // remove mention
                Request r = new Request(input, myUser, AI); // This generates a request using text from message, the user and the AI object's.
                Result res = AI.Chat(r); // This sends the request off to the object AI to get a reply back based of the AIML file's.

                await botClient.SendTextMessageAsync(
                    replyToMessageId: e.Message.MessageId,
                    chatId: e.Message.Chat,
                    text: res.Output
                    );
            }
                //Message msg1 = await botClient.SendStickerAsync(
                //    chatId: e.Message.Chat,
                //    sticker: "https://github.com/TelegramBots/book/raw/master/src/docs/sticker-fred.webp"
                //    );
                //Message msg2 = await botClient.SendStickerAsync(
                //    chatId: e.Message.Chat,
                //    sticker: msg1.Sticker.FileId
                //    );

                //Message message = await botClient.SendPhotoAsync(chatId: e.Message.Chat,
                //    photo: "https://github.com/TelegramBots/book/raw/master/src/docs/photo-ara.jpg",
                //    caption: "<b>Ara bird</b>. <i>Source</i>: <a href=\"https://pixabay.com\">Pixabay</a>",
                //    parseMode: ParseMode.Html
                //    );

                //Message message = await botClient.SendTextMessageAsync(
                //  chatId: e.Message.Chat, // or a chat id: 123456789
                //  text: "Trying *all the parameters* of `sendMessage` method",
                //  parseMode: ParseMode.Markdown,
                //  disableNotification: true,
                //  replyToMessageId: e.Message.MessageId,
                //  replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl(
                //    "Check sendMessage method",
                //    "https://core.telegram.org/bots/api#sendmessage"
                //  ))
                //);
                //Console.WriteLine(
                //    $"{message.From.FirstName} sent message {message.MessageId} " +
                //    $"to chat {message.Chat.Id} at {message.Date}. " +
                //    $"It is a reply to message {message.ReplyToMessage.MessageId} " +
                //    $"and has {message.Entities.Length} message entities."
                //    );

                //await botClient.SendStickerAsync(
                //    chatId: e.Message.Chat,
                //    sticker: "https://github.com/TelegramBots/book/raw/master/src/docs/sticker-dali.webp"
                //    );

                //await botClient.SendTextMessageAsync(
                //    chatId: e.Message.Chat,
                //    text: "Hello, World!"
                //    );

                //await botClient.SendTextMessageAsync(
                //  chatId: e.Message.Chat,
                //  text: "You said:\n" + e.Message.Text
                //);
            }
    }
}

using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

var botClient = new TelegramBotClient("6950442650:AAGxeSVlqSBSYYMEZSO0RywuYpH7R_RFPsc");

using CancellationTokenSource cts = new();

// StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
ReceiverOptions receiverOptions = new()
{
    AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
};

botClient.StartReceiving(
    updateHandler: HandleUpdateAsync,
    pollingErrorHandler: HandlePollingErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);

var me = await botClient.GetMeAsync();

Console.WriteLine($"Start listening for @{me.Username}");
Console.ReadLine();

// Send cancellation request to stop bot
cts.Cancel();

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    // Only process Message updates: https://core.telegram.org/bots/api#message
    if (update.Message is not { } message)
        return;
    // Only process text messages
    if (message.Text is not { } messageText)
        return;

    var chatId = message.Chat.Id;

    Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

    if (messageText == "Проверка")
    {
        await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: "Проверка бота: работа корректна",
            cancellationToken: cancellationToken);
    }

    if (messageText == "Привет")
    {
        await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: "Здравствуй, Олег ",
            cancellationToken: cancellationToken);
    }

    if (messageText == "Картинка")
    {
        await botClient.SendPhotoAsync(
            chatId: chatId,
            photo: InputFile.FromUri("https://yandex.ru/images/search?pos=1&from=tabbar&img_url=https%3A%2F%2Fknowcrunch.com%2Fuploads%2Fcourses%2Fknowcrunch-elearning-fundamentals-urls-utms-header-image.jpg&text=image+url&rpt=simage&lr=213"),
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken);
    }
    if (messageText == "Видео")
    { 
        await botClient.SendVideoAsync(
        chatId: chatId,
        video: InputFile.FromUri("https://raw.githubusercontent.com/TelegramBots/book/master/src/docs/video-countdown.mp4"),
        thumbnail: InputFile.FromUri("https://raw.githubusercontent.com/TelegramBots/book/master/src/2/docs/thumb-clock.jpg"),
        supportsStreaming: true,
        cancellationToken: cancellationToken);
    }

     if (messageText == "Стикер")
     {
        Message message1 = await botClient.SendStickerAsync(
    chatId: chatId,
    sticker: InputFile.FromUri("https://github.com/TelegramBots/book/raw/master/src/docs/sticker-fred.webp"),
    cancellationToken: cancellationToken);

        Message message2 = await botClient.SendStickerAsync(
            chatId: chatId,
            sticker: InputFile.FromFileId(message1.Sticker!.FileId),
            cancellationToken: cancellationToken);
    }
    if (messageText == "Опрос")
    {
       await botClient.SendPollAsync(
       chatId: chatId,
       question: "Аниме топ?",
       options: new[]
       {
        "Yes",
        "No"
       },
       cancellationToken: cancellationToken);
    }


}

Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}
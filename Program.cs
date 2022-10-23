using Telegram.Bot;
using Telegram.Bot.Types;


#nullable disable


var botClient = new TelegramBotClient("");

User bot = botClient.GetMeAsync().Result;

List<long> userIds = new List<long>();

string secretKey = "123";

long adminId = 0;


while (true)
{
    Update[] updates = await botClient.GetUpdatesAsync();
    for (int i = 0; i < updates.Length; i++)
    {
        AddUser(updates[i].Message.From.Id);

        SetAdmin(updates[i].Message.Text, updates[i].Message.From.Id);

        if (updates[i].Message.From.Id == adminId)
        {
            if (updates[i].Message.Text == "GET")
            {
                SendUserIdsToAdmin(updates[i].Message);
            }
            if (updates[i].Message.Text.Contains("everyone"))
            {
                updates[i].Message.Text = updates[i].Message.Text.Remove(0, 8);
                SendMessageToEveryone(updates[i].Message);
            }
            if (updates[i].Message.Text.Contains("personal"))
            {
               SendToUser(updates[i]);
            }
        }
        updates = await botClient.GetUpdatesAsync(updates[^1].Id + 1);
    }
}

async Task SendToUser(Update update)
{
    string[] texts = update.Message.Text.Split(" ");
    bool isParsed = long.TryParse(texts[1], out long userId);

    if (isParsed)
    {
        string message = "";
        for (int o = 2; o < texts.Length; o++)
        {
            message += $"{texts[o]} ";
        }
        await botClient.SendTextMessageAsync(new ChatId(userId), message);
    }
}

async Task SendUserIdsToAdmin(Message message)
{
    for (int i = 0; i < userIds.Count; i++)
    {
        await botClient.SendTextMessageAsync(new ChatId(message.From.Id), userIds[i].ToString());
    }
}


void SetAdmin(string message, long userId)
{
    if (adminId == 0 && message == secretKey)
    {
        adminId = userId;
    }
}

void AddUser(long userId)
{
    if (!userIds.Contains(userId))
    {
        userIds.Add(userId);
    }
}


async Task SendMessageToEveryone(Message message)
{
    for (int i = 0; i < userIds.Count; i++)
    {
        await botClient.SendTextMessageAsync(new ChatId(userIds[i]), message.Text);
    }
}
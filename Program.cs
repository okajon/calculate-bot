 

using System.Text.RegularExpressions;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Types;

public class Program
{


        
    public static async Task Main(string[] args)
    {

        var botClient = new TelegramBotClient("telegramBotToken here");


        botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            pollingErrorHandler: HandlePollingErrorAsync,
            receiverOptions: null,
            CancellationToken.None);

        var me = await botClient.GetMeAsync();

        Console.WriteLine($"Hello, World! I am user {me.Id} and my name is {me.FirstName}.");
        Console.ReadLine();
        
  
        async Task HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken token)
        {


            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                var chatId = update.Message.Chat.Id;
                var text = update.Message.Text;

                Console.WriteLine($"{update.Message.From.FirstName} said {update.Message.Text}");

                if (text == "/start" || text == "start")
                {
                    await client.SendTextMessageAsync(
                       chatId: update.Message.Chat.Id,
                        $"👋Salom `{update?.Message?.From?.FirstName}` Calculate 🤖 botga xush kelibsiz"+
                       "bu yerga matematik amallar ifodasini kiritsangiz men sizga hisoblab beraman",
                       parseMode: Telegram.Bot.Types.Enums.ParseMode.MarkdownV2);

                }
                // else if (text == "Hello" || text == "hello")
                // {
                //     await client.SendTextMessageAsync(
                //     chatId: chatId,
                //     text: $"nma gap biratiwka  iwla bo'vottimi 😎",
                //     parseMode: Telegram.Bot.Types.Enums.ParseMode.MarkdownV2);
                // }
                else if (char.IsDigit(text[0]) || text[0] == '(')
                {
                    string exp = CountExp(text);

                    await client.SendTextMessageAsync(
                    chatId: update.Message.Chat.Id,
                    text: $"javob {exp}",                   
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
                }
                // else  
                // {
                //     await client.SendTextMessageAsync(
                //     chatId: chatId,
                //     text: $"oh my god, 🤦‍♂️ what are you doing man",
                //     parseMode: Telegram.Bot.Types.Enums.ParseMode.MarkdownV2);
                // }
                
            }

        }

        Task HandlePollingErrorAsync(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
        {
            throw new NotImplementedException();
        }

    }

   

    static string CountExp(string text) 
    {
        string exp = GetPostfix(text);
        Console.WriteLine($"{exp}");
        // Console.WriteLine($"postfix evaluatega o'tti");
        
        var result = PostfixEvaluate(exp);
        //  WriteLine($"{result}"); 
        
        return result;
    }

    static string PostfixEvaluate(string input)
    {
        Stack<double> stack = new Stack<double>();

        for (int i = 0; i < input.Length; i++)
        {
            char characterOfInput = input[i];

            if (char.IsDigit(characterOfInput))
            {
                double number = 0;

                while (char.IsDigit(characterOfInput))
                {
                    number = number * 10 + (int)(characterOfInput - '0');
                    i++;
                    characterOfInput = input[i];
                }
                i--;
                stack.Push(number);
            }
            else if (characterOfInput == ' ')
            {
                continue;
            }
            else
            {
                double val1 = stack.Pop();
                double val2 = stack.Pop();

                switch (characterOfInput)
                {
                    case '+':
                        stack.Push(val2 + val1);
                        break;
                    case '-':
                        stack.Push(val2 - val1);
                        break;
                    case '*':
                        stack.Push(val2 * val1);
                        break;
                    case '/':
                        stack.Push(val2 / val1);
                        break;
                    case '^':
                        stack.Push((int)Math.Pow(val2, val1));
                        break;
                }
            }
        }
        return stack.Pop().ToString();
    }


    static string GetPostfix(string input)
    {
        string patternForOperators = @"";
        string[] operatorsArray = Regex.Split(input, patternForOperators);

        string word = "";

        for (int i = 0; i < operatorsArray.Length; i++)
        {
            word += operatorsArray[i];
        }
        word += " ";

        string infixView = "";
        for (int i = 0; i < word.Length; i++)
        {
            infixView += word[i];
            if (IsNumber(word[i]))
            {
                if (IsOperator(word[i + 1]))
                {
                    infixView += " ";
                }
            }
            else if (IsOperator(word[i]))
            {
                infixView += " ";
            }
        }

        /////until here we have the infix view of the expression

        var list = infixView.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();

        Stack<string> stack = new Stack<string>();
        stack.Push("#");
        Queue<string> queue = new Queue<string>();


        for (int i = 0; i < list.Count; i++)
        {
            if (!IsOperator(list[i]))
            {
                queue.Enqueue(list[i]);
            }
            else if (list[i] == "(")
            {
                stack.Push(list[i]);
            }
            else if (list[i] == "^")
            {
                stack.Push(list[i]);
            }
            else if (list[i] == ")")
            {
                while (stack.Peek() != "#" && stack.Peek() != "(")
                {
                    queue.Enqueue(stack.Pop());

                }
                stack.Pop();
            }
            else
            {
                if (preced(list[i]) > preced(stack.Peek()))
                {
                    stack.Push(list[i]);
                }
                else
                {
                    while (stack.Peek() != "#" && preced(list[i]) <= preced(stack.Peek()))
                    {
                        queue.Enqueue(stack.Pop());
                    }
                    stack.Push(list[i]);
                }
            }
        }


        while (stack.Peek() != "#")
        {
            queue.Enqueue(stack.Pop());
        }

        string exp = "";
        foreach (var item in queue)
        {
            exp += item + " ";
        }

        return exp;
    }


    static int preced(string ch)
    {
        if (ch == "+" || ch == "-")
        {
            return 1;              //Precedence of + or - is 1
        }
        else if (ch == "*" || ch == "/")
        {
            return 2;            //Precedence of * or / is 2
        }
        else if (ch == "^")
        {
            return 3;            //Precedence of ^ is 3
        }
        else
        {
            return 0;
        }
    }

    static bool IsNumber(char input) => input switch
    {
        '0' => true,
        '1' => true,
        '2' => true,
        '3' => true,
        '4' => true,
        '5' => true,
        '6' => true,
        '7' => true,
        '8' => true,
        '9' => true,
        _ => false
    };

    static bool IsOperator(char input) => input switch
    {
        '+' => true,
        '-' => true,
        '*' => true,
        '/' => true,
        '(' => true,
        ')' => true,
        '^' => true,
        '%' => true,
        _ => false
    };
    static bool IsOperator(string input) => input switch
    {
        "+" => true,
        "-" => true,
        "*" => true,
        "/" => true,
        "(" => true,
        ")" => true,
        "^" => true,
        "%" => true,
        _ => false
    };

}

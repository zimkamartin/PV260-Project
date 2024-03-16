using SendEmailExample;

// Run the SendEmail console app.
IConsoleApp app = new SendEmailConsoleApp();
await app.SendMail(new List<string> {"martin.zimka@gmail.com", "493023@mail.muni.cz"}, "/home/martin/PV260.txt");
Console.WriteLine("Mail should be sent!");
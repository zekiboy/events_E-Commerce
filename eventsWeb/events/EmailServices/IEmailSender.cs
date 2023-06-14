namespace events.EmailServices
{
    public interface IEmailSender
    {
        //smtp => gmail,hotmail 
        //api =>sendgrid
        //aldığımız hosting de bize bir email veriyor

        Task SendEmailAsync(string email, string subject, string htmlMessage);

        

    }
}
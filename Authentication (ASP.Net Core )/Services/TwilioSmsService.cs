using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Authentication__ASP.Net_Core__.Services
{
    public class TwilioSmsService
    {
        private readonly IConfiguration _configuration;

        public TwilioSmsService(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public async Task SendOTPAsync(string toPhoneNumber , string otp)
        {
            // Get Twilio credentials and phone number from configuration
            var accountSid = _configuration["Twilio:AccountSIDProduction"];
            var authToken = _configuration["Twilio:AuthTokenProduction"];
            var fromPhoneNumber = _configuration["Twilio:TwilioPhoneNumberUS"];

            //initialize twilio client
            TwilioClient.Init(accountSid, authToken);

            // create OTP Message
            var messageBody = $"Your verification code is {otp}";

            // send SMS
            await MessageResource.CreateAsync
                (
                to: new PhoneNumber(toPhoneNumber),
                from: new PhoneNumber(fromPhoneNumber),
                body: messageBody
                );

        }


        // a random 6-digit OTP
        public string GenerateOtp()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }
}

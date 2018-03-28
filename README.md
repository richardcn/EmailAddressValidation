# EmailAddressValidation
A simple .NET Core 2.0 Web API project using DNS MX records to validate email address. It is using Redis as cache to store the known domain names.

The project was built using Visual Studio 2017 and C#. It is using [StackExchange.Redis](https://github.com/StackExchange/StackExchange.Redis) and [jstedfast/EmailValidation](https://github.com/jstedfast/EmailValidation).
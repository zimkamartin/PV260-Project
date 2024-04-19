# StockAnalysis
StockAnalysis is an application intended to help with the tracking od different ETF funds.

## Setup

### Json Configurations

To setup what holdings are to be analyzed, you must fill out ``StockAnalysisConsole/Config/Configuration.json``. Here, you set the holding name and the url to pull data from.

To setup automatic email receivers, you must write down these addresses in ``StockAnalysisConsole/Config/Emails``.

### Environment Variables
The following environment variables must be set for the application to function properly.

- ``CLIENT_HOST`` - The host address of your email client.
- ``SMTP_PORT`` - The smtp port the application can use.
- ``SENDER_MAIL`` - The email address that the application uses to send emails.
- ``PV260_EMAIL_PASSWORD`` - The password for the email address in ``SENDER_MAIL``; 
- ``INPUT_EXTENSION`` - The extension given to downloaded etf holdings. Currently supported: ```.csv```
- ``OUTPUT_EXTENSION`` - The extension given to sent email attachments. This affects formatting. Currently supported: ```.csv```, ```.html```
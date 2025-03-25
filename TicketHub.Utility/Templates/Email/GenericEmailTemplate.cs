using TicketHub.Utility.Constants;

namespace TicketHub.Utility.Templates.Email;

/// <summary>
///     GenericEmailTemplate: A class that represents a generic email
///     This contains the default values for the fields that are required for an email
/// </summary>
public class GenericEmailTemplate
{
    protected GenericEmailTemplate()
    {
    }

    public virtual string TemplateName { get; set; } = StaticEmailTemplates.Default;
    public virtual string? SenderName { get; set; } = StaticEmailSettings.SenderName;
    public virtual string? SenderEmail { get; set; } = StaticEmailSettings.SenderEmail;
    public virtual string Category { get; set; } = "General";
    public virtual string Subject { get; set; } = "No Subject";
    public virtual string? PreHeaderText { get; set; } = "";
    public virtual string? PersonalizationTags { get; set; } = "";
    public virtual string BodyContent { get; set; } = "Default body content.";
    public virtual string? FooterContent { get; set; } = "Default footer content.";
    public virtual string? CallToAction { get; set; } = "#";
    public virtual string? CallToActionText { get; set; } = "Click here";
    public virtual string? Language { get; set; } = "en-US";
    public virtual string? RecipientType { get; set; } = "General";

    /// <summary>
    ///     Replace placeholders in the email template with the values from the placeholders dictionary.
    /// </summary>
    /// <param name="content"></param>
    /// <param name="placeholders"></param>
    /// <returns> The content with the placeholders set with actual values </returns>
    protected string ReplacePlaceholders(string content, Dictionary<string, string> placeholders)
    {
        foreach (var placeholder in placeholders)
            content = content.Replace($"{{{{{placeholder.Key}}}}}", placeholder.Value);

        return content;
    }

    /// <summary>
    ///     Render: A method that renders the email template with the placeholders replaced with the actual values
    /// </summary>
    /// <param name="placeholders"></param>
    /// <returns> An HTML-formatted string that contains the passed values </returns>
    public virtual string Render(Dictionary<string, string> placeholders)
    {
        var subject = ReplacePlaceholders(Subject, placeholders);
        var preHeaderText = ReplacePlaceholders(PreHeaderText ?? "", placeholders);
        var bodyContent = ReplacePlaceholders(BodyContent, placeholders);
        var callToActionUrl = ReplacePlaceholders(CallToAction ?? "#", placeholders);
        var callToActionText = ReplacePlaceholders(CallToActionText!, placeholders);
        var footerContent = ReplacePlaceholders(FooterContent ?? "", placeholders);

        return $@"
                <html lang='en' style='margin: 0; padding: 0;'>
                    <head>
                        <meta charset='UTF-8'>
                        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                        <title>{subject}</title>
                        <style>
                            body {{
                            margin: 0;
                            padding: 0;
                            background-color: white;
                            font-family: Arial, sans-serif;
                            line-height: 1.6;
                            color: #333;
                            }}

                            .container {{width: 100%;
                                max-width: 602px;
                                margin: auto;
                                background: white;
                                border-radius: 10px;
                                overflow: hidden;
                                box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
                            }}

                            .header {{
                                background-color: white;
                                padding: 40px 0;
                                text-align: center;
                            }}
                            
                            .header img {{
                                width: 250px;
                                display: block;
                                margin: auto;
                            }}
                            
                            .content {{
                                padding: 30px;
                            }}
                            
                            h1 {{
                                font-size: 26px;
                                color: rgb(247, 201, 0);
                                margin-bottom: 20px;
                            }}
                            
                            p {{
                                font-size: 16px;
                                color: #555555;
                                margin-bottom: 20px;
                                line-height: 1.5;
                            }}
                            
                            .call-to-action {{
                                margin-top: 20px;
                                text-align: center;
                            }}
                            
                            .button {{
                                display: inline-block;
                                padding: 12px 25px;
                                color: #ffffff;
                                background-color: rgb(247, 201, 0);
                                text-decoration: none;
                                border-radius: 5px;
                                font-weight: bold;
                                transition: background-color 0.3s ease;
                                box-shadow: 0 2px 5px rgba(0, 0, 0, 0.2);
                            }}
                            
                            .button:hover {{
                                background-color: rgb(221, 181, 0);
                            }}
                            
                            .footer {{
                                background-color: #eeeeee;
                                padding: 20px;
                                text-align: center;
                                font-size: 14px;
                                color: #666;
                                border-top: 1px solid #ddd;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <div class='header' style='background-color: white; padding: 40px 0; text-align: center;'>
                                <img src='https://i.imgur.com/vwYGyU1.jpeg' alt='Company Logo'/>
                            </div>
                             <div class='content'>
                                <h1>{subject}</h1>
                                <h2>{preHeaderText}</h2>
                                <p>{bodyContent}</p>
                                <div class='call-to-action'>
                                    <a href='{callToActionUrl}' class='button'>{callToActionText}</a>
                                </div>
                            </div>
                            <div class='footer'>
                                <p>&copy; {SenderName}, 2024</p>
                                {footerContent}
                            </div>
                        </div>
                    </body>
                </html>";
    }
}
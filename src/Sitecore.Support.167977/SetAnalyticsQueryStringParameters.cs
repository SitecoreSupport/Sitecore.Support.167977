using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Modules.EmailCampaign;
using Sitecore.Modules.EmailCampaign.Core.Pipelines.GenerateLink;
using System;

namespace Sitecore.Support.Modules.EmailCampaign.Core.Pipelines.GenerateLink.Hyperlink
{
    public class SetAnalyticsQueryStringParameters : GenerateLinkProcessor
    {
        public override void Process(GenerateLinkPipelineArgs args)
        {
            Assert.IsNotNull(args, "Arguments can't be null");
            Guid recipient = GetRecipientId(args.Url.Substring(args.Url.IndexOf(GlobalSettings.ConfirmSubscriptionQueryStringKey) + GlobalSettings.ConfirmSubscriptionQueryStringKey.Length + 1));
            if (recipient != null && recipient != Guid.Empty)
            {
                args.QueryString[GlobalSettings.AnalyticsContactIdQueryKey] = new ShortID(recipient).ToString();
            }
            else
            {
                args.QueryString[GlobalSettings.AnalyticsContactIdQueryKey] = new ShortID(args.MailMessage.RecipientId).ToString();
            }
            args.QueryString[GlobalSettings.MessageIdQueryKey] = args.MailMessage.InnerItem.ID.ToShortID().ToString();
        }

        private Guid GetRecipientId(string confirmationKey)
        {
            Database contentDb = Util.GetContentDb();
            Assert.IsNotNull(contentDb, "Sitecore Support 486611: contentDb == null");
            foreach (string current in contentDb.DataManager.GetPropertyKeys("EmailCampaign"))
            {
                if (current.IndexOf(confirmationKey, StringComparison.OrdinalIgnoreCase) > -1)
                {
                    string guid = current.Substring(current.IndexOf("EmailCampaign") + "EmailCampaign".Length + 37, 36);
                    try
                    {
                        Guid recipientId = Guid.Parse(guid);
                        return recipientId;
                    }
                    catch (Exception)
                    {
                        return Guid.Empty;
                    }

                }
            }
            return Guid.Empty;
        }
    }
}

using System.Collections.Generic;
using Fujitsu.AFC.Constants;
using Fujitsu.AFC.Model;

namespace Fujitsu.AFC.Data.Migrations.AFC
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Fujitsu.AFC.Data.AFCDataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"Migrations\AFC";
        }

        protected override void Seed(Fujitsu.AFC.Data.AFCDataContext context)
        {
            const string insertUserName = "matthew.jordan@uk.fujitsu.com";
            const string noneSpecified = "None Specified";
            var dateTime = DateTime.Now;

            #region Parameters

            var existingParameters = context.Parameters
                .AsQueryable()
                .Select(s => s.Name)
                .ToList();

            var newParameters = new List<Parameter>();


            if (!existingParameters.Contains(ParameterNames.SiteProvisionerCurrentSiteCollectionId))
            {
                newParameters.Add(
                new Parameter
                {
                    Name = ParameterNames.SiteProvisionerCurrentSiteCollectionId,
                    Value = "0",
                    Description = "Defines the current Site Collection Id.",
                    InsertedBy = insertUserName,
                    InsertedDate = dateTime,
                    UpdatedBy = insertUserName,
                    UpdatedDate = dateTime
                });
            }

            if (!existingParameters.Contains(ParameterNames.SiteProvisionerTotalUnallocatedSitesMaximum))
            {
                newParameters.Add(
                new Parameter
                {
                    Name = ParameterNames.SiteProvisionerTotalUnallocatedSitesMaximum,
                    Value = "10",
                    Description = "Maximum number of available case sites to be advanced provisioned.",
                    InsertedBy = insertUserName,
                    InsertedDate = dateTime,
                    UpdatedBy = insertUserName,
                    UpdatedDate = dateTime
                });
            }

            if (!existingParameters.Contains(ParameterNames.SiteProvisionerTotalUnallocatedSitesMinimum))
            {
                newParameters.Add(
                new Parameter
                {
                    Name = ParameterNames.SiteProvisionerTotalUnallocatedSitesMinimum,
                    Value = "2",
                    Description = "Minimum number of available case sites before an alert notification is raised i.e. we can use this to notify support of potential issues where case sites have not been advanced provisioned.",
                    InsertedBy = insertUserName,
                    InsertedDate = dateTime,
                    UpdatedBy = insertUserName,
                    UpdatedDate = dateTime
                });
            }

            if (!existingParameters.Contains(ParameterNames.SiteProvisionerSitesPerSiteCollection))
            {
                newParameters.Add(
                new Parameter
                {
                    Name = ParameterNames.SiteProvisionerSitesPerSiteCollection,
                    Value = "9",
                    Description = "Number of sites per site collection. Please note root site is NOT used therefore the SharePoint guidance of 2K sites (per collection) means we can only created 1,999 DCF Case Sites.",
                    InsertedBy = insertUserName,
                    InsertedDate = dateTime,
                    UpdatedBy = insertUserName,
                    UpdatedDate = dateTime
                });
            }

            if (!existingParameters.Contains(ParameterNames.SiteProvisionerSiteCollectionFeatures))
            {
                newParameters.Add(
                new Parameter
                {
                    Name = ParameterNames.SiteProvisionerSiteCollectionFeatures,
                    Value = noneSpecified,
                    Description = "Defines a list of features to activate upon site collection creation.",
                    InsertedBy = insertUserName,
                    InsertedDate = dateTime,
                    UpdatedBy = insertUserName,
                    UpdatedDate = dateTime
                });
            }

            if (!existingParameters.Contains(ParameterNames.SiteProvisionerSiteFeatures))
            {
                newParameters.Add(
                new Parameter
                {
                    Name = ParameterNames.SiteProvisionerSiteFeatures,
                    Value = noneSpecified,
                    Description = "Defines a list of features to activate upon site creation.",
                    InsertedBy = insertUserName,
                    InsertedDate = dateTime,
                    UpdatedBy = insertUserName,
                    UpdatedDate = dateTime
                });
            }

            if (!existingParameters.Contains(ParameterNames.SiteCollectionGroupOwners))
            {
                newParameters.Add(
                new Parameter
                {
                    Name = ParameterNames.SiteCollectionGroupOwners,
                    Value = noneSpecified,
                    Description = "Delimited list of Users of AD Group Names assigned as Site Collection Owner.",
                    InsertedBy = insertUserName,
                    InsertedDate = dateTime,
                    UpdatedBy = insertUserName,
                    UpdatedDate = dateTime
                });
            }

            if (!existingParameters.Contains(ParameterNames.SiteCollectionStorageMaximumLevel))
            {
                newParameters.Add(
                new Parameter
                {
                    Name = ParameterNames.SiteCollectionStorageMaximumLevel,
                    Value = "300",
                    Description = "Site collection storage maximum level.",
                    InsertedBy = insertUserName,
                    InsertedDate = dateTime,
                    UpdatedBy = insertUserName,
                    UpdatedDate = dateTime
                });
            }

            if (!existingParameters.Contains(ParameterNames.SiteCollectionUserCodeMaximumLevel))
            {
                newParameters.Add(
                new Parameter
                {
                    Name = ParameterNames.SiteCollectionUserCodeMaximumLevel,
                    Value = "200",
                    Description = "Site collection user code maximum level.",
                    InsertedBy = insertUserName,
                    InsertedDate = dateTime,
                    UpdatedBy = insertUserName,
                    UpdatedDate = dateTime
                });
            }

            if (!existingParameters.Contains(ParameterNames.SiteCollectionStorageWarningLevel))
            {
                newParameters.Add(
                new Parameter
                {
                    Name = ParameterNames.SiteCollectionStorageWarningLevel,
                    //Value = "879609302220",
                    Value = "100",
                    Description = "Site collection storage warning level.",
                    InsertedBy = insertUserName,
                    InsertedDate = dateTime,
                    UpdatedBy = insertUserName,
                    UpdatedDate = dateTime
                });
            }

            if (!existingParameters.Contains(ParameterNames.SiteCollectionGroupMembers))
            {
                newParameters.Add(
                new Parameter
                {
                    Name = ParameterNames.SiteCollectionGroupMembers,
                    Value = noneSpecified,
                    Description = "Delimited list of Users of AD Group Names assigned as Site Collection Members.",
                    InsertedBy = insertUserName,
                    InsertedDate = dateTime,
                    UpdatedBy = insertUserName,
                    UpdatedDate = dateTime
                });
            }

            if (!existingParameters.Contains(ParameterNames.SiteTemplate))
            {
                newParameters.Add(
                new Parameter
                {
                    Name = ParameterNames.SiteTemplate,
                    Value = "STS#0",
                    Description = "Name of Site Template to use for site creation.",
                    InsertedBy = insertUserName,
                    InsertedDate = dateTime,
                    UpdatedBy = insertUserName,
                    UpdatedDate = dateTime
                });
            }

            if (!existingParameters.Contains(ParameterNames.OperationsTimerLockExpiryIntervalInDays))
            {
                newParameters.Add(
                new Parameter
                {
                    Name = ParameterNames.OperationsTimerLockExpiryIntervalInDays,
                    Value = "5",
                    Description = "Period (minutes) after which if the current locked time is not updated a competing service can assume the lock.",
                    InsertedBy = insertUserName,
                    InsertedDate = dateTime,
                    UpdatedBy = insertUserName,
                    UpdatedDate = dateTime
                });
            }

            if (!existingParameters.Contains(ParameterNames.OperationsJournalRetentionPeriodInDays))
            {
                newParameters.Add(
                new Parameter
                {
                    Name = ParameterNames.OperationsJournalRetentionPeriodInDays,
                    Value = "30",
                    Description = "Defines the number of days to retain the operations journal records.",
                    InsertedBy = insertUserName,
                    InsertedDate = dateTime,
                    UpdatedBy = insertUserName,
                    UpdatedDate = dateTime
                });
            }

            if (!existingParameters.Contains(ParameterNames.TenantAdminWebSiteDomainPath))
            {
                newParameters.Add(
                new Parameter
                {
                    Name = ParameterNames.TenantAdminWebSiteDomainPath,
                    // Value = @"https://mattjordan-admin.sharepoint.com/",
                    Value = @"https://fujitsuuki-admin.sharepoint.com/",
                    Description = @"Domain name for the tenant admin web site (e.g. https://domainname-admin.onsharepoint.com).",
                    InsertedBy = insertUserName,
                    InsertedDate = dateTime,
                    UpdatedBy = insertUserName,
                    UpdatedDate = dateTime
                });
            }

            if (!existingParameters.Contains(ParameterNames.TenantWebSiteDomainPath))
            {
                newParameters.Add(
                new Parameter
                {
                    Name = ParameterNames.TenantWebSiteDomainPath,
                    //Value = @"https://mattjordan.sharepoint.com",
                    Value = @"https://fujitsuuki.sharepoint.com",
                    Description = @"Domain name for the tenant web site (e.g. https://domainname.onsharepoint.com).",
                    InsertedBy = insertUserName,
                    InsertedDate = dateTime,
                    UpdatedBy = insertUserName,
                    UpdatedDate = dateTime
                });
            }

            if (!existingParameters.Contains(ParameterNames.TenantWebSiteUrlPath))
            {
                newParameters.Add(
                new Parameter
                {
                    Name = ParameterNames.TenantWebSiteUrlPath,
                    Value = "sites",
                    Description = @"URL to the tenant web site (e.g. https://domainname.onsharepoint.com)",
                    InsertedBy = insertUserName,
                    InsertedDate = dateTime,
                    UpdatedBy = insertUserName,
                    UpdatedDate = dateTime
                });
            }

            if (!existingParameters.Contains(ParameterNames.EnvironmentSiteCollectionPrefix))
            {
                newParameters.Add(
                new Parameter
                {
                    Name = ParameterNames.EnvironmentSiteCollectionPrefix,
                    Value = "DCF-D",
                    Description = "Character string used to prefix the site collection identifier allowing the site collection naming convention to create collections in the same tenant and simultaneously support different environments (e.g. Live(L), UAT(U), Training(T), Other(O)).",
                    InsertedBy = insertUserName,
                    InsertedDate = dateTime,
                    UpdatedBy = insertUserName,
                    UpdatedDate = dateTime
                });
            }

            if (!existingParameters.Contains(ParameterNames.CaseSiteOperationsRecordDeletionIntervalInDays))
            {
                newParameters.Add(
                new Parameter
                {
                    Name = ParameterNames.CaseSiteOperationsRecordDeletionIntervalInDays,
                    Value = "1",
                    Description = "Defines how old records must be (in days) before they are deleted). This is used as input to a DateDiff function in a housekeeping procedure to allow record deletion, preventing excessive table growth.",
                    InsertedBy = insertUserName,
                    InsertedDate = dateTime,
                    UpdatedBy = insertUserName,
                    UpdatedDate = dateTime
                });
            }

            if (!existingParameters.Contains(ParameterNames.DigitialCaseFileRetentionPeriodInWeeks))
            {
                newParameters.Add(
                new Parameter
                {
                    Name = ParameterNames.DigitialCaseFileRetentionPeriodInWeeks,
                    Value = "1300",
                    Description = "Defines the period to retain a DCF library current setting is 25 years equivalent.",
                    InsertedBy = insertUserName,
                    InsertedDate = dateTime,
                    UpdatedBy = insertUserName,
                    UpdatedDate = dateTime
                });
            }


            if (!existingParameters.Contains(ParameterNames.TenantAccountUserName))
            {
                newParameters.Add(
                new Parameter
                {
                    Name = ParameterNames.TenantAccountUserName,
                    Value = "Matthew.Jordan@FujitsuUKI.onmicrosoft.com", // "mattjordan@mattjordan.onmicrosoft.com"
                    Description = "Defines the SharePoint online tenant account user account name.",
                    InsertedBy = insertUserName,
                    InsertedDate = dateTime,
                    UpdatedBy = insertUserName,
                    UpdatedDate = dateTime
                });
            }


            if (!existingParameters.Contains(ParameterNames.TenantAccountPassword))
            {
                newParameters.Add(
                new Parameter
                {
                    Name = ParameterNames.TenantAccountPassword,
                    Value = "##PASSWORD##!",
                    Description = "Defines the password of the SharePoint online tenant user account name.",
                    InsertedBy = insertUserName,
                    InsertedDate = dateTime,
                    UpdatedBy = insertUserName,
                    UpdatedDate = dateTime
                });
            }

            if (!existingParameters.Contains(ParameterNames.SupportSiteCollectionSuffix))
            {
                newParameters.Add(
                new Parameter
                {
                    Name = ParameterNames.SupportSiteCollectionSuffix,
                    Value = @"Support",
                    Description = @"Character string used to suffix the support site collection identifier.",
                    InsertedBy = insertUserName,
                    InsertedDate = dateTime,
                    UpdatedBy = insertUserName,
                    UpdatedDate = dateTime
                });
            }

            if (!existingParameters.Contains(ParameterNames.SupportEscalationListName))
            {
                newParameters.Add(
                new Parameter
                {
                    Name = ParameterNames.SupportEscalationListName,
                    Value = "Application Issues",
                    Description = "The name of the SharePoint list item used for escalation of issues.",
                    InsertedBy = insertUserName,
                    InsertedDate = dateTime,
                    UpdatedBy = insertUserName,
                    UpdatedDate = dateTime
                });
            }

            if (!existingParameters.Contains(ParameterNames.RecoverableExceptionRetryDelayIntervalInMinutes))
            {
                newParameters.Add(
                new Parameter
                {
                    Name = ParameterNames.RecoverableExceptionRetryDelayIntervalInMinutes,
                    Value = "30",
                    Description = "Defines the delay interval in minutes before a task will be rescheduled once initial retries have been exhausted.",
                    InsertedBy = insertUserName,
                    InsertedDate = dateTime,
                    UpdatedBy = insertUserName,
                    UpdatedDate = dateTime
                });
            }

            if (!existingParameters.Contains(ParameterNames.ActiveDirectoryGlobalSystemsAdministratorGroup))
            {
                newParameters.Add(
                new Parameter
                {
                    Name = ParameterNames.ActiveDirectoryGlobalSystemsAdministratorGroup,
                    Value = "EASPIRE_GSA",
                    Description = "Defines Active Directory Global Systems Administrator name.",
                    InsertedBy = insertUserName,
                    InsertedDate = dateTime,
                    UpdatedBy = insertUserName,
                    UpdatedDate = dateTime
                });
            }

            if (!existingParameters.Contains(ParameterNames.ActiveDirectoryContributeClosedGroup))
            {
                newParameters.Add(
                new Parameter
                {
                    Name = ParameterNames.ActiveDirectoryContributeClosedGroup,
                    Value = "EASPIRE_{0}_CONTRIBUTE_CLOSED",
                    Description = "Defines Active Directory Contribute Closed Group name, which will control access to DCF''s that have been closed.",
                    InsertedBy = insertUserName,
                    InsertedDate = dateTime,
                    UpdatedBy = insertUserName,
                    UpdatedDate = dateTime
                });
            }

            if (!existingParameters.Contains(ParameterNames.ActiveDirectoryContributeGroup))
            {
                newParameters.Add(
                new Parameter
                {
                    Name = ParameterNames.ActiveDirectoryContributeGroup,
                    Value = "EASPIRE_{0}_CONTRIBUTE",
                    Description = "Defines Active Directory Contribute Group name, which will control contribute access to DCF''s.",
                    InsertedBy = insertUserName,
                    InsertedDate = dateTime,
                    UpdatedBy = insertUserName,
                    UpdatedDate = dateTime
                });
            }

            if (!existingParameters.Contains(ParameterNames.ActiveDirectoryContributeGroup))
            {
                newParameters.Add(
                new Parameter
                {
                    Name = ParameterNames.ActiveDirectoryReadGroup,
                    Value = "EASPIRE_{0}_READ",
                    Description = "Defines Active Directory Read Group name, which will control read access to DCF''s.",
                    InsertedBy = insertUserName,
                    InsertedDate = dateTime,
                    UpdatedBy = insertUserName,
                    UpdatedDate = dateTime
                });
            }

            if (!existingParameters.Contains(ParameterNames.SearchCentreUrl))
            {
                newParameters.Add(
                new Parameter
                {
                    Name = ParameterNames.SearchCentreUrl,
                    Value = "https://fujitsuuki.sharepoint.com/sites/DCF-Search",
                    Description = "Defines the Search Centre URL.",
                    InsertedBy = insertUserName,
                    InsertedDate = dateTime,
                    UpdatedBy = insertUserName,
                    UpdatedDate = dateTime
                });
            }

            if (!existingParameters.Contains(ParameterNames.SearchCentreResultsPageUrl))
            {
                newParameters.Add(
                new Parameter
                {
                    Name = ParameterNames.SearchCentreResultsPageUrl,
                    Value = "https://fujitsuuki.sharepoint.com/sites/DCF-Search/Pages/Results.aspx",
                    Description = "Defines the Search Centre Results Page Url where search results page queries will be sent to.",
                    InsertedBy = insertUserName,
                    InsertedDate = dateTime,
                    UpdatedBy = insertUserName,
                    UpdatedDate = dateTime
                });
            }

            if (!existingParameters.Contains(ParameterNames.CaseLibraryFolderNames))
            {
                newParameters.Add(
                    new Parameter
                    {
                        Name = ParameterNames.CaseLibraryFolderNames,
                        Value = "Referral Information;Chronology;Assessments;Service Plans and Reviews;Running Record;Safeguarding and Child Protection;Correspondence;Education and Training;Health;Third Party and Confidential Information;Scrutiny Record;LAC and ICS Documentation;Complaints;Finance;Project and Service Specific;Closure",
                        Description = "Defines the semi-colon (;) separated Case Library Folder Names",
                        InsertedBy = insertUserName,
                        InsertedDate = dateTime,
                        UpdatedBy = insertUserName,
                        UpdatedDate = dateTime
                    });
            }

            newParameters.ForEach(p => context.Parameters.AddOrUpdate(p));
            context.SaveChanges();

            #endregion

            #region Tasks


            var existingTasks = context.Tasks
                .AsQueryable()
                .Select(s => s.Name)
                .ToList();


            var newTasks = new List<Task>();

            if (!existingTasks.Contains(TaskNames.CaseSiteProvisioning))
            {
                newTasks.Add(
                    new Task
                    {
                        Frequency = TaskFrequencyNames.Daily,
                        Handler = TaskHandlerNames.ProvisioningHandler,
                        Name = TaskNames.CaseSiteProvisioning,
                        NextScheduledDate = new DateTime(dateTime.Year, dateTime.Month, dateTime.AddDays(-1).Day, 21, 0, 0),
                        InsertedBy = insertUserName,
                        InsertedDate = dateTime,
                        UpdatedBy = insertUserName,
                        UpdatedDate = dateTime,
                    });
            }

            if (!existingTasks.Contains(TaskNames.HistoryErrorLogMonitoring))
            {
                newTasks.Add(
                    new Task
                    {
                        Frequency = "M5",
                        Handler = TaskHandlerNames.SupportHandler,
                        Name = TaskNames.HistoryErrorLogMonitoring,
                        NextScheduledDate = new DateTime(dateTime.Year, dateTime.Month, dateTime.AddDays(-1).Day, 9, 0, 0),
                        InsertedBy = insertUserName,
                        InsertedDate = dateTime,
                        UpdatedBy = insertUserName,
                        UpdatedDate = dateTime,
                    });
            }

            if (!existingTasks.Contains(TaskNames.AllocatePin))
            {
                newTasks.Add(
                    new Task
                    {
                        Frequency = TaskFrequencyNames.OneTime,
                        Handler = TaskHandlerNames.OperationsHandler,
                        Name = TaskNames.AllocatePin,
                        Pin = 12345,
                        SiteTitle = "An Example PIN site",
                        InsertedBy = insertUserName,
                        InsertedDate = dateTime,
                        UpdatedBy = insertUserName,
                        UpdatedDate = dateTime,
                    });
            }

            if (!existingTasks.Contains(TaskNames.UpdateCaseTitle))
            {
                newTasks.Add(
                    new Task
                    {
                        Frequency = TaskFrequencyNames.OneTime,
                        Handler = TaskHandlerNames.OperationsHandler,
                        Name = TaskNames.HistoryErrorLogMonitoring,  // Invalid operations task added to test Dead Task processing 
                        Pin = 12345,
                        InsertedBy = insertUserName,
                        InsertedDate = dateTime,
                        UpdatedBy = insertUserName,
                        UpdatedDate = dateTime,
                    });
            }

            if (!existingTasks.Contains(TaskNames.AllocateCase))
            {
                newTasks.Add(
                    new Task
                    {
                        Frequency = TaskFrequencyNames.OneTime,
                        Handler = TaskHandlerNames.OperationsHandler,
                        Name = TaskNames.UpdateServiceUserTitle,
                        Pin = 12345,
                        SiteTitle = "Mr Matt Jordan",
                        InsertedBy = insertUserName,
                        InsertedDate = dateTime,
                        UpdatedBy = insertUserName,
                        UpdatedDate = dateTime,
                    });
            }

            if (!existingTasks.Contains(TaskNames.UpdateCaseTitle))
            {
                newTasks.Add(
                    new Task
                    {
                        Frequency = TaskFrequencyNames.OneTime,
                        Handler = TaskHandlerNames.OperationsHandler,
                        Name = TaskNames.UpdateCaseTitle,
                        // Pin omitted to test Dead Task processing
                        InsertedBy = insertUserName,
                        InsertedDate = dateTime,
                        UpdatedBy = insertUserName,
                        UpdatedDate = dateTime,
                    });
            }

            if (!existingTasks.Contains(TaskNames.UpdateCaseTitleByProject))
            {
                newTasks.Add(
                    new Task
                    {
                        Frequency = TaskFrequencyNames.OneTime,
                        Handler = TaskHandlerNames.OperationsHandler,
                        Name = TaskNames.UpdateCaseTitleByProject,
                        Pin = 12345,
                        InsertedBy = insertUserName,
                        InsertedDate = dateTime,
                        UpdatedBy = insertUserName,
                        UpdatedDate = dateTime,
                    });
            }

            newTasks.ForEach(t => context.Tasks.AddOrUpdate(t));
            context.SaveChanges();


            #endregion
        }
    }
}

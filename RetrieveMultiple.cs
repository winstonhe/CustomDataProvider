using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Extensions;
using System.Net.Http;


namespace CustomDataProvider_Demo
{
    public class RetrieveMultiple : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            //Get the trace provider
            ITracingService tracer = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            //get the execution context;
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));


            //Get the service agent
            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = factory.CreateOrganizationService(context.UserId);

            try
            {
                tracer.Trace("RetrieveMultiple is called");
                QueryExpression query = context.InputParameterOrDefault<QueryExpression>("Query");
                SearchVisitor visitor = new SearchVisitor();
                query.Accept(visitor);


                IEntityDataSourceRetrieverService retrieveService = (IEntityDataSourceRetrieverService)serviceProvider.Get<IEntityDataSourceRetrieverService>();

                Entity sourceEntity = retrieveService.RetrieveEntityDataSource();

                string externalwebapiurl;

                if (string.IsNullOrEmpty(visitor.SearchKeyWords))
                    //Get the settings of Data source               
                    externalwebapiurl = sourceEntity.GetAttributeValue<string>("new_externalwebapiurl") + "/Students";
                else
                {
                    tracer.Trace("Search keywords is " + visitor.SearchKeyWords);
                    //The search URL wil be like this: http://winstonodata4.azurewebsites.net/Students?$filter=contains(name,'D')
                    externalwebapiurl = sourceEntity.GetAttributeValue<string>("new_externalwebapiurl") + "/Students?$filter=contains(name,'" + visitor.SearchKeyWords + "')";
                }

                tracer.Trace("external web api url: " + externalwebapiurl);

                EntityCollection results = new EntityCollection();
                results.EntityName = "new_student";

                //Getting Data from ODATA
                tracer.Trace("Begin to get data");
                var getAPIDataTask = Task.Run(async () => await HttpHelper.GetAPIdata(externalwebapiurl));
                Task.WaitAll(getAPIDataTask);

                OData odata = getAPIDataTask.Result;


                foreach (QueryResult row in odata.Value)
                {
                    Entity newrecord = new Entity("new_student");

                    newrecord.Attributes.Add("new_studentid", row.StudentId);
                    newrecord.Attributes.Add("new_name", row.name);
                    newrecord.Attributes.Add("new_familyaddress", row.familyaddress);
                    //newrecord.Attributes.Add("new_age", row.age);
                    //newrecord.Attributes.Add("new_grade", row.grade);
                    newrecord.Attributes.Add("new_phoneno", row.phoneno);

                    results.Entities.Add(newrecord);
                }

                context.OutputParameters["BusinessEntityCollection"] = results;

            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }

        }
    }
}

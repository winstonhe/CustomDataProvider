using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Extensions;

namespace CustomDataProvider_Demo
{
    public class Retrieve : IPlugin
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
                tracer.Trace("Retrieve is called");
                tracer.Trace("Get retrieve Service");
                IEntityDataSourceRetrieverService retrieveService = (IEntityDataSourceRetrieverService)serviceProvider.Get<IEntityDataSourceRetrieverService>();
                Entity sourceEntity = retrieveService.RetrieveEntityDataSource();

                EntityReference target = (EntityReference)context.InputParameters["Target"];
                tracer.Trace("Target ID is : " + target.Id);

                string externalwebapiurl = sourceEntity.GetAttributeValue<string>("new_externalwebapiurl") + "/Students(" + target.Id + ")";
                tracer.Trace("Query URL: " + externalwebapiurl);

                //Getting Data from ODATA
                tracer.Trace("Begin to get data");
                var getAPIDataTask = Task.Run(async () => await HttpHelper.GetAPIdatabyID(externalwebapiurl));
                Task.WaitAll(getAPIDataTask);


                QueryResult row = getAPIDataTask.Result;

                Entity newrecord = new Entity("new_student");
                newrecord.Attributes.Add("new_studentid", row.StudentId);
                newrecord.Attributes.Add("new_name", row.name);
                newrecord.Attributes.Add("new_familyaddress", row.familyaddress);
                //newrecord.Attributes.Add("new_age", row.age);
                //newrecord.Attributes.Add("new_grade", row.grade);
                newrecord.Attributes.Add("new_phoneno", row.phoneno);
                context.OutputParameters["BusinessEntity"] = newrecord;

            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }

        }
    }
}

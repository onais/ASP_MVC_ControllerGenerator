using YounesController.UI;
using Microsoft.AspNet.Scaffolding;
using System.Collections.Generic;
using System;
using Microsoft.AspNet.Scaffolding.Core.Metadata;
using Microsoft.AspNet.Scaffolding.EntityFramework;
using EnvDTE;
using YounesController.Utils;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNet.Scaffolding.NuGet;
using System.Reflection;
using Microsoft.CSharp;
using System.CodeDom;

namespace YounesController
{
    public class CustomCodeGenerator : CodeGenerator
    {
        public static int i1 = 0;
        public static int i2 = 0;
     
        CustomViewModel _viewModel;
        public delegate void TestDelegate(string message);
        /// <summary>
        /// Constructor for the custom code generator
        /// </summary>
        /// <param name="context">Context of the current code generation operation based on how scaffolder was invoked(such as selected project/folder) </param>
        /// <param name="information">Code generation information that is defined in the factory class.</param>
        public CustomCodeGenerator(
            CodeGenerationContext context,
            CodeGeneratorInformation information)
            : base(context, information)
        {
            _viewModel = new CustomViewModel(Context);
        }



        /// <summary>
        /// Any UI to be displayed after the scaffolder has been selected from the Add Scaffold dialog.
        /// Any validation on the input for values in the UI should be completed before returning from this method.
        /// </summary>
        /// <returns></returns>
        public override bool ShowUIAndValidate()
        {
            // Bring up the selection dialog and allow user to select a model type
            SelectModelWindow window = new SelectModelWindow(_viewModel);
            bool? showDialog = window.ShowDialog();
            return showDialog ?? false;
        }

        /// <summary>
        /// This method is executed after the ShowUIAndValidate method, and this is where the actual code generation should occur.
        /// In this example, we are generating a new file from t4 template based on the ModelType selected in our UI.
        /// </summary>
        public override void GenerateCode()
        {
            // Get the selected code type
            var codeType = _viewModel.SelectedModelType.CodeType;
            var codeType2 = _viewModel.SelectedModelType2.CodeType;
            var filterClass = _viewModel.SelectedModelType3.CodeType;

            var isCreate = _viewModel.isCreate;
            var isUpdate = _viewModel.isUpdate;
            var isDelete = _viewModel.isDelete;
            var isActivateDisactivate = _viewModel.isActivateDisactivate;
            var isOrderNum = _viewModel.isOrderNum;

            var isFemale = _viewModel.isFemale;

            var plural = _viewModel.plural;
            var singular = _viewModel.singular;

            // Setup the scaffolding item creation parameters to be passed into the T4 template.
            var arabicPrefix=((bool)isFemale)?"هذه":"هذا";  
   

            string modelTypeName = codeType.FullName;
            string dbContextTypeName = codeType2.FullName;

            string storedProcedureName = filterClass.Name;
            storedProcedureName = storedProcedureName.Substring(0, storedProcedureName.Length - 7);
            // First Scaffold the DB Context
            IEntityFrameworkService efService = (IEntityFrameworkService)Context.ServiceProvider.GetService(typeof(IEntityFrameworkService));

            ModelMetadata modelMetadata = efService.AddRequiredEntity(Context, dbContextTypeName, modelTypeName);

            Type storedClass = Type.GetType(filterClass.FullName);

            string[,] p = getParamsMatrix(dbContextTypeName, storedProcedureName);



            PropertyInfo[] mb = storedClass.GetProperties(BindingFlags.DeclaredOnly |
                        BindingFlags.Public |
                        BindingFlags.Instance);
            IList<PropertyInfo> props = new List<PropertyInfo>(mb);

            string str = "";

            foreach (var prop in props)
            {
                if (!prop.Name.Equals("TotalRows"))
                {
                    if (prop.PropertyType.ToString().Contains("tring"))
                    {
                        str += "\t\t\td." + prop.Name + ",\n";
                    }
                    else
                    {
                        str += "\t\t\td." + prop.Name + ".ToString(),\n";
                    }
                }
            }

            str = str.Substring(0, str.Length - 2);

           
            string[,] filterProps=getPropsMatrix(filterClass.FullName,codeType.Name);
            string[,] filterParams=getParamsMatrix(dbContextTypeName,storedProcedureName);
         
          

            int diff =(i2 - i1)>=0?i2-i1:0;


            HashSet<string> requiredNamespaces = new HashSet<string>();
            requiredNamespaces.Add(Context.ActiveProject.Name + ".Models");
            requiredNamespaces.Add(Context.ActiveProject.Name + ".Helpers");
            var parameters = new Dictionary<string, object>()
            {
                {"ControllerName",codeType.Name+"Controller"},
                {"ControllerRootName",""},
                 {"Namespace",Context.ActiveProject.Name+".Controllers"},
                  {"AreaName",""},
                   {"ContextTypeName",codeType2.Name},
                   {"ModelTypeName",codeType.Name},
                   {"ModelTypeFullName",codeType.FullName},

                   {"StoredProcedureTypeName",storedProcedureName},
                     {"StoredProcedureParams",getParamsString(dbContextTypeName, storedProcedureName)},
                     {"filterParams",filterParams},
                     {"filterProps",filterProps},
                     {"diff",diff},

                       {"createParams",getParamsStringForCreateUpdate(codeType.Name,dbContextTypeName,"sp"+codeType.Name+"Add")},
                       {"updateParams",getParamsStringForCreateUpdate(codeType.Name,dbContextTypeName,"sp"+codeType.Name+"Update")},

                       {"ParamsMatrixForCreate",getParamsMatrixForCreateUpdate(dbContextTypeName,"sp"+codeType.Name+"Add")},
                       {"ParamsMatrixForUpdate",getParamsMatrixForCreateUpdate(dbContextTypeName,"sp"+codeType.Name+"Update")},

                        {"isCreate",isCreate},
                         {"isUpdate",isUpdate},
                          {"isDelete",isDelete},
                          {"isActivateDisactivate",isActivateDisactivate},
                          {"isOrderNum",isOrderNum},

                            {"pluralName",plural},
                              {"singularName",singular},
                                {"arabicPrefix",arabicPrefix},

                   {"ModelVariable",codeType.Name.ToLower()},

                      {"ModelMetadata",modelMetadata},

                      {"ModelMetadataStoredProcedure",str},

                         {"EntitySetVariable",modelMetadata.EntitySetName.ToLowerInvariant()},
                            {"UseAsync",false},
                               {"IsOverpostingProtectionRequired",false},
                                  {"BindAttributeIncludeText",""},
                                     {"OverpostingWarningMessage",""},
                                        {"RequiredNamespaces",requiredNamespaces}
           //    { 
                    /* This value should match the parameter in T4 */
                 //  "ModelType", 
                    
                    /* This is the value passed */ 
               //     codeType
           //     }
             //   ,
              //   { 
                    /* This value should match the parameter in T4 */
                //    "ModelType2", 
                    
                    /* This is the value passed */ 
               //     contextClass
                //}

                //You can pass more parameters after they are defined in the template
            };





            // Add the custom scaffolding item from T4 template.
            //AddFileFromTemplate:current project-project path-template path-parameters

            /* this.AddFileFromTemplate(Context.ActiveProject,
                 "CustomCode",
                 "CustomTextTemplate",
                 parameters,
                 skipIfExists: false);*/
            //Adding the controller
            this.AddFileFromTemplate(Context.ActiveProject,
                string.Format("Controllers\\{0}Controller", codeType.Name),
               "Controller0",
               parameters,
               skipIfExists: false);

            this.AddFolder(Context.ActiveProject, @"Views\" + codeType.Name);
            this.AddFileFromTemplate(Context.ActiveProject,
             "Views\\" + codeType.Name+"\\Index",
             "Index0",
             parameters,
             skipIfExists: false);

            if (isCreate)
            {
                this.AddFileFromTemplate(Context.ActiveProject,
                 "Views\\" + codeType.Name + "\\Create",
                 "Create0",
                 parameters,
                 skipIfExists: false);
            }

            if (isUpdate)
            {
                this.AddFileFromTemplate(Context.ActiveProject,
                 "Views\\" + codeType.Name + "\\Edit",
                 "Edit0",
                 parameters,
                 skipIfExists: false);
            }

        }


        public static string getParamsString(string context, string methodName)
        {
            Type t = Type.GetType(context);
            if (t == null) return "";
            MethodInfo methodInfo = t.GetMethod(methodName);
            if (methodInfo == null) return "";
            string typeName;
            string result = "";
            foreach (ParameterInfo pParameter in methodInfo.GetParameters())
            {
                using (var provider = new CSharpCodeProvider())
                {
                    var typeRef = new CodeTypeReference(pParameter.ParameterType);
                    typeName = provider.GetTypeOutput(typeRef);
                }
                if (!pParameter.Name.Equals("iSortCol") && !pParameter.Name.Equals("sSortDir") && !pParameter.Name.Equals("pageNumber") && !pParameter.Name.Equals("pageSize"))
                    result += pParameter.Name + " ,";
            }
            return result;
        }


        public static string getParamsStringForCreateUpdate(string model, string context, string methodName)
        {
            Type t=Type.GetType(context);
            if (t == null) return "";
            MethodInfo methodInfo = t.GetMethod(methodName);
            if (methodInfo == null) return "";
            string typeName;
            string result = "";
            foreach (ParameterInfo pParameter in methodInfo.GetParameters())
            {
                using (var provider = new CSharpCodeProvider())
                {
                    var typeRef = new CodeTypeReference(pParameter.ParameterType);
                    typeName = provider.GetTypeOutput(typeRef);
                }
                result += Char.ToLowerInvariant(model[0]) + model.Substring(1) + "." + Char.ToUpperInvariant(pParameter.Name[0]) + pParameter.Name.Substring(1) + " ,";
            }
            return result.Substring(0, result.Length - 1);
        }




        public static string[,] getParamsMatrixForCreateUpdate(string context, string methodName)
        {
            string[,] result = new string[100, 2];
            Type t = Type.GetType(context);
            if (t == null) return result;
            MethodInfo methodInfo = t.GetMethod(methodName);
            if (methodInfo == null) return result;
            string typeName;

            int i = 0;
            foreach (ParameterInfo pParameter in methodInfo.GetParameters())
            {
                using (var provider = new CSharpCodeProvider())
                {
                    var typeRef = new CodeTypeReference(pParameter.ParameterType);
                    typeName = provider.GetTypeOutput(typeRef);
                }

                        result[i, 0] = typeName;
                        result[i, 1] = pParameter.Name;
                    i++;
                
            }

            return result;
        }


        public static string[,] getParamsMatrix(string context, string methodName)
        {
            string[,] result = new string[100, 2];
            Type t = Type.GetType(context);
            if (t == null) return result;
            MethodInfo methodInfo = t.GetMethod(methodName);
            if (methodInfo == null) return result;
            string typeName;
           
            int i = 0;
            foreach (ParameterInfo pParameter in methodInfo.GetParameters())
            {
                using (var provider = new CSharpCodeProvider())
                {
                    var typeRef = new CodeTypeReference(pParameter.ParameterType);
                    typeName = provider.GetTypeOutput(typeRef);
                }

                if (!pParameter.Name.Equals("iSortCol") && !pParameter.Name.Equals("sSortDir") && !pParameter.Name.Equals("pageNumber") && !pParameter.Name.Equals("pageSize"))
                {

                    if (!typeName.Equals("string") && typeName.Contains("Nullable") && typeName.Length > 9)
                    {
                        result[i, 0] = typeName.Substring(16, typeName.Length - 17) + "?";
                        result[i, 1] = pParameter.Name;
                    }
                    else
                    {
                        result[i, 0] = typeName;
                        result[i, 1] = pParameter.Name;
                    }
                    i++;
                }
                i2 = i;
            }

            return result;
        }

        public static string[,] getPropsMatrix(string fullname, string model)
        {
            string[,] result = new string[100, 2];
            int i = 0;
            Type storedClass = Type.GetType(fullname);
            if (storedClass == null) return result;
            PropertyInfo[] mb = storedClass.GetProperties(BindingFlags.DeclaredOnly |
            BindingFlags.Public |
            BindingFlags.Instance);
            IList<PropertyInfo> props = new List<PropertyInfo>(mb);

            foreach (var prop in props)
            {
                if (!prop.Name.Equals("TotalRows") && !prop.Name.Equals(Char.ToUpperInvariant(model[0]) + model.Substring(1) + "Id"))
                {
                        result[i, 0] = prop.PropertyType.ToString();
                        result[i, 1] = prop.Name;
                        i++;
                }
            }
             i1 = i;
            return result;
        }


    }
}

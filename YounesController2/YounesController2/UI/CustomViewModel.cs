using Microsoft.AspNet.Scaffolding;
using Microsoft.AspNet.Scaffolding.EntityFramework;
using System.Collections.Generic;
using System.Linq;

namespace YounesController.UI
{
    /// <summary>
    /// View model for code types so that it can be displayed on the UI.
    /// </summary>
    public class CustomViewModel
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">The code generation context</param>
        public CustomViewModel(CodeGenerationContext context)
        {
            Context = context;
        }

        /// <summary>
        /// This gets all the Model types from the active project.
        /// </summary>
        public IEnumerable<ModelType> ModelTypes
        {
            get
            {
                ICodeTypeService codeTypeService = (ICodeTypeService)Context
                    .ServiceProvider.GetService(typeof(ICodeTypeService));

                return codeTypeService
                    .GetAllCodeTypes(Context.ActiveProject)
                    .Select(codeType => new ModelType(codeType))
                      .Where(codeType => codeType.CodeType.FullName.Contains("Models") && !codeType.CodeType.FullName.EndsWith("_Result"))
                      .OrderBy(codeType => codeType.CodeType.Name);
                    
            }
        }


        public IEnumerable<ModelType> ModelTypes2
        {
            get
            {
                ICodeTypeService codeTypeService = (ICodeTypeService)Context
                    .ServiceProvider.GetService(typeof(ICodeTypeService));

                return codeTypeService
                    .GetAllCodeTypes(Context.ActiveProject)
                       .Select(codeType => new ModelType(codeType))
                      .Where(codeType => codeType.CodeType.FullName.Contains("Entities"));
            }
        }


        public IEnumerable<ModelType> ModelTypes3
        {
            get
            {
                ICodeTypeService codeTypeService = (ICodeTypeService)Context
                    .ServiceProvider.GetService(typeof(ICodeTypeService));

                return codeTypeService
                    .GetAllCodeTypes(Context.ActiveProject)
                      .Where(codeType => codeType.FullName.Contains("_Result") && !codeType.FullName.Contains("spWS_"))
                    .Select(codeType => new ModelType(codeType))
                    .OrderBy(codeType => codeType.CodeType.Name);
            }
        }


        public ModelType SelectedModelType { get; set; }
        public ModelType SelectedModelType2 { get; set; }

        public ModelType SelectedModelType3 { get; set; }

        public bool isCreate { get; set; }
        public bool isUpdate { get; set; }
        public bool isDelete { get; set; }

        public bool isActivateDisactivate { get; set; }

        public bool isOrderNum { get; set; }


        public string plural { get; set; }
        public string singular { get; set; }
        public bool isFemale { get; set; }

        public CodeGenerationContext Context { get; private set; }
    }
}

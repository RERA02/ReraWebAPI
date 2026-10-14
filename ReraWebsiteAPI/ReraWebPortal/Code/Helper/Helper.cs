using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ReraWebPortal.Code.Helper
{
    public class Helper
    {
    }

    public static class ExtentionHelper
    {
        /// <summary>
        /// Returns a Key/Value pair with all the errors in the model
        /// according to the data annotation properties.
        /// </summary>
        /// <param name="errDictionary"></param>
        /// <returns>
        /// Key: Name of the property
        /// Value: The error message returned from data annotation
        /// </returns>
        public static List<object> GetModelErrors(this ModelStateDictionary errDictionary)
        {
            List<object> lst = new List<object>();
            errDictionary.Where(k => k.Value.Errors.Count > 0).ToList().ForEach(i =>
            {
                foreach (var item in i.Value.Errors.Select(e => e.ErrorMessage))
                {
                    lst.Add(new
                    {
                        ProptyName = i.Key,
                        ErrorMessage = item
                    });
                }

            });
            return lst;
        }
    }
}

using System.Reflection;

namespace Exate.Rules.WebApi.DataAccess.Helper
{
    public static class GetSessionHelper
    {
        public static T PopulateIds<T>(T obj, int firmId, int userFirmId = 0) where T : class
        {
            //TODO this could be more efficient by using an common interface to set values rather than using System.Reflection
            PropertyInfo prop = obj.GetType().GetProperty("FirmId");
            prop.SetValue(obj, firmId);

            if (userFirmId > 0)
            {
                prop = obj.GetType().GetProperty("UserFirmId");
                prop.SetValue(obj, userFirmId);
            }

            return obj;
        }
    }
   
}

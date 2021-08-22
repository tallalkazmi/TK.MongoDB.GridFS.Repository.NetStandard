using MongoDB.Bson;

namespace TK.MongoDB.GridFS.Repository.Classes
{
    /// <summary>
    /// BsonValue conversion
    /// </summary>
    public static class BsonValueConversion
    {
        /// <summary>
        /// Converts BsonValue to C# equivalent type
        /// </summary>
        /// <param name="bsonValue">BsonValue</param>
        /// <returns>C# type</returns>
        public static object Convert(BsonValue bsonValue)
        {
            if (bsonValue.IsString)
                return System.Convert.ToString(bsonValue);
            //if (bsonValue.IsInt32)
            //    return System.Convert.ToInt32(bsonValue);
            if (bsonValue.IsInt32 || bsonValue.IsInt64 || bsonValue.IsDouble)
                return System.Convert.ToInt64(bsonValue);
            if (bsonValue.IsBoolean)
                return System.Convert.ToBoolean(bsonValue);
            if (bsonValue.IsValidDateTime)
                return System.Convert.ToDateTime(bsonValue);
            if (bsonValue.IsDecimal128)
                return System.Convert.ToDecimal(bsonValue);
            if (bsonValue.IsObjectId)
                return System.Convert.ToString(bsonValue);
            else
                return null;
        }
    }
}

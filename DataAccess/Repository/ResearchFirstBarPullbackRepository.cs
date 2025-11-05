using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.VisualBasic;
using Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataAccess.Repository
{
    internal class ResearchFirstBarPullbackRepository : Repository<ResearchFirstBarPullback>, IResearchFirstBarPullbackRepository
    {
        private ApplicationDbContext _db;

        public ResearchFirstBarPullbackRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void UpdateRange(IEnumerable<ResearchFirstBarPullback> entities)
        {
            _db.ResearchFirstBarPullbacks.UpdateRange(entities);
        }

        public async Task UpdateAsync(ResearchFirstBarPullback researchTrade)
        {
            // Get the object from the database
            // TODO: if objFromDb is null, I don't get any error message after Update()
            ResearchFirstBarPullback? objFromDb = await _db.ResearchFirstBarPullbacks.FindAsync(researchTrade.Id);
            if (objFromDb != null)
            {
                // Get the type of the object
                Type type = typeof(ResearchFirstBarPullback);

                // Iterate over the properties of the object
                foreach (PropertyInfo property in type.GetProperties())
                {
                    // Skip the Id and the SampleSizeId properties to avoid unnecessary changes
                    if (property.Name == "Id" || property.Name == "SampleSizeId")
                    {
                        continue;
                    }

                    // Get the value of the property from the researchTrade object
                    object? value = property.GetValue(researchTrade);

                    // The property.SetValue method will accept an object and automatically convert it to the correct data type for the property, as long as the value is compatible with the property type.However, there are a few things to be aware of:
                    // 1. Type Compatibility: The object returned by property.GetValue must be of a type that is assignable to the property type.If the types are not compatible, a runtime exception will be thrown.
                    // 2. Null Values: If the property can accept null values(for example, a reference type or a nullable value type), then setting a null value will work fine. However, setting a null value on a non-nullable value type will result in an exception.
                    try
                    {
                        // Set the value of the property in the objFromDb object
                        property.SetValue(objFromDb, value);
                    }
                    catch (Exception ex)
                    {

                    }

                }
            }
        }
    }
}

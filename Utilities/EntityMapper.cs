using Microsoft.Extensions.Primitives;
using Models;
using Models.ViewModels.DisplayClasses;

namespace Utilities
{
    public class EntityMapper
    {
        /// <summary>
        ///  Converts the values from an entity to a display class.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TViewModelProperty"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static TViewModelProperty EntityToViewModel<TEntity, TViewModelProperty>(TEntity entity) where TViewModelProperty : new()
        {
            var currentTrade = new TViewModelProperty();
            var entityType = typeof(TEntity);
            var viewModelPropType = typeof(TViewModelProperty);

            foreach (var entityProp in entityType.GetProperties())
            {
                var viewModelProp = viewModelPropType.GetProperty(entityProp.Name + "Display");
                if (entityProp.ToString().Contains("Screenshot"))
                {
                    var value = entityProp.GetValue(entity) as List<string>;
                    (currentTrade as ResearchFirstBarPullbackDisplay).ScreenshotsUrls = value;
                }

                if (viewModelProp != null)
                {
                    if (entityProp.PropertyType == typeof(bool) && viewModelProp.PropertyType == typeof(string))
                    {
                        var value = (bool)entityProp.GetValue(entity);
                        viewModelProp.SetValue(currentTrade, value ? "1" : "0");
                    }
                    else if (entityProp.PropertyType == typeof(int) && viewModelProp.PropertyType == typeof(string) || Nullable.GetUnderlyingType(entityProp.PropertyType) == typeof(int))
                    {
                        var value = entityProp.GetValue(entity)?.ToString();
                        viewModelProp.SetValue(currentTrade, value);
                    }
                    else if ((entityProp.PropertyType == typeof(double) || entityProp.PropertyType == typeof(double?)) && viewModelProp.PropertyType == typeof(string))
                    {
                        var value = entityProp.GetValue(entity)?.ToString();
                        viewModelProp.SetValue(currentTrade, value);
                    }
                    // The Trade class has a List<double> for the targets
                    else if (entityProp.PropertyType == typeof(List<double>) || Nullable.GetUnderlyingType(entityProp.PropertyType) == typeof(List<double>))
                    {
                        var value = entityProp.GetValue(entity) as List<double>;
                        var stringValue = value != null ? string.Join("; ", value) : string.Empty;
                        viewModelProp.SetValue(currentTrade, stringValue);
                    }
                    else
                    {
                        viewModelProp.SetValue(currentTrade, entityProp.GetValue(entity));
                    }
                }
            }

            return currentTrade;
        }

        public static void ViewModelToEntity<T>(T dbEntity, T viewData) where T : class, new()
        {
            if (dbEntity == null || viewData == null)
            {
                throw new ArgumentNullException("Entities cannot be null");
            }

            var entityType = typeof(T);
            var properties = entityType.GetProperties();

            foreach (var property in properties)
            {
                var viewValue = property.GetValue(viewData);
                if (viewValue != null) // Check if value is set
                {
                    property.SetValue(dbEntity, viewValue);
                }
            }
        }


        /// <summary>
        ///  Converts the values from a display class to an entity.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TViewModelProperty"></typeparam>
        /// <param name="viewModel"></param>
        /// <returns></returns>

        public static TEntity ViewModelDisplayToEntity<TEntity, TViewModelProperty>(TViewModelProperty viewModel, TEntity existingEntity) where TEntity : new()
        {
            var entity = existingEntity ?? new TEntity();
            var entityType = typeof(TEntity);
            var viewModelPropType = typeof(TViewModelProperty);

            foreach (var viewModelProp in viewModelPropType.GetProperties())
            {
                // Don't set values which are null. Example: UpdateTradeData() - the user updates some of the properties.
                if (viewModelProp.GetValue(viewModel) == null)
                {
                    continue;
                }

                string VMPropName = viewModelProp.Name.Replace("Display", "");
                var entityProp = entityType.GetProperty(VMPropName);
                if (entityProp != null)
                {
                    if (viewModelProp.PropertyType == typeof(string) && entityProp.PropertyType == typeof(bool))
                    {
                        var value = (string)viewModelProp.GetValue(viewModel);
                        entityProp.SetValue(entity, value == "1");
                    }
                    else if (viewModelProp.PropertyType == typeof(string) && entityProp.PropertyType == typeof(int))
                    {
                        if (int.TryParse((string)viewModelProp.GetValue(viewModel), out int intValue))
                        {
                            entityProp.SetValue(entity, intValue);
                        }
                        else
                        {
                            // Set default values. The value from the view maybe empty string (e.g. UpdateTradeData() - some of the properties are being changed by the user)
                            entityProp.SetValue(entity, default);
                        }
                    }
                    else if (viewModelProp.PropertyType == typeof(string) && entityProp.PropertyType == typeof(double) || viewModelProp.PropertyType == typeof(string) && entityProp.PropertyType == typeof(double?))
                    {
                        if (double.TryParse((string)viewModelProp.GetValue(viewModel), out double intValue))
                        {
                            entityProp.SetValue(entity, intValue);
                        }
                        else
                        {
                            entityProp.SetValue(entity, default);
                        }
                    }
                    else if (viewModelProp.PropertyType == typeof(string) && entityProp.PropertyType == typeof(List<double>) || Nullable.GetUnderlyingType(entityProp.PropertyType) == typeof(List<double>))
                    {
                        //// A lot of properties can be null. Not all properties have values, especially when creating a new trade.
                        string[] valueArray = ((string)viewModelProp.GetValue(viewModel)).Split("; ");
                        List<double> doubleList = new List<double>();
                        foreach (string value in valueArray)
                        {
                            if (double.TryParse(value, out double intValue))
                            {
                                doubleList.Add(intValue);
                            }
                        }
                        entityProp.SetValue(entity, doubleList);
                    }
                    else
                    {
                        entityProp.SetValue(entity, viewModelProp.GetValue(viewModel));
                    }
                }
            }

            return entity;
        }
    }
}

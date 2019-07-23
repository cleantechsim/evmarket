
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

using Microsoft.Azure.Cosmos.Table;

namespace CleanTechSim.EVMarket.Helpers.Storage.AzureTableStorage
{
    public class ReflectionMapper
    {
        private readonly IDictionary<string, TypeInfo> typesByTableName;

        public ReflectionMapper(params Type[] types)
            : this(new List<Type>(types))
        {

        }

        public ReflectionMapper(IEnumerable<Type> types)
        {
            this.typesByTableName = new Dictionary<string, TypeInfo>(types.Count());

            foreach (Type type in types)
            {
                this.typesByTableName[GetTableName(type)] = MakeTypeInfo(type);
            }
        }

        internal static string GetTableName(Type type)
        {
            return type.Name;
        }

        private static KeyInfo MakeKeyInfo(IEnumerable<PropertyInfo> properties, Type attributeType)
        {

            IEnumerable<PropertyInfo> keyProperties
                = from property in properties
                  where Attribute.IsDefined(property, attributeType)
                  orderby ((CompositeKeyAttribute)property.GetCustomAttribute(attributeType)).Order
                  select property;

            int count = keyProperties.Count();

            KeyInfo keyInfo;

            switch (count)
            {
                case 0:
                    throw new ArgumentException("No annotated properties of type " + attributeType.Name);

                case 1:
                default:
                    keyInfo = new KeyInfo(keyProperties, "_");
                    break;

            }

            return keyInfo;
        }

        private static TypeInfo MakeTypeInfo(Type type)
        {
            PropertyInfo[] properties = type.GetProperties();

            KeyInfo partitionKeyInfo = MakeKeyInfo(properties, typeof(PartitionKey));
            KeyInfo rowKeyInfo = MakeKeyInfo(properties, typeof(RowKey));

            Dictionary<string, PropertyInfo> valueProperties = new Dictionary<string, PropertyInfo>(properties.Length);

            foreach (PropertyInfo property in properties)
            {
                if (!partitionKeyInfo.Properties.Contains(property)
                 && !rowKeyInfo.Properties.Contains(property))
                {
                    valueProperties[property.Name] = property;
                }
            }

            return new TypeInfo(type, partitionKeyInfo, rowKeyInfo, valueProperties);
        }

        public T Convert<T>(Type type, string partitionKey, string rowKey, IDictionary<string, EntityProperty> properties)
        {
            return Convert<T>(GetTableName(type), partitionKey, rowKey, properties);
        }


        public T Convert<T>(string typeTableName, string partitionKey, string rowKey, IDictionary<string, EntityProperty> properties)
        {

            TypeInfo typeInfo = typesByTableName[typeTableName];

            T instance = (T)typeInfo.Type.GetConstructor(Type.EmptyTypes).Invoke(new object[0]);

            if (typeInfo == null)
            {
                throw new ArgumentException("No type info for type " + typeTableName);
            }

            ConvertString(instance, typeInfo.PartitionKey, partitionKey);
            ConvertString(instance, typeInfo.RowKey, rowKey);

            foreach (string name in properties.Keys)
            {
                EntityProperty entityProperty = properties[name];
                PropertyInfo valueProperty = typeInfo.ValueProperties[name];

                object value = GetEntityPropertyValue(valueProperty, entityProperty);

                valueProperty.SetValue(instance, value);
            }

            return instance;
        }

        private static void ConvertString(object instance, KeyInfo keyInfo, string stringValue)
        {
            if (keyInfo.Properties.Count() == 1)
            {
                PropertyInfo property = keyInfo.Properties.ElementAt(0);

                object value = ConvertString(property.PropertyType, stringValue);

                property.SetValue(instance, value);
            }
            else
            {
                int num = keyInfo.Properties.Count();
                string[] split = stringValue.Split(keyInfo.Separator);

                if (num != split.Length)
                {
                    throw new InvalidOperationException();
                }

                for (int i = 0; i < num; ++i)
                {
                    PropertyInfo property = keyInfo.Properties.ElementAt(i);

                    object value = ConvertString(property.PropertyType, split[i]);

                    property.SetValue(instance, value);
                }
            }

        }

        private static object ConvertString(Type valueType, string stringValue)
        {
            object value;

            if (valueType.Equals(typeof(int)))
            {
                int intValue;

                if (!int.TryParse(stringValue, out intValue))
                {
                    throw new ArgumentException();
                }

                value = intValue;
            }
            else if (valueType.Equals(typeof(string)))
            {
                value = stringValue;
            }
            else if (valueType.Equals(typeof(List<string>)))
            {
                value = stringValue.Split(',').Select(s => s.Trim()).ToList();
            }
            else
            {
                throw new NotImplementedException();
            }

            return value;
        }

        private static object GetEntityPropertyValue(PropertyInfo valueProperty, EntityProperty entityProperty)
        {
            object value;

            Type valueType = valueProperty.PropertyType;
            EdmType entityType = entityProperty.PropertyType;

            switch (entityType)
            {
                case EdmType.String:
                    value = ConvertString(valueType, entityProperty.StringValue);
                    break;

                case EdmType.Int32:
                    if (valueType.Equals(typeof(int)))
                    {
                        value = entityProperty.Int32Value;
                    }
                    else if (valueType.Equals(typeof(int?)))
                    {
                        value = entityProperty.Int32Value;
                    }
                    else if (valueType.Equals(typeof(decimal)))
                    {
                        value = (decimal)entityProperty.Int32Value;
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                    break;

                case EdmType.Double:
                    if (valueType.Equals(typeof(decimal)))
                    {
                        value = (decimal)entityProperty.DoubleValue;
                    }
                    else if (valueType.Equals(typeof(decimal?)))
                    {
                        value = (decimal?)entityProperty.DoubleValue;
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                    break;

                default:
                    throw new NotImplementedException();

            }

            return value;
        }
    }

    internal class KeyInfo
    {
        public IEnumerable<PropertyInfo> Properties { get; }
        public string Separator { get; }

        public KeyInfo(IEnumerable<PropertyInfo> properties, string separator)
        {
            this.Properties = properties;
            this.Separator = separator;
        }
    }

    internal class TypeInfo
    {
        public Type Type { get; }
        public KeyInfo PartitionKey { get; }
        public KeyInfo RowKey { get; }
        public IDictionary<string, PropertyInfo> ValueProperties { get; }

        public TypeInfo(Type type, KeyInfo partitionKey, KeyInfo rowKey, IDictionary<string, PropertyInfo> valueProperties)
        {
            this.Type = type;
            this.PartitionKey = partitionKey;
            this.RowKey = rowKey;
            this.ValueProperties = valueProperties;
        }
    }
}
using ImageCampus.ToolBox.Cast;
using ImageCampus.ToolBox.Services;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ImageCampus.ToolBox.Blueprints
{
    public sealed class BlueprintBinder : IService
    {
        public bool IsPersistance => true;

        private BlueprintRegistry BlueprintRegistry => ServiceProvider.Instance.GetService<BlueprintRegistry>();

        private Dictionary<Type, FieldInfo[]> fieldsInType;
        private Dictionary<FieldInfo, (bool hasAttribute, BlueprintParameterAttribute attribute)> attributeInFields;

        public BlueprintBinder()
        {
            fieldsInType = new Dictionary<Type, FieldInfo[]>();
            attributeInFields = new Dictionary<FieldInfo, (bool hasAttribute, BlueprintParameterAttribute attribute)>();
        }

        public void Apply(ref object instance, string blueprintTable, string blueprintID)
        {
            Type instanceType = instance.GetType();

            do
            {
                foreach (FieldInfo fieldInfo in GetFields(instanceType))
                {
                    (bool hasAttribute, BlueprintParameterAttribute attribute) blueprintParameter = 
                        GetBlueprintParameterAttribute(fieldInfo);

                    if (blueprintParameter.hasAttribute)
                    {
                        fieldInfo.SetValue(instance, StringCast.Convert(
                            BlueprintRegistry.BlueprintDatas[blueprintTable]
                            [blueprintID, blueprintParameter.attribute.ParameterHeader],
                            fieldInfo.FieldType));
                    }
                }
                instanceType = instanceType.BaseType;

            } while (instanceType != typeof(object));

        }

        private FieldInfo[] GetFields(Type type)
        {
            if (!fieldsInType.ContainsKey(type))
                fieldsInType.Add(type, type.GetFields(
                    BindingFlags.Public | BindingFlags.NonPublic |
                    BindingFlags.Instance | BindingFlags.DeclaredOnly));
            return fieldsInType[type];
        }

        private (bool hasAttribute, BlueprintParameterAttribute attribute)
            GetBlueprintParameterAttribute(FieldInfo fieldInfo)
        {
            if (!attributeInFields.ContainsKey(fieldInfo))
            {
                bool contains = false;
                foreach (Attribute attribute in fieldInfo.GetCustomAttributes())
                {
                    if (attribute is BlueprintParameterAttribute)
                    {
                        attributeInFields.Add(fieldInfo, (true, attribute as BlueprintParameterAttribute));
                        contains = true;
                        break;
                    }
                }

                if (!contains)
                    attributeInFields.Add(fieldInfo, (false, null));
            }
            return attributeInFields[fieldInfo];
        }
    }
}